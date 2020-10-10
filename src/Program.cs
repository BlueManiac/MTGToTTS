using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeckParser.FileParsers;
using DeckParser.Models;
using DeckParser.TabletopSimulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ScryfallApi.Client;

namespace DeckParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var basePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            var host = Host.CreateDefaultBuilder(args)
                .UseContentRoot(basePath)
                .ConfigureLogging(config => {
                    config.ClearProviders();
                })
                .ConfigureHostConfiguration(x => {
                    x.SetBasePath(basePath);
                    x.AddJsonFile("DeckParser.json", optional: true);
                })
                .ConfigureServices((context, collection) => {
                    collection.Configure<Options>(context.Configuration);
                    collection.PostConfigure<Options>(x => x.FilePath = args.Length > 0 ? args[0] : null);
                    collection.AddSingleton(x => x.GetService<IOptions<Options>>().Value);

                    collection.AddHttpClient<ScryfallApiClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.scryfall.com/");
                    });

                    collection.AddSingleton<DelverLensParser>();
                    collection.AddSingleton<CardParser>();
                    collection.AddSingleton<DeckCreator>();
                })
                .Build();

            var options = host.Services.GetService<Options>();
            var optionsFilePath = Path.Combine(basePath, "DeckParser.json");

            if (!File.Exists(optionsFilePath)) {
                var json = JsonConvert.SerializeObject(options, new JsonSerializerSettings { Formatting = Formatting.Indented});

                await File.WriteAllTextAsync(optionsFilePath, json);
            }

            await options.Expand();
            
            var parser = host.Services.GetService<DelverLensParser>();
            var cardParser = host.Services.GetService<CardParser>();
            var deckCreator = host.Services.GetService<DeckCreator>();

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
