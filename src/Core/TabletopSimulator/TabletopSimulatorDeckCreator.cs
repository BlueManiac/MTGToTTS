using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Scryfall.Models;
using Core.Parser.Models;
using Core.BackImages;
using System.Text.RegularExpressions;

namespace Core.TabletopSimulator;

public partial class TabletopSimulatorDeckCreator
{
    private readonly ParserConfig _config;
    private readonly IEnumerable<IBackImageResolver> _backImageResolvers;

    public TabletopSimulatorDeckCreator(ParserConfig config, IEnumerable<IBackImageResolver> backImageResolvers)
    {
        _config = config;
        _backImageResolvers = backImageResolvers;
    }

    public async Task<string> SaveDeckFile(Deck deck, IEnumerable<Card> cards)
    {
        string? backUrl = null;

        foreach (var resolver in _backImageResolvers)
        {
            backUrl = await resolver.Resolve(deck.FilePath, CancellationToken.None);

            if (backUrl != null)
            {
                break;
            }
        }

        if (backUrl == null)
        {
            throw new Exception($@"Could not create a back image for ""{deck.FilePath}"".");
        }

        Directory.CreateDirectory(_config.ResultPath);

        var state = new SaveState
        {
            ObjectStates = new List<ObjectState>() {
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

        var objects = state.ObjectStates[0].ContainedObjects = new List<ObjectState>();
        var customDeck = state.ObjectStates[0].CustomDeck = new Dictionary<int, CustomDeckState>();
        var deckIds = state.ObjectStates[0].DeckIDs = new List<int>();

        int id = 100;

        foreach (var card in cards)
        {
            var quantity = deck.Cards.First(x => x.ScryfallId == card.Id.ToString()).Quantity;
            var faceUrl = card.ImageUris == null
                ? card.CardFaces[0].ImageUris["border_crop"].ToString()
                : card.ImageUris["border_crop"].ToString();

            for (int i = 0; i < quantity; i++)
            {
                AddCard(id, $"{card.Name} ({card.TypeLine})", card.OracleText, faceUrl, backUrl, true);

                id += 100;
            }
        }

        // Add double faced cards to the top
        foreach (var card in cards.Where(x => x.ImageUris == null).DistinctBy(x => x.Id))
        {
            var faceUrl = card.CardFaces[0].ImageUris["border_crop"].ToString();
            var doubleFacedbackUrl = card.CardFaces[1].ImageUris["border_crop"].ToString();

            AddCard(id, $"{card.Name} [double faced]", card.OracleText, faceUrl, doubleFacedbackUrl, false);

            id += 100;
        }

        var deckName = _config.CleanDeckNames
            ? CleanDeckName(deck.Name)
            : deck.Name;

        var filePath = Path.Combine(_config.ResultPath, deckName) + ".json";

        using var fileStream = File.Create(filePath);

        await JsonSerializer.SerializeAsync(fileStream, state, DeckSourceGenerationContext.Default.SaveState);

        return filePath;

        void AddCard(int id, string nickName, string description, string faceUrl, string backUrl, bool backIsHidden)
        {
            objects.Add(new ObjectState
            {
                CardID = id,
                Name = "Card",
                Nickname = nickName,
                Description = description,
                Transform = new TransformState
                {
                    rotY = 180,
                    rotZ = 180,
                    scaleX = 1,
                    scaleY = 1,
                    scaleZ = 1
                }
            });

            customDeck[id / 100] = new CustomDeckState
            {
                FaceURL = faceUrl,
                BackURL = backUrl,
                NumHeight = 1,
                NumWidth = 1,
                BackIsHidden = backIsHidden
            };

            deckIds.Add(id);
        }
    }

    private static string CleanDeckName(string name)
    {
        var match = DeckNameRegex().Match(name);

        if (match.Success)
        {
            name = match.Groups[1].Value;
        }

        return name.Replace("_", " ");
    }

    [GeneratedRegex("^(.*?)_(\\d{4}_\\w+_\\d{2}-\\d{2})$")]
    private static partial Regex DeckNameRegex();
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SaveState))]
internal partial class DeckSourceGenerationContext : JsonSerializerContext
{
}