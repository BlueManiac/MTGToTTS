using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.FileParsers;
using Core.Models;
using ScryfallApi.Client;
using static ScryfallApi.Client.Models.SearchOptions;

public class CardImageExtractor {
    public static async Task SaveImageFiles(string path, ScryfallApiClient client, IEnumerable<Deck> decks) {
        var sort = new CardSort();
        var webClient = new WebClient();
        var invalidChars = Path.GetInvalidFileNameChars();

        foreach (var deck in decks) {
            var deckName = Path.GetFileNameWithoutExtension(deck.FilePath);
            var deckPath = Path.Combine(path, "Downloaded", deckName);

            Directory.CreateDirectory(deckPath);

            foreach (var card in deck.Cards) {
                var name = new string(card.Name.Where(x => !invalidChars.Contains(x)).ToArray());

                var filePath = Path.Combine(deckPath, name + " (" + card.Quantity + ").jpg");

                if (File.Exists(filePath))
                    continue;

                Console.WriteLine("Downloading " + deckName + ": " + card.Name + "...");

                var data = await GetCard(card, client, sort);
                var images = data.ImageUris;

                if (images != null) {
                    var imageUri = images["border_crop"];

                    webClient.DownloadFile(imageUri.ToString(), filePath);
                }
                else {
                    Console.WriteLine("Multiple faces...");

                    webClient.DownloadFile(data.CardFaces[0].ImageUris["border_crop"].ToString(), filePath);

                    var facePath = Path.Combine(deckPath, name + " (" + card.Quantity + ")");
                    Directory.CreateDirectory(facePath);

                    int i = 1;
                    foreach (var face in data.CardFaces) {
                        filePath = Path.Combine(facePath, i++ + ".jpg");

                        var imageUri = face.ImageUris["border_crop"];

                        webClient.DownloadFile(imageUri.ToString(), filePath);
                    }
                }
            }
        }
    }

    private static async Task<ScryfallApi.Client.Models.Card> GetCard(CardEntry card, ScryfallApiClient client, CardSort sort) {
        if (card.ScryfallId != null) {
            try {
                return await client.Cards.GetById(card.ScryfallId);
            }
            catch (Exception) {
                Console.Error.WriteLine("Could not get data by id, trying by name instead...");
            }
        }
        
        var results = (await client.Cards.Search(card.Name, 0, sort)).Data;

        return results.FirstOrDefault(x => x.Name == card.Name) ?? results.First();
    } 
}