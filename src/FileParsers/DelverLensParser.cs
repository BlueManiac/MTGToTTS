using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

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
                Map(m => m.Name);
                Map(m => m.Quantity).ConvertUsing(row => int.Parse(row.GetField("QuantityX").Replace("x", "")));
                Map(m => m.ScryfallId).ConvertUsing(row => row.GetField("Scryfall ID"));
            }
        }
    }
}