using Core;
using Core.BackImages;
using Core.CardFileFormatParsers;
using Core.FileParsers;
using Core.Parser;
using Core.Parser.Models;
using Core.Scryfall;
using Core.TabletopSimulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Logging.ClearProviders();
}

builder.Configuration.AddJsonFile(ParserFileConfig.FILENAME, optional: true);

var fileConfig = builder.Configuration.Get<ParserFileConfig>()!;

builder.Services.AddSingleton(x => new ParserConfig(fileConfig, args.FirstOrDefault()));

builder.Services.AddScryfallApiClient();
builder.Services.AddSingleton<DeckFileObserver>();
builder.Services.AddSingleton<CardParser>();
builder.Services.AddSingleton<DelverLensDeckFileParser>();
builder.Services.RegisterBackImageResolvers();
builder.Services.AddSingleton<TabletopSimulatorDeckCreator>();

var host = builder.Build();

await fileConfig.CreateFile();

var config = host.Services.GetRequiredService<ParserConfig>();
var fileObserver = host.Services.GetRequiredService<DeckFileObserver>();
var fileParser = host.Services.GetRequiredService<DelverLensDeckFileParser>();
var cardParser = host.Services.GetRequiredService<CardParser>();
var deckCreator = host.Services.GetRequiredService<TabletopSimulatorDeckCreator>();

foreach (var filePath in config.FilePaths)
{
    await TryParseFile(filePath);
}

var lifeTime = host.Services.GetRequiredService<IHostApplicationLifetime>();

Console.WriteLine("Watching for files in {0}...", config.ImportPath);
Console.WriteLine("Press CTRL-C to quit");

await foreach (var (filePath, change) in fileObserver.ObserveImportDirectory(lifeTime.ApplicationStopping))
{
    await TryParseFile(filePath);
}

async Task TryParseFile(string filePath)
{
    try
    {
        if (!fileParser.IsValidFile(filePath))
            return;

        var deck = new Deck
        {
            FilePath = filePath,
            Cards = fileParser.Parse(filePath),
            Name = Path.GetFileNameWithoutExtension(filePath)
        };

        Console.Write("Parsing {0}... ", deck.Name);

        // Parse and sort using scryfall
        var cards = await cardParser.Parse(deck.Cards.Where(x => !x.Exclude));

        // Save TTS deck file
        var resultFilePath = await deckCreator.SaveDeckFile(deck, cards);

        // Move parsed deck file if needed
        fileObserver.MoveParsedFile(filePath);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("OK");
        Console.ResetColor();
        Console.WriteLine("Deck was saved to {0}.", resultFilePath);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("ERROR");
        Console.Error.WriteLine("Could not parse {0}.", filePath);
        Console.Error.WriteLine(ex);
        Console.ResetColor();
    }
}