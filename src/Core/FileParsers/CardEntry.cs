namespace Core.FileParsers;

public class CardEntry
{
    public int Quantity { get; set; }
    public required string Name { get; set; }
    public required string ScryfallId { get; set; }
    public bool Exclude { get; set; }
}