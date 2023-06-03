using System.Text.Json;

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

        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        await File.WriteAllTextAsync(FILENAME, json);
    }
}
