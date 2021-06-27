using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace DeckParser.FileParsers {
    public class TappedOutParser : IDeckFileParser {
        public bool IsValidFile(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CardEntry> Parse(string filePath) {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {   
                csv.Configuration.Delimiter = ",";
                csv.Configuration.RegisterClassMap<Map>();
                csv.Configuration.ShouldSkipRecord = array => array[0] == "maybe";
                
                var records = csv.GetRecords<CardEntry>();

                foreach (var card in records) {
                    yield return card;
                }
            }
        }

        public class Map : ClassMap<CardEntry>
        {
            public Map()
            {
                Map(m => m.Name);
                Map(m => m.Quantity).ConvertUsing(row => int.Parse(row.GetField("Qty")));
            }
        }
    }
}