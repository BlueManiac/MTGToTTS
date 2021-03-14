namespace DeckParser.FileParsers
{
    public class CardEntry
    {
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string ScryfallId { get; set; }
        public bool Exclude { get; set; }
    }
}