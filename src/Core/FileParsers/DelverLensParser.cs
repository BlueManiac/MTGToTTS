using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

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
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            //csv.Configuration.Delimiter = "\t";
            csv.Context.RegisterClassMap<Map>();

            var records = csv.GetRecords<CardEntry>();

            foreach (var card in records)
            {
                yield return card;
            }
        }
    }

    public class Map : ClassMap<CardEntry>
    {
        public Map()
        {
            Map(m => m.Name).Name("name", "Name");
            Map(m => m.Quantity).Name("QuantityX", "count").TypeConverter<QuantityConverter>();
            Map(m => m.ScryfallId).Name("Scryfall ID", "scryfall_id");
            Map(m => m.Exclude).Name("section").Optional().TypeConverter<ExcludeConverter>();
        }
    }

    private class QuantityConverter : TypeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return int.Parse(text.Replace("x", ""));
        }
    }

    private class ExcludeConverter : TypeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return text == "maybeboard";
        }
    }
}