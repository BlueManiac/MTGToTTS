using Core;
using Core.BackImages;
using Core.FileParsers;
using Core.Models;
using Core.Scryfall;
using Core.TabletopSimulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(ParserFileConfig.FILENAME, optional: true);

var fileConfig = builder.Configuration.Get<ParserFileConfig>()!;

builder.Services.AddSingleton(x => new ParserConfig(fileConfig, args.FirstOrDefault()));

builder.Services.AddScryfallApiClient();
builder.Services.AddSingleton<CardParser>();
builder.Services.AddSingleton<DelverLensParser>();
builder.Services.RegisterBackImageResolvers();
builder.Services.AddSingleton<DeckCreator>();

var host = builder.Build();

await fileConfig.CreateFile();

var options = host.Services.GetRequiredService<ParserConfig>();

if (options.FilePaths.Length == 0)
{
    Console.WriteLine($@"No deck files could be found in ""{options.ImportPath}"".");
}
else
{
    var parser = host.Services.GetRequiredService<DelverLensParser>();
    var cardParser = host.Services.GetRequiredService<CardParser>();
    var deckCreator = host.Services.GetRequiredService<DeckCreator>();

    foreach (var filePath in options.FilePaths)
    {
        try
        {
            if (!parser.IsValidFile(filePath))
                continue;

            var deck = new Deck
            {
                FilePath = filePath,
                Cards = parser.Parse(filePath),
                Name = Path.GetFileNameWithoutExtension(filePath)
            };

            Console.Write("Parsing {0}... ", deck.Name);

            // Parse and sort using scryfall
            var cards = await cardParser.Parse(deck.Cards.Where(x => !x.Exclude));

            // Save TTS deck file
            var resultFilePath = await deckCreator.SaveDeckFile(deck, cards);

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
}

Console.WriteLine();
Console.Write("Press any key to continue...");
Console.ReadKey();