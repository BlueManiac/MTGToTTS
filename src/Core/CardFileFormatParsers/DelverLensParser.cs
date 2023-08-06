using Core.CardFileFormatParsers;
using Core.CardFileFormatParsers.Models;
using Csv;

namespace Core.FileParsers;

public class DelverLensParser : IDeckFileParser
{
    private static readonly string[] _extensions = new[] { string.Empty, ".csv" };

    private readonly ParserConfig _config;

    public DelverLensParser(ParserConfig config)
    {
        _config = config;
    }

    public bool IsValidFile(string filePath)
    {
        var extension = Path.GetExtension(filePath)?.ToLower();

        return _extensions.Contains(extension);
    }

    public IEnumerable<CardEntry> Parse(string filePath)
    {
        var options = new CsvOptions();

        using var stream = File.OpenRead(filePath);
        
        var lines = CsvReader.ReadFromStream(stream, options);

        foreach (var line in lines)
        {
            if (line is null)
                throw new Exception("Invalid line");

            var quantity = GetColumn(line, "QuantityX", "count").Replace("x", "");

            yield return new CardEntry
            {
                Name = GetColumn(line, "Name", "name"),
                Quantity = int.Parse(quantity),
                ScryfallId = GetColumn(line, "Scryfall ID", "scryfall_id"),
                Exclude = line.HasColumn("section") && line["section"] == "maybeboard"
            };
        }

        static string GetColumn(ICsvLine line, params string[] columns)
        {
            foreach (var column in columns)
            {
                if (line.HasColumn(column))
                    return line[column];
            }

            throw new Exception($"Could not read any of the columns {string.Join(", ", columns)} from line {line.Index}.");
        }
    }

    public bool MoveParsedFile(string filePath)
    {
        if (!_config.MoveParsedFiles)
            return false;

        Directory.CreateDirectory(_config.CompletedPath);

        var destFilePath = Path.Combine(_config.CompletedPath, Path.GetFileName(filePath));

        File.Move(filePath, destFilePath, true);

        return true;
    }
}