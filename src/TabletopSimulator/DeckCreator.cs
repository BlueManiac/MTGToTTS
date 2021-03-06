using System.Threading;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeckParser.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using DeckParser.BackImages;

namespace DeckParser.TabletopSimulator {
    public class DeckCreator {
        private readonly Options _options;
        private readonly IEnumerable<IBackImageResolver> _backImageResolvers;

        public DeckCreator(Options options, IEnumerable<IBackImageResolver> backImageResolvers)
        {
            _options = options;
            _backImageResolvers = backImageResolvers;
        }

        public async Task<string> SaveDeckFile(Deck deck, IEnumerable<ScryfallApi.Client.Models.Card> cards)
        {
            string backUrl = null;

            foreach (var resolver in _backImageResolvers)
            {
                backUrl = await resolver.Resolve(deck.FilePath, CancellationToken.None);

                if (backUrl != null) {
                    break;
                }
            }

            if (backUrl == null) {
                throw new Exception($@"Could not create a back image for ""{deck.FilePath}"".");
            }

            Directory.CreateDirectory(_options.ResultPath);

            var state = new SaveState {
                ObjectStates = new System.Collections.Generic.List<ObjectState>() {
                    new ObjectState {
                        Name = "DeckCustom",
                        Nickname = deck.Name,
                        Transform = new TransformState {
                            rotY = 180,
                            scaleX = 1.645f,
                            scaleY = 1,
                            scaleZ = 1.645f
                        }
                    }
                }
            };

            var objects = state.ObjectStates[0].ContainedObjects = new System.Collections.Generic.List<ObjectState>();
            var customDeck = state.ObjectStates[0].CustomDeck = new System.Collections.Generic.Dictionary<int, CustomDeckState>();
            var deckIds = state.ObjectStates[0].DeckIDs = new List<int>();

            int id = 100;

            foreach (var card in cards)
            {
                var quantity = deck.Cards.First(x => x.ScryfallId == card.Id.ToString()).Quantity;
                var faceUrl = card.ImageUris == null
                    ? card.CardFaces[0].ImageUris["border_crop"].ToString()
                    : card.ImageUris["border_crop"].ToString();

                for (int i = 0; i < quantity; i++) {
                    AddCard(id, $"{card.Name} ({card.TypeLine})", faceUrl, backUrl, true);
                    
                    id += 100;
                }
            }

            // Add double faced cards to the top
            foreach (var card in cards.Where(x => x.ImageUris == null).DistinctBy(x => x.Id))
            {
                var faceUrl = card.CardFaces[0].ImageUris["border_crop"].ToString();
                var doubleFacedbackUrl = card.CardFaces[1].ImageUris["border_crop"].ToString();

                AddCard(id, $"{card.Name} [double faced]", faceUrl, doubleFacedbackUrl, false);
                    
                id += 100;
            }

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            var filePath = Path.Combine(_options.ResultPath, deck.Name) + ".json";
            File.WriteAllText(filePath, json);

            return filePath;

            void AddCard(int id, string nickName, string faceUrl, string backUrl, bool backIsHidden) {
                objects.Add(new ObjectState {
                    CardID = id,
                    Name = "Card",
                    Nickname = nickName,
                    Transform = new TransformState {
                        rotY = 180,
                        rotZ = 180,
                        scaleX = 1,
                        scaleY = 1,
                        scaleZ = 1
                    }
                });

                customDeck[id/100] = new CustomDeckState {
                    FaceURL = faceUrl,
                    BackURL = backUrl,
                    NumHeight = 1,
                    NumWidth = 1,
                    BackIsHidden = backIsHidden
                };

                deckIds.Add(id);
            }
        }
    }
}