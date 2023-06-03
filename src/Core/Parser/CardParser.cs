using Core.CardFileFormatParsers.Models;
using Core.Scryfall;
using Core.Scryfall.Models;

namespace Core.Parser;

public class CardParser
{
    private readonly ScryfallApiClient _client;

    public CardParser(ScryfallApiClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<Card>> Parse(IEnumerable<CardEntry> cards)
    {
        var list = await ParseInnerAsync(cards).ToListAsync();

        var legendaries = list
            .Where(x => x.TypeLine.StartsWith("Legendary Creature"))
            .OrderByDescending(x => x.ColorIdentity.Length)
            .ThenBy(x => x.Name);
        var lands = list
            .Where(x => x.TypeLine.Split(" ").Contains("Land"))
            .OrderBy(x => x.TypeLine.StartsWith("Basic Land") ? 0 : 1)
            .ThenBy(x => x.Name);
        var rest = list
            .Except(legendaries)
            .Except(lands)
            .OrderBy(x => x.TypeLine)
            .ThenBy(x => x.Name);

        return legendaries
            .Concat(rest)
            .Concat(lands)
            .Reverse();

        async IAsyncEnumerable<Card> ParseInnerAsync(IEnumerable<CardEntry> cards)
        {
            foreach (var chunk in cards.Chunk(75))
            {
                var result = await _client.Collection(chunk.Select(x => x.ScryfallId));

                foreach (var item in result.Data)
                {
                    yield return item;
                }
            }
        }
    }
}