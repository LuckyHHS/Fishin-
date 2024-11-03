using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class ServerItem : MonoBehaviour
{
    //* This just holds data in the server list for a server.

    // Publics
    public CSteamID lobbyID;
    public String lobbyName;
    public TextMeshProUGUI lobbynameText;
    public TextMeshProUGUI avaliableSlotsText;


    public void SetLobbyData()
    {
        // Check if th server name is empty.
        if (lobbyName == String.Empty)
        {
            lobbynameText.text = "Empty";
        }
        else
        {
            
            lobbynameText.text = lobbyName;
        }
    }

    public void JoinLobby()
    {
        // Call to join lobby.
        SteamLobbies.instance.JoinLobby(lobbyID);
    }
}
