using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numberOfPlayers;
    public int cardsPerPlayer;
    public GameObject playerPrefab;
    public Transform dealer;
    public GameObject[] pusoyDosPlayers;
    public float dealAnimationSpeed = 1.0f;

    public Deck deck;
    private GameplayManager gameplayManager;

    void Start()
    {
        InitializeComponents();
        //Debug.Log("Player List: " + pusoyDosPlayers.Length);
        StartCoroutine(DealCardsWithAnimation(cardsPerPlayer));
    }

    void InitializeComponents()
    {
        if (pusoyDosPlayers.Length < numberOfPlayers)
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
        //Debug.Log("Number of Players: " + numberOfPlayers);
        float dealDelay = 0.5f / dealAnimationSpeed;

        for (int i = 0; i < cardsToDealPerPlayer; i++)
        {
            for (int j = 0; j < numberOfPlayers; j++)
            {
                Player player = pusoyDosPlayers[j].GetComponent<Player>();                
                Card dealtCard = deck.DealCard();                
                if (dealtCard != null)
                {
                    player.hand.Add(dealtCard);

                    Vector3 startPosition = dealer.position;
                    Vector3 targetPosition = player.transform.position;

                    GameObject cardObj = deck.CreateCardObject(dealtCard, startPosition, dealer);

                    yield return StartCoroutine(MoveCardSmoothly(cardObj.transform, targetPosition, dealDelay));
                    cardObj.transform.SetParent(player.transform);
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
}
