using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostButton : MonoBehaviour
{
    //* This button just controls hosting.
    
    public void StartHosting()
    {
        // Start host
        SteamLobbies.CreateLobby();
    }
}
