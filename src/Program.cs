using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeckParser.FileParsers;
using DeckParser.Models;
using DeckParser.TabletopSimulator;
using Microsoft.Extensions.DependencyInjection;
using ScryfallApi.Client;

namespace DeckParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();

            collection.AddSingleton(x => new Options {
                FilePath = args.Length > 0
                    ? args[0]
                    : null
            });
            collection.AddHttpClient<ScryfallApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.scryfall.com/");
            });

            collection.AddSingleton<DelverLensParser>();
            collection.AddSingleton<CardParser>();
            collection.AddSingleton<DeckCreator>();

            var services = collection.BuildServiceProvider();

            var options = services.GetService<Options>();

            await options.Expand();
            
            var parser = services.GetService<DelverLensParser>();
            var cardParser = services.GetService<CardParser>();
            var deckCreator = services.GetService<DeckCreator>();

            foreach (var filePath in options.FilePaths)
            {
                try {
                    var deck = new Deck {
                        FilePath = filePath,
                        Cards = parser.Parse(filePath),
                        Name = Path.GetFileNameWithoutExtension(filePath)
                    };

                    Console.Write("Parsing {0}... ", deck.Name);
                    
                    // Parse and sort using scryfall
                    var cards = await cardParser.Parse(deck.Cards);

                    // Save TTS deck file
                    var resultFilePath = deckCreator.SaveDeckFile(deck, cards);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ResetColor();
                    Console.WriteLine("Deck was saved to {0}.", resultFilePath);
                }
                catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("ERROR");
                    Console.WriteLine("Could not parse {0}.", filePath);
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
