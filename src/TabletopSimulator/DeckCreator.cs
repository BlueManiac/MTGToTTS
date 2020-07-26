using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeckParser.Models;
using Newtonsoft.Json;

namespace DeckParser.TabletopSimulator {
    public class DeckCreator {
        private readonly Options options;

        public DeckCreator(Options options)
        {
            this.options = options;
        }

        public string SaveDeckFile(Deck deck, IEnumerable<ScryfallApi.Client.Models.Card> cards)
        {
            Directory.CreateDirectory(options.ResultPath);

            var state = new SaveState {
                ObjectStates = new System.Collections.Generic.List<ObjectState>() {
                    new ObjectState {
                        Name = "DeckCustom",
                        Nickname = deck.Name,
                        Transform = new TransformState {
                            rotY = 180,
                            scaleX = 1,
                            scaleY = 1,
                            scaleZ = 1
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
                    AddCard(id, $"{card.Name} ({card.TypeLine})", faceUrl, options.BackUrl, true);
                    
                    id += 100;
                }
            }

            // Add double faced cards to the top
            foreach (var card in cards.Where(x => x.ImageUris == null).DistinctBy(x => x.Id))
            {
                var faceUrl = card.CardFaces[0].ImageUris["border_crop"].ToString();
                var backUrl = card.CardFaces[1].ImageUris["border_crop"].ToString();

                AddCard(id, $"{card.Name} [double faced]", faceUrl, backUrl, false);
                    
                id += 100;
            }

            var json = JsonConvert.SerializeObject(state, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var filePath = Path.Combine(options.ResultPath, deck.Name) + ".json";
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