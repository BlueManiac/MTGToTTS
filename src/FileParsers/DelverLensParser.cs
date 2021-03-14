using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace DeckParser.FileParsers
{
    public class DelverLensParser : IDeckFileParser
    {
        public IEnumerable<CardEntry> Parse(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                //csv.Configuration.Delimiter = "\t";
                csv.Configuration.RegisterClassMap<Map>();

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

        private class QuantityConverter : TypeConverter {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) {
                return int.Parse(text.Replace("x", ""));
            }
        }

        private class ExcludeConverter : TypeConverter {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) {
                return text == "maybeboard";
            }
        }
    }
}