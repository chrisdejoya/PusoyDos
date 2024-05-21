using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public List<Card> hand = new List<Card>();
    public bool isActivePlayer = false; // Added isActivePlayer flag

    // Constructor
    public Player(string name)
    {
        playerName = name;
    }
}
