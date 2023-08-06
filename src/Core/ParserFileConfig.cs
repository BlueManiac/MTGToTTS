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
    public bool CleanDeckNames { get; set; } = true;

    public async Task CreateFile()
    {
        using var fileStream = File.Open(FILENAME, FileMode.Create, FileAccess.Write);

        await JsonSerializer.SerializeAsync(fileStream, this, ConfigSourceGenerationContext.Default.ParserFileConfig);
    }
}


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ParserFileConfig))]
internal partial class ConfigSourceGenerationContext : JsonSerializerContext
{
}