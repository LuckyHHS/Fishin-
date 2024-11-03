using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostButton : MonoBehaviour
{
    //* This button just controls hosting.
    
    // PUBLICS
    [SerializeField] private TMP_InputField tMP_InputField;
    [SerializeField] private Toggle Public;
  
    
    // PRIVATES
    private int type = 0;
    
    public void StartHosting()
    {
        // Start host
        SteamLobbies.CreateLobby.Invoke(type, tMP_InputField.text);
    }

   
    public void OnPublicToggleChanged(bool newvalue)
    {
        type = newvalue  ? 0 : 1;
    }

   
}
