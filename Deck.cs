using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Deck : MonoBehaviour
{
    [SerializeField] public CardRules cardRules; // Reference to the CardRules script
    [SerializeField] private List<string> cardNames = new List<string>(); // Names of all the cards in the deck
    public List<Card> cards = new List<Card>();

    void Start()
    {
    }

    // Generate a standard deck of 52 cards
    public void GenerateDeck()
    {
        if (cardRules == null)
        {
            Debug.LogError("CardRules script not assigned to the Deck.");
            return;
        }

        int cardValue = 1; // Initialize card value counter

        foreach (string suit in cardRules.suitValues)
        {
            foreach (string card in cardRules.cardValues)
            {
                Debug.Log(cardValue);
                string cardName = card + " of " + suit;
                cards.Add(new Card(cardName, cardValue, suit, card)); // Pass suit and rank to Card constructor
                cardValue++; // Increment card value for next card
            }
        }
    }

    // Shuffle the deck
    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    // Deal a card from the deck
    public Card DealCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty. Cannot deal more cards.");
            return null;
        }

        Card dealtCard = cards[0];
        cards.RemoveAt(0);
        return dealtCard;
    }

    // Export the list of cards to a text file
    [ContextMenu("Export Card List")]
    public void ExportCardList()
    {
        string filePath = "Assets/CardList.txt";
        ExportCardList(filePath);
    }

    // Export the list of cards to a text file
    public void ExportCardList(string filePath)
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("No cards to export.");
            return;
        }

        // Open a file stream for writing
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write each card name to the file
            foreach (Card card in cards)
            {
                writer.WriteLine(card.cardName + ", " + card.cardValue);
            }
        }

        Debug.Log("Card list exported successfully to: " + filePath);
    }

}
