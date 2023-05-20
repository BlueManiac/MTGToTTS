using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core;
public class ParserFileConfig
{
    public const string File_NAME = "DeckParser.json";

    public string ResultPath { get; set; } = @"%USERPROFILE%\Documents\My Games\Tabletop Simulator\Saves\Saved Objects\Imported";
    public string ImportPath { get; set; } = "Decks";
    public string BackUrl { get; set; } = "https://loremflickr.com/480/680";
    public string? ImgurClientKey { get; set; }

    public async Task CreateFile()
    {
        if (File.Exists(File_NAME))
            return;

        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        await File.WriteAllTextAsync(File_NAME, json);
    }
}
