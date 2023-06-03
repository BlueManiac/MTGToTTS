using Core.CardFileFormatParsers.Models;

namespace Core.CardFileFormatParsers;

public interface IDeckFileParser
{
    bool IsValidFile(string filePath);
    IEnumerable<CardEntry> Parse(string filePath);
}