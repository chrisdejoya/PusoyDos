using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Deck : MonoBehaviour
{
    [SerializeField] public CardRules cardRules; // Reference to the CardRules script
    [SerializeField] private GameObject cardPrefab; // Reference to the Card prefab    
    [SerializeField] public List<Card> cards = new List<Card>(); // Make sure the list is serialized to be visible in the Inspector    

    void Start()
    {
        // Check if CardRules script is assigned
        if (cardRules == null)
        {
            Debug.LogError("CardRules script not assigned to the Deck.");
        }

        // Check if Card prefab is assigned
        if (cardPrefab == null)
        {
            Debug.LogError("Card prefab not assigned to the Deck.");
        }
    }

    // Generate a standard deck of 52 cards
    public void GenerateDeck()
    {
        if (cardRules == null)
        {
            Debug.LogError("CardRules script not assigned to the Deck.");
            return;
        }

        cards.Clear();
        int cardValue = 1; // Initialize card value counter

        // Iterate through suits and ranks to generate cards
        foreach (string suit in cardRules.suitValues)
        {
            foreach (string rank in cardRules.cardValues)
            {
                string cardName = $"{rank} of {suit}";
                Card card = new Card(cardName, cardValue++, suit, rank);
                cards.Add(card);
            }
        }        
    }

    // Shuffle the deck
    public void Shuffle()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cards[i];
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

    // Create a card GameObject and parent it to the dealer
    public GameObject CreateCardObject(Card card, Vector3 startPosition, Transform dealerTransform)
    {
        // Instantiate card prefab at start position and set its parent to dealerTransform
        GameObject cardObj = Instantiate(cardPrefab, startPosition, Quaternion.identity, dealerTransform);
        cardObj.name = card.cardName;

        // Configure CardDisplay component if present
        if (cardObj.TryGetComponent(out CardDisplay cardDisplay))
        {
            cardDisplay.cardName = card.cardName;
            cardDisplay.cardValue = card.cardValue;
            //cardDisplay.DisplayCard();
        }
        else
        {
            Debug.LogError("Card prefab is missing CardDisplay script.");
        }

        return cardObj;
    }

    // Export the list of cards to a text file
    [ContextMenu("Export Card List")]
    public void ExportCardList()
    {
        ExportCardList("Assets/CardList.txt");
    }

    // Export the list of cards to a text file
    public void ExportCardList(string filePath)
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("No cards to export.");
            return;
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Card card in cards)
            {
                writer.WriteLine($"{card.cardName}, {card.cardValue}");
            }
        }

        Debug.Log($"Card list exported successfully to: {filePath}");
    }
}
