using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;
public class ParserFileConfig
{
    public const string FILENAME = "DeckParser.json";

    public string ResultPath { get; set; } = @"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported";
    public string ImportPath { get; set; } = "Decks";
    public string CompletedPath { get; set; } = "Completed";
    public string BackUrl { get; set; } = "https://gamepedia.cursecdn.com/mtgsalvation_gamepedia/f/f8/Magic_card_back.jpg?version=0ddc8d41c3b69c2c3c4bb5d72669ffd7";
    public bool CleanDeckNames { get; set; } = true;
    public bool MoveParsedFiles { get; set; } = true;
    public string? ImgurClientKey { get; set; }

    public async Task CreateFile()
    {
        using var fileStream = File.Create(FILENAME);

        await JsonSerializer.SerializeAsync(fileStream, this, ConfigSourceGenerationContext.Default.ParserFileConfig);
    }
}


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ParserFileConfig))]
internal partial class ConfigSourceGenerationContext : JsonSerializerContext
{
}