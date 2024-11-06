using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    //* This the data that is being stored.

    public float Coins;
    public int TotalCasts;
    
    public GameData()
    {
        // Setup the base data for a new player.
        Coins = 0;
        TotalCasts = 0;
        
    }
}
