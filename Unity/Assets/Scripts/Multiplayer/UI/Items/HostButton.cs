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
    [SerializeField] private Toggle Private;
    [SerializeField] private Toggle Friends;
    
    // PRIVATES
    private int type = 0;
    
    public void StartHosting()
    {
        // Start host
        SteamLobbies.CreateLobby.Invoke(type, tMP_InputField.text);
    }

    public void OnPrivateToggleChanged(bool newvalue)
    {
        type = 2;
       
        Public.isOn = false;
        Friends.isOn = false;
    }

    public void OnPublicToggleChanged(bool newvalue)
    {
        type = 0;
        Private.isOn = false;
     
        Friends.isOn = false;
    }

    public void OnFriendsToggleChanged(bool newvalue)
    {
        type = 1;
        Private.isOn = false;
        Public.isOn = false;
      
    }
}
