using System.Collections.Generic;
using DeckParser.FileParsers;

namespace DeckParser.Models
{
    public class Deck
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public IEnumerable<CardEntry> Cards { get; set; }
    }
}