using Core.FileParsers;

namespace Core.Models;

public class Deck
{
    public required string Name { get; set; }
    public required string FilePath { get; set; }
    public required IEnumerable<CardEntry> Cards { get; set; }
}