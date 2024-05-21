using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numberOfPlayers;
    public int cardsPerPlayer;
    public GameObject cardPrefab;
    public GameObject playerPrefab;
    public Transform dealer;
    public GameObject[] playerPositions; // Changed to array of player prefabs
    public float dealAnimationSpeed = 1.0f;

    public Deck deck;
    private GameplayManager gameplayManager;

    void Start()
    {
        InitializeComponents();
        StartCoroutine(DealCardsWithAnimation(cardsPerPlayer));
    }

    void InitializeComponents()
    {
        if (playerPositions.Length < numberOfPlayers)
        {
            Debug.LogError("Insufficient player positions provided.");
            return;
        }

        deck = GetComponent<Deck>();
        if (deck == null)
        {
            Debug.LogError("Deck component not found on the GameManager's GameObject.");
            return;
        }

        gameplayManager = GetComponent<GameplayManager>();
        if (gameplayManager == null)
        {
            Debug.LogError("GameplayManager component not found on the GameManager's GameObject.");
            return;
        }

        deck.GenerateDeck();
        string exportFilePath = Application.dataPath + "/card_list.txt";
        deck.ExportCardList(exportFilePath);
        deck.Shuffle();
    }

    IEnumerator DealCardsWithAnimation(int cardsToDealPerPlayer)
    {
        float dealDelay = 0.5f / dealAnimationSpeed;

        for (int i = 0; i < cardsToDealPerPlayer; i++)
        {
            for (int j = 0; j < numberOfPlayers; j++)
            {
                Player player = playerPositions[j].GetComponent<Player>(); // Get the player component from the prefab
                Card dealtCard = deck.DealCard();
                if (dealtCard != null)
                {
                    player.hand.Add(dealtCard);

                    Vector3 startPosition = dealer.position;
                    Vector3 targetPosition = playerPositions[j].transform.position; // Get transform position from the prefab

                    GameObject cardObj = Instantiate(cardPrefab, startPosition, Quaternion.identity);
                    cardObj.name = dealtCard.cardName;

                    CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
                    if (cardDisplay != null)
                    {
                        cardDisplay.cardName = dealtCard.cardName;
                        cardDisplay.cardValue = dealtCard.cardValue;
                        cardDisplay.DisplayCard();
                    }
                    else
                    {
                        Debug.LogError("Card prefab is missing CardDisplay script.");
                    }

                    StartCoroutine(MoveCardSmoothly(cardObj.transform, targetPosition, dealDelay));
                    StartCoroutine(ParentCardToPlayer(cardObj.transform, playerPositions[j].transform, dealDelay));
                }
                else
                {
                    Debug.LogWarning("Deck ran out of cards while dealing.");
                    break;
                }

                yield return new WaitForSeconds(dealDelay);
            }
        }
        gameplayManager.MovePointerToLowestCardPlayer();
    }


    IEnumerator MoveCardSmoothly(Transform cardTransform, Vector3 targetPosition, float duration)
    {
        Vector3 initialPosition = cardTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cardTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardTransform.position = targetPosition;
    }

    IEnumerator ParentCardToPlayer(Transform cardTransform, Transform playerPosition, float delay)
    {
        yield return new WaitForSeconds(delay);
        cardTransform.SetParent(playerPosition);
    }
}
