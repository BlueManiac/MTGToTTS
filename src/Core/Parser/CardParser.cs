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

        var commanders = list
            .Where(x => x.IsCommander)
            .OrderBy(x => x.TypeLine)
            .ThenBy(x => x.Name);
        var legendaries = list
            .Except(commanders)
            .Where(x => x.TypeLine.StartsWith("Legendary Creature"))
            .OrderByDescending(x => x.ColorIdentity.Length)
            .ThenBy(x => x.Name);
        var lands = list
            .Except(commanders)
            .Where(x => x.TypeLine.Split(" ").Contains("Land") && !x.IsCommander)
            .OrderBy(x => x.TypeLine.StartsWith("Basic Land") ? 0 : 1)
            .ThenBy(x => x.Name);
        var rest = list
            .Except(commanders)
            .Except(legendaries)
            .Except(lands)
            .OrderBy(x => x.TypeLine)
            .ThenBy(x => x.Name);

        return commanders
            .Concat(legendaries)
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
                    var scryfallId = item.Id.ToString();

                    item.IsCommander = chunk.First(x => x.ScryfallId == scryfallId).IsCommander;

                    yield return item;
                }
            }
        }
    }
}