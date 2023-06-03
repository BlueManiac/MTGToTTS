using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;
public class ParserFileConfig
{
    public const string FILENAME = "DeckParser.json";

    public string ResultPath { get; set; } = @"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported";
    public string ImportPath { get; set; } = "Decks";
    public string BackUrl { get; set; } = "https://loremflickr.com/480/680";
    public string? ImgurClientKey { get; set; }

    public async Task CreateFile()
    {
        if (File.Exists(FILENAME))
            return;

        var json = JsonSerializer.Serialize(this, ConfigSourceGenerationContext.Default.ParserFileConfig);

        await File.WriteAllTextAsync(FILENAME, json);
    }
}


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ParserFileConfig))]
internal partial class ConfigSourceGenerationContext : JsonSerializerContext
{
}