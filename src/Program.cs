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

            var services = collection.BuildServiceProvider();

            var client = services.GetService<ScryfallApiClient>();

            var path = Directory.GetCurrentDirectory();

            var decks = ParseDecks("DelverLensDecks", new DelverLensParser(), path)
                .Concat(ParseDecks("TappedOutDecks", new TappedOutParser(), path));

            //var resultPath = Path.Combine(path, "deckfiles");
            var resultPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported");
            Directory.CreateDirectory(resultPath);
            
            var parser = services.GetService<CardParser>();

            foreach (var deck in decks)
            {
                // Parse and sort using scryfall
                var cards = await parser.Parse(deck.Cards);

                DeckCreator.SaveDeckFiles(resultPath, deck, cards);
            }
        }

        private static IEnumerable<Deck> ParseDecks(string folder, IDeckFileParser parser, string path)
        {
            Directory.CreateDirectory(Path.Combine(path, folder));

            return Directory.GetFiles(folder)
                .Select(x => Path.Combine(path, x))
                .Select(filePath => new Deck
                {
                    FilePath = filePath,
                    Cards = parser.Parse(filePath),
                    Name = Path.GetFileNameWithoutExtension(filePath)
                });
        }
    }
}
