using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour, IDataPersistance
{
    //* This is the data that is used ingame for stuff.
    public static GameData data;
    public static UserData instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // SAVING AND LOADING
    
    public void LoadData(GameData gameData)
    {
        data = gameData;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData = data;
    }
}
