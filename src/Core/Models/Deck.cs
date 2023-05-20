using System.Collections.Generic;
using Core.FileParsers;

namespace Core.Models
{
    public class Deck
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public IEnumerable<CardEntry> Cards { get; set; }
    }
}