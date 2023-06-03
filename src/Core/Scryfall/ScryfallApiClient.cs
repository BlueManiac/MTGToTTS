using Core.Scryfall.Models;
using Core.Scryfall.Requests;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Scryfall;
public class ScryfallApiClient
{
    private readonly HttpClient _httpClient;

    public ScryfallApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultList<Card>> Collection(IEnumerable<string> ids)
    {
        var data = new CollectionRequest
        {
            Identifiers = ids.Select(x => new CollectionRequestId { Id = x }).ToArray()
        };

        var response = await _httpClient.PostAsJsonAsync("/cards/collection", data, ScryfallSourceGenerationContext.Default.CollectionRequest).ConfigureAwait(false);
        var jsonStream = await response.Content.ReadAsStreamAsync();
        var obj = await JsonSerializer.DeserializeAsync(jsonStream, ScryfallSourceGenerationContext.Default.ResultListCard);

        if (obj.ObjectType.Equals("error", StringComparison.OrdinalIgnoreCase))
        {
            jsonStream.Position = 0;
            var error = await JsonSerializer.DeserializeAsync(jsonStream, ScryfallSourceGenerationContext.Default.Error);
            throw new ScryfallApiException(error?.Details)
            {
                ResponseStatusCode = response.StatusCode,
                RequestUri = response.RequestMessage?.RequestUri,
                RequestMethod = response.RequestMessage?.Method,
                ScryfallError = error
            };
        }

        return obj;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CollectionRequest))]
[JsonSerializable(typeof(ResultList<Card>))]
[JsonSerializable(typeof(Models.Error))]
internal partial class ScryfallSourceGenerationContext : JsonSerializerContext
{
}