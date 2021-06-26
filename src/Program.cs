using System.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DeckParser.BackImages;
using DeckParser.FileParsers;
using DeckParser.Models;
using DeckParser.TabletopSimulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScryfallApi.Client;

namespace DeckParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DeckParser.json");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(config => {
                    config.ClearProviders();
                })
                .ConfigureHostConfiguration(x => {
                    x.AddJsonFile(optionsFilePath, optional: true);
                })
                .ConfigureServices((context, collection) => {
                    collection.Configure<Options>(context.Configuration);
                    collection.PostConfigure<Options>(x => x.FilePath = args.Length > 0 ? args[0] : null);
                    collection.AddSingleton(x => x.GetService<IOptions<Options>>().Value);

                    collection.AddHttpClient<ScryfallApiClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.scryfall.com/");
                    });
                    collection.RegisterBackImageResolvers();

                    collection.AddSingleton<DelverLensParser>();
                    collection.AddSingleton<CardParser>();
                    collection.AddSingleton<DeckCreator>();
                })
                .Build();

            var options = host.Services.GetService<Options>();

            if (!File.Exists(optionsFilePath)) {
                var json = JsonSerializer.Serialize(options, new JsonSerializerOptions {
                    WriteIndented = true,
                });

                await File.WriteAllTextAsync(optionsFilePath, json);
            }

            await options.Expand();
            
            var parser = host.Services.GetService<DelverLensParser>();
            var cardParser = host.Services.GetService<CardParser>();
            var deckCreator = host.Services.GetService<DeckCreator>();

            if (options.FilePaths.Length == 0) {
                Console.WriteLine($@"No deck files could be found in ""{options.ImportPath}"".");
            }

            foreach (var filePath in options.FilePaths)
            {
                try {
                    var deck = new Deck {
                        FilePath = filePath,
                        Cards = parser.Parse(filePath),
                        Name = Path.GetFileNameWithoutExtension(filePath),
                        BackImageFilePath = RelatedImageResolver.Find(filePath)
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
                catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("ERROR");
                    Console.Error.WriteLine("Could not parse {0}.", filePath);
                    Console.Error.WriteLine(ex);
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
    
}
