using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Core.FileParsers
{
    public class TappedOutParser : IDeckFileParser
    {
        public bool IsValidFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CardEntry> Parse(string filePath)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                ShouldSkipRecord = array => array.Row[0] == "maybe"
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, configuration);

            csv.Context.RegisterClassMap<Map>();

            var records = csv.GetRecords<CardEntry>();

            foreach (var card in records)
            {
                yield return card;
            }
        }

        public class Map : ClassMap<CardEntry>
        {
            public Map()
            {
                Map(m => m.Name);
                Map(m => m.Quantity).Convert(row => int.Parse(row.Row.GetField("Qty")));
            }
        }
    }
}