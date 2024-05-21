using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject pointer;
    [SerializeField] private float verticalOffset = 1.0f;
    [SerializeField] private GameObject playContainer; // Changed from Transform to GameObject

    public CardRules cardRules;
    private GameManager gameManager;
    private Player activePlayer;
    private CardCombinationValidator cardValidator;
    private List<Card> cardsInPlay = new List<Card>();
    private List<Card> currentCardsToBeat = new List<Card>();
    private string currentCombinationType = "";

    void Start()
    {        
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }

        if (pointer == null)
        {
            Debug.LogError("Pointer object not assigned.");
            return;
        }

        if (playContainer == null)
        {
            Debug.LogError("Play container object not assigned.");
            return;
        }

        cardValidator = new CardCombinationValidator();
        if (cardValidator == null)
        {
            Debug.LogError("Failed to create an instance of CardCombinationValidator.");
            return;
        }

        SetInitialActivePlayer();
    }

    public void MovePointerToLowestCardPlayer()
    {
        if (gameManager == null || gameManager.numberOfPlayers == 0)
        {
            Debug.LogWarning("No players available to find the lowest card.");
            return;
        }

        Player lowestCardPlayer = null;
        int lowestCardValue = int.MaxValue;

        foreach (GameObject playerPosition in gameManager.playerPositions)
        {
            Player player = playerPosition.GetComponent<Player>();
            foreach (Card card in player.hand)
            {
                if (card.cardValue < lowestCardValue)
                {
                    lowestCardValue = card.cardValue;
                    lowestCardPlayer = player;
                }
            }
        }

        if (lowestCardPlayer != null)
        {
            Transform playerPositionTransform = lowestCardPlayer.transform;
            if (playerPositionTransform != null)
            {
                Vector3 playerPosition = playerPositionTransform.position;
                pointer.transform.position = new Vector3(playerPosition.x, playerPosition.y + verticalOffset, playerPosition.z);
                Debug.Log("Pointer moved to the player with the lowest card.");
            }
            else
            {
                Debug.LogWarning("Player position transform not found.");
            }
        }
        else
        {
            Debug.LogWarning("No lowest card player found.");
        }
    }

    private void SetInitialActivePlayer()
    {
        Player lowestCardPlayer = null;
        int lowestCardValue = int.MaxValue;

        foreach (GameObject playerPosition in gameManager.playerPositions)
        {
            Player player = playerPosition.GetComponent<Player>();
            foreach (Card card in player.hand)
            {
                if (card.cardValue < lowestCardValue)
                {
                    lowestCardValue = card.cardValue;
                    lowestCardPlayer = player;
                }
            }
        }

        if (lowestCardPlayer != null)
        {
            activePlayer = lowestCardPlayer;
            MovePointerToPlayer(activePlayer);
            Debug.Log($"{activePlayer.playerName} is the active player with the lowest card.");
        }
        else
        {
            Debug.LogWarning("No lowest card player found.");
        }
    }

    private void MovePointerToPlayer(Player player)
    {
        Transform playerPositionTransform = player.transform;
        if (playerPositionTransform != null)
        {
            Vector3 playerPosition = playerPositionTransform.position;
            pointer.transform.position = new Vector3(playerPosition.x, playerPosition.y + verticalOffset, playerPosition.z);
            Debug.Log($"Pointer moved to {player.playerName}");
        }
        else
        {
            Debug.LogWarning("Player position transform not found.");
        }
    }

    public void PlayCards(List<Card> cardsToPlay)
    {
        if (cardsToPlay.Count < 1 || cardsToPlay.Count > 5)
        {
            Debug.LogWarning("Invalid number of cards to play.");
            return;
        }

        if (IsValidCombination(cardsToPlay))
        {
            currentCardsToBeat.Clear();
            currentCardsToBeat.AddRange(cardsInPlay);

            cardsInPlay.Clear();
            cardsInPlay.AddRange(cardsToPlay);
            currentCombinationType = GetCombinationType(cardsToPlay);

            foreach (Card card in cardsToPlay)
            {
                activePlayer.hand.Remove(card);
            }

            foreach (Card card in cardsToPlay)
            {
                Transform cardTransform = playContainer.transform.Find(card.cardName);
                if (cardTransform != null)
                {
                    cardTransform.SetParent(playContainer.transform);
                }
                else
                {
                    Debug.LogWarning($"Card GameObject not found: {card.cardName}");
                }
            }

            Debug.Log($"Cards played: {string.Join(", ", cardsToPlay.Select(card => card.cardName))}");
            ProceedToNextPlayer();
        }
        else
        {
            Debug.LogWarning("Invalid card combination.");
        }
    }

    private bool IsValidCombination(List<Card> cards)
    {
        cardValidator.cardRules = gameManager.deck.cardRules;

        if (string.IsNullOrEmpty(currentCombinationType))
        {
            return cardValidator.IsValidCombination(cards);
        }
        else
        {
            return cardValidator.IsValidCombinationOfType(cards, currentCombinationType) &&
                   cardValidator.IsHigherCombination(cards, currentCardsToBeat, currentCombinationType);
        }
    }

    private string GetCombinationType(List<Card> cards)
    {
        return cardValidator.GetCombinationType(cards);
    }

    private void ProceedToNextPlayer()
    {
        int currentIndex = System.Array.IndexOf(gameManager.playerPositions, activePlayer.transform);
        int nextIndex = (currentIndex + 1) % gameManager.playerPositions.Length;
        activePlayer = gameManager.playerPositions[nextIndex].GetComponent<Player>();
        MovePointerToPlayer(activePlayer);
        Debug.Log($"{activePlayer.playerName} is now the active player.");
    }
}
