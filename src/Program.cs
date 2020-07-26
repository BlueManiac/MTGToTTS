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

            collection.AddHttpClient<ScryfallApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.scryfall.com/");
            });
            collection.AddSingleton<CardParser>();
            collection.AddSingleton(x => new Options {
                FilePath = args.Length > 0
                    ? args[0]
                    : null
            });
            collection.AddSingleton<DeckCreator>();

            var services = collection.BuildServiceProvider();

            var options = services.GetService<Options>();

            await options.Expand();

            var filePaths = options.FilePath != null
                ? Enumerable.Empty<string>().Append(options.FilePath)
                : GetFilePaths("DelverLensDecks");

            var decks = ParseDecks(new DelverLensParser(), filePaths);
            
            var parser = services.GetService<CardParser>();
            var deckCreator = services.GetService<DeckCreator>();

            foreach (var deck in decks)
            {
                Console.Write("Parsing {0}... ", deck.Name);

                try {
                    // Parse and sort using scryfall
                    var cards = await parser.Parse(deck.Cards);

                    // Save TTS deck file
                    var filePath = deckCreator.SaveDeckFile(deck, cards);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ResetColor();
                    Console.WriteLine("Deck was saved to {0}.", filePath);
                }
                catch (Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("ERROR");
                    Console.Error.WriteLine(ex);
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public static IEnumerable<string> GetFilePaths(string folder, string path = null) {
            path = path ?? Directory.GetCurrentDirectory();

            Directory.CreateDirectory(Path.Combine(path, folder));

            return Directory
                .GetFiles(folder)
                .Select(x => Path.Combine(path, x));
        }

        private static IEnumerable<Deck> ParseDecks(IDeckFileParser parser, IEnumerable<string> filePaths)
        {
            return filePaths
                .Select(filePath => new Deck
                {
                    FilePath = filePath,
                    Cards = parser.Parse(filePath),
                    Name = Path.GetFileNameWithoutExtension(filePath)
                });
        }
    }
}
