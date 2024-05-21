using System;
using UnityEngine;

[Serializable]

public class Card
{
    public string cardName { get; private set; }
    public int cardValue { get; private set; }
    public string Suit { get; private set; }
    public string Rank { get; private set; }

    // Constructor
    public Card(string name, int value, string suit, string rank)
    {
        cardName = name;
        cardValue = value;
        Suit = suit;
        Rank = rank;
    }

    // Override ToString method for easier debugging and logging
    public override string ToString()
    {
        return $"{cardName} (Value: {cardValue}, Suit: {Suit}, Rank: {Rank})";
    }
}
