using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CoinUIUpdate : MonoBehaviour, IDataPersistance
{
    //* This is for visible coin values, or just some UI that needs to update based on a value.

    // PUBLICS
    [Header("Objects")]
    [SerializeField] private TextMeshProUGUI DataText;

    [Header("Settings")]
    [SerializeField] private  string Prefix = "";
    [SerializeField] private  string Suffix = "";
    [SerializeField] private  bool Commas = true;

    public static Action updateCoin;

    void Start()
    {
        updateCoin += UpdateText;
    }

    public void UpdateText()
    {
        // Check if we want to update the text.
        if (DataText != null)
        {
            // Get text
            String wantedText = UserData.data.Coins.ToString(Commas ? "n2" : "");
            
            // Set
            DataText.text = Prefix + wantedText + Suffix;
        }
    }

    public void LoadData(GameData data)
    {
        UpdateText();
    }
    public void SaveData(ref GameData data) {}
}
