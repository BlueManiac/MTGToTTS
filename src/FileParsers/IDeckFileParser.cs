using System.Collections.Generic;

namespace DeckParser.FileParsers
{
    public interface IDeckFileParser
    {
        bool IsValidFile(string filePath);
        IEnumerable<CardEntry> Parse(string filePath);
    }
}