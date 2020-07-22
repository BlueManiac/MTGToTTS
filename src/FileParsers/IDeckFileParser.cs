using System.Collections.Generic;

namespace DeckParser.FileParsers
{
    public interface IDeckFileParser
    {
        IEnumerable<CardEntry> Parse(string filePath);
    }
}