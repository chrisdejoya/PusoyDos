using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public string cardName;
    public int cardValue;

    // Display the card's name and value
    public void DisplayCard()
    {
        Debug.Log("Card Name: " + cardName);
        Debug.Log("Card Value: " + cardValue);
    }
}
