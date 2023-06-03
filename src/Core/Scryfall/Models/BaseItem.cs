using System.Text.Json.Serialization;

namespace Core.Scryfall.Models;

public abstract class BaseItem
{
    [JsonPropertyName("object")]
    public string ObjectType { get; set; }
}