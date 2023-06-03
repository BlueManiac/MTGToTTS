using System.Text.Json.Serialization;

namespace Core.Scryfall.Requests;

internal class CollectionRequest
{
    [JsonPropertyName("identifiers")]
    public CollectionRequestId[] Identifiers { get; set; }
}

internal class CollectionRequestId
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}