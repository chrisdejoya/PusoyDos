public class Card
{
    public string cardName;
    public int cardValue;
    public string Suit; // Add Suit property
    public string Rank; // Add Rank property

    // Constructor
    public Card(string name, int value, string suit, string rank)
    {
        cardName = name;
        cardValue = value;
        Suit = suit;
        Rank = rank;
    }
}
