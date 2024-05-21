using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombinationValidator: MonoBehaviour
{
    public CardRules cardRules; // Reference to the CardRules script

    // Method to validate a single card
    public bool IsValidSingleCard(Card card)
    {
        string rank = GetRank(card.cardName);
        return cardRules != null && cardRules.cardValues.Contains(rank);
    }

    // Method to validate a pair
    public bool IsValidPair(List<Card> cards)
    {
        return HasSameRank(cards, 2);
    }

    // Method to validate three of a kind
    public bool IsValidThreeOfAKind(List<Card> cards)
    {
        return HasSameRank(cards, 3);
    }

    // Method to validate a five-card hand
    public bool IsValidFiveCardHand(List<Card> cards)
    {
        return IsRoyalFlush(cards) || IsStraightFlush(cards) || IsFourOfAKind(cards) || IsFullHouse(cards) ||
               IsFlush(cards) || IsStraight(cards);
    }

    // Method to validate a combination of cards
    public bool IsValidCombination(List<Card> cards)
    {
        // Placeholder, replace with actual logic
        return true;
    }

    // Method to validate a combination of cards against a specific type
    public bool IsValidCombinationOfType(List<Card> cards, string combinationType)
    {
        // Placeholder, replace with actual logic
        return true;
    }

    // Method to check if a combination is higher than another combination
    public bool IsHigherCombination(List<Card> newCombination, List<Card> currentCombination, string combinationType)
    {
        // Placeholder, replace with actual logic
        return true;
    }

    // Method to get the type of combination for a given set of cards
    public string GetCombinationType(List<Card> cards)
    {
        // Placeholder, replace with actual logic
        return "SomeCombinationType";
    }

    #region Private Methods

    private bool HasSameRank(List<Card> cards, int count)
    {
        return cards.Count == count && cards.Select(card => GetRank(card.cardName)).Distinct().Count() == 1;
    }

    private string GetRank(string cardName)
    {
        return cardName.Split(' ')[0];
    }

    private bool IsRoyalFlush(List<Card> cards)
    {
        var suits = cards.Select(card => card.cardName.Split(' ')[2]).Distinct().ToList();
        var ranks = cards.Select(card => GetRank(card.cardName)).OrderBy(rank => cardRules.cardValues.IndexOf(rank)).ToList();
        var royalFlushRanks = new List<string> { "10", "J", "Q", "K", "A" };

        return suits.Count == 1 && ranks.SequenceEqual(royalFlushRanks.OrderBy(rank => cardRules.cardValues.IndexOf(rank)).ToList());
    }

    private bool IsStraightFlush(List<Card> cards)
    {
        return IsFlush(cards) && IsStraight(cards);
    }

    private bool IsFourOfAKind(List<Card> cards)
    {
        var rankGroups = cards.GroupBy(card => GetRank(card.cardName)).ToList();
        return rankGroups.Any(group => group.Count() == 4);
    }

    private bool IsFullHouse(List<Card> cards)
    {
        var rankGroups = cards.GroupBy(card => GetRank(card.cardName)).ToList();
        return rankGroups.Count == 2 && rankGroups.Any(group => group.Count() == 3);
    }

    private bool IsFlush(List<Card> cards)
    {
        return cards.Select(card => card.cardName.Split(' ')[2]).Distinct().Count() == 1;
    }

    private bool IsStraight(List<Card> cards)
    {
        var ranks = cards.Select(card => GetRank(card.cardName)).OrderBy(rank => cardRules.cardValues.IndexOf(rank)).ToList();
        int firstIndex = cardRules.cardValues.IndexOf(ranks[0]);
        return ranks.Select(rank => cardRules.cardValues.IndexOf(rank)).SequenceEqual(Enumerable.Range(firstIndex, 5));
    }

    #endregion
}
