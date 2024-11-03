using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    //* This the data that is being stored.

    public float Coins;

    public GameData()
    {
        // Setup the base data for a new player.
        Coins = 0;
      
        
    }
}
