using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour
{
    public List<Transform> playerHands; // List of player hands
    public Transform playArea; // The play area where cards are moved
    public Color highlightColor = Color.yellow; // Color to highlight cards
    public float highlightDuration = 0.2f; // Duration for highlight animation
    public AnimationCurve highlightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Curve for highlight animation
    public float moveDuration = 0.5f; // Duration for card movement
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Curve for card movement
    public Transform pointer; // Pointer object to indicate the active player's hand
    public string activePlayerName; // Name of the active player
    public float maxRotationAngle = 90f; // Maximum angle by which to rotate (can be set in the inspector)

    private Dictionary<GameObject, Transform> originalParents; // Dictionary to track original parent of cards
    private Dictionary<GameObject, Coroutine> highlightCoroutines; // Dictionary to manage highlight coroutines
    private Dictionary<GameObject, Color> originalColors; // Dictionary to store original colors of cards
    private Transform currentPlayerHand; // Track the current player's hand
    private bool playAreaWasEmptyAtTurnStart = true; // Track if the play area was empty at the start of the current player's turn

    void Start()
    {
        originalParents = new Dictionary<GameObject, Transform>();
        highlightCoroutines = new Dictionary<GameObject, Coroutine>();
        originalColors = new Dictionary<GameObject, Color>();
    }

    void Update()
    {
        HandleCardHover();
        HandleCardClick();
    }

    private void HandleCardHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool isCardHovered = false;
            GameObject topMostCard = null;

            foreach (RaycastResult result in results)
            {
                GameObject card = result.gameObject;
                if (card.CompareTag("Card"))
                {
                    if (topMostCard == null || result.index > results.FindIndex(r => r.gameObject == topMostCard))
                    {
                        topMostCard = card;
                    }
                }
            }

            if (topMostCard != null)
            {
                HighlightCard(topMostCard, true);
                isCardHovered = true;
            }

            if (!isCardHovered)
            {
                foreach (Transform hand in playerHands)
                {
                    foreach (Transform card in hand)
                    {
                        HighlightCard(card.gameObject, false);
                    }
                }
                foreach (Transform slot in playArea)
                {
                    foreach (Transform card in slot)
                    {
                        HighlightCard(card.gameObject, false);
                    }
                }
            }
        }
    }

    private void HandleCardClick()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            GameObject topMostCard = null;

            foreach (RaycastResult result in results)
            {
                GameObject card = result.gameObject;
                if (card.CompareTag("Card"))
                {
                    if (topMostCard == null || result.index > results.FindIndex(r => r.gameObject == topMostCard))
                    {
                        topMostCard = card;
                    }
                }
            }

            if (topMostCard != null)
            {
                if (originalParents.ContainsKey(topMostCard))
                {
                    MoveCardBackToHand(topMostCard);
                }
                else if (CanMoveToPlayArea(topMostCard))
                {
                    MoveCardToPlayArea(topMostCard);
                }
            }
        }
    }

    private bool CanMoveToPlayArea(GameObject card)
    {
        // Check if play area is full
        int filledSlots = 0;
        foreach (Transform slot in playArea)
        {
            if (slot.childCount > 0)
            {
                filledSlots++;
            }
        }
        if (filledSlots >= 5)
        {
            return false;
        }

        // Check if the current player's hand is set and if the card is from the same hand
        Transform cardParent = card.transform.parent;

        if (playAreaWasEmptyAtTurnStart)
        {
            if (currentPlayerHand == null)
            {
                currentPlayerHand = cardParent;
                MovePointerToHand();
            }
            return cardParent == currentPlayerHand;
        }
        else
        {
            return cardParent == currentPlayerHand;
        }
    }

    private void HighlightCard(GameObject card, bool highlight)
    {
        var renderer = card.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (!originalColors.ContainsKey(card))
            {
                originalColors[card] = renderer.material.color;
            }
                        
            if (highlight)
            {
                if (highlightCoroutines.ContainsKey(card))
                {
                    StopCoroutine(highlightCoroutines[card]);
                }
                highlightCoroutines[card] = StartCoroutine(AnimateHighlight(card, highlightColor));
            }
            else
            {
                if (highlightCoroutines.ContainsKey(card))
                {
                    StopCoroutine(highlightCoroutines[card]);
                }
                highlightCoroutines[card] = StartCoroutine(AnimateHighlight(card, originalColors[card]));
            }
        }
    }

    private IEnumerator AnimateHighlight(GameObject card, Color targetColor)
    {
        var renderer = card.GetComponent<Renderer>();
        if (renderer == null) yield break;

        Color startColor = renderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < highlightDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = highlightCurve.Evaluate(elapsedTime / highlightDuration);
            renderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        renderer.material.color = targetColor;
    }

    private void MoveCardToPlayArea(GameObject card)
    {
        foreach (Transform slot in playArea)
        {
            // Check if the slot is empty
            if (slot.childCount == 0)
            {
                originalParents[card] = card.transform.parent;
                StartCoroutine(AnimateMove(card, slot));
                playAreaWasEmptyAtTurnStart = false; // Play area is no longer empty at turn start
                return;
            }
            else
            {
                // Check if the slot already contains the same card
                if (slot.GetChild(0).gameObject == card)
                {
                    return;
                }
            }
        }
    }

    private void MoveCardBackToHand(GameObject card)
    {
        Transform originalParent = originalParents[card];
        originalParents.Remove(card);
        StartCoroutine(AnimateMove(card, originalParent, true));
        if (playAreaIsEmpty())
        {
            currentPlayerHand = null; // Reset the current player hand when play area is empty
            pointer.position = new Vector3(0, pointer.position.y, pointer.position.z); // Reset pointer position
            playAreaWasEmptyAtTurnStart = true;
            activePlayerName = ""; // Reset the active player name
        }
    }

    private bool playAreaIsEmpty()
    {
        foreach (Transform slot in playArea)
        {
            if (slot.childCount > 0)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator AnimateMove(GameObject card, Transform target, bool reshuffle = false)
    {
        Vector3 startPosition = card.transform.position;
        Quaternion startRotation = card.transform.rotation;
        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        float elapsedTime = 0f;

        // Determine the horizontal distance the card needs to move
        float horizontalDistance = Mathf.Abs(startPosition.x - targetPosition.x);

        // Determine the maximum rotation angle based on the horizontal distance
        float maxRotationAngle = Mathf.Clamp(horizontalDistance, 0f, this.maxRotationAngle);

        // Determine the direction of rotation based on the horizontal movement
        float rotationDirection = Mathf.Sign(startPosition.x - targetPosition.x);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsedTime / moveDuration);
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Calculate the rotation angle based on the progress of the movement
            float rotationAngle = Mathf.Lerp(0f, maxRotationAngle, t);

            // Apply rotation along the z-axis based on the direction of movement
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationDirection * rotationAngle);
            card.transform.rotation = Quaternion.Lerp(startRotation, rotation, t);

            yield return null;
        }

        // Ensure the card's position and rotation are exactly at the target
        card.transform.position = targetPosition;
        card.transform.rotation = targetRotation;

        card.transform.SetParent(target);

        if (reshuffle)
        {
            ReshufflePlayArea();
        }
    }

    private void ReshufflePlayArea()
    {
        List<Transform> cards = new List<Transform>();

        foreach (Transform slot in playArea)
        {
            if (slot.childCount > 0)
            {
                cards.Add(slot.GetChild(0));
            }
        }

        foreach (Transform slot in playArea)
        {
            if (cards.Count == 0) break;

            Transform card = cards[0];
            cards.RemoveAt(0);

            if (slot.childCount == 0)
            {
                card.SetParent(slot);
                StartCoroutine(AnimateMove(card.gameObject, slot));
            }
        }

        if (cards.Count == 0)
        {
            currentPlayerHand = null; // Reset the current player hand when play area is empty
            pointer.position = new Vector3(0, pointer.position.y, pointer.position.z); // Reset pointer position
            playAreaWasEmptyAtTurnStart = true; // Reset the flag
        }
    }

    private void MovePointerToHand()
    {
        if (currentPlayerHand != null)
        {
            pointer.position = new Vector3(currentPlayerHand.position.x, pointer.position.y, pointer.position.z);
            activePlayerName = currentPlayerHand.name; // Set the active player name
        }
    }
}
