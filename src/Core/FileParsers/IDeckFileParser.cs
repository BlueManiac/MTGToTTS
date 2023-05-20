namespace Core.FileParsers;

public interface IDeckFileParser
{
    bool IsValidFile(string filePath);
    IEnumerable<CardEntry> Parse(string filePath);
}