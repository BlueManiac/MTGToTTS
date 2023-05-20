using Csv;

namespace Core.FileParsers;

public class DelverLensParser : IDeckFileParser
{
    private static readonly string[] _extensions = new[] { string.Empty, ".csv" };

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
            var quantity = GetColumn(line, "QuantityX", "count")?.Replace("x", "") ?? throw new Exception("Invalid quantity on line " + line.Index);

            yield return new CardEntry
            {
                Name = GetColumn(line, "Name", "name") ?? throw new Exception("Invalid name on line " + line.Index),
                Quantity = int.Parse(quantity),
                ScryfallId = GetColumn(line, "Scryfall ID", "scryfall_id") ?? throw new Exception("Invalid scryfall id on line " + line.Index),
                Exclude = GetColumn(line, "section") == "maybeboard"
            };
        }

        static string? GetColumn(ICsvLine? line, params string[] columns)
        {
            if (line is null)
                return null;

            foreach (var column in columns)
            {
                if (line.HasColumn(column))
                    return line[column];
            }

            return null;
        }
    }
}