using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the original parent and position of the card
        originalParent = transform.parent;
        startPosition = transform.position;

        // Check if the parent is an active player
        Player player = originalParent.GetComponent<Player>();
        if (player != null && !player.isActivePlayer)
        {
            // If the parent is not an active player, disable dragging
            eventData.pointerDrag = null;
            return;
        }

        // Detach the card from its parent
        transform.SetParent(originalParent.parent);
        // Ensure the card is on top of other UI elements
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the card with the mouse
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the card's parent and position if dropped outside play area
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("PlayArea"))
        {
            transform.SetParent(originalParent);
            transform.position = startPosition;
        }
        // Ensure the card can interact with raycasts again
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
