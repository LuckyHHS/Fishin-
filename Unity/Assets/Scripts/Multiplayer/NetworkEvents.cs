using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkEvents : NetworkBehaviour
{
    //* This is for network events like leaving.

    // Publics
    public static Action OnHostLeaveEvent;

    public static Action OnClientRemovedEvent;

    void Start()
    {
        // Listen for event.
        OnHostLeaveEvent += OnHostLeave;
     
        OnClientRemovedEvent += OnClientRemoved;

        // Dont Destroy
        DontDestroyOnLoad(this);
    }
    
    public void OnHostLeave()
    {
        if (isServer)
        {
            Debug.Log("[SERVER] : Is server");

            // Call the RPC.
            RpcHostLeft();
        }   
    }

    [ClientRpc]
    public void RpcHostLeft()
    {
        OnHostLeft();
    }

    // Called on ALL computers when the host left.
    public void OnHostLeft()
    {
        Debug.Log("[CLIENT] : Host has left.");
        // Show notification.
        Notification.showMessage.Invoke("The host has stopped hosting the server.");

        // Load the main menu scene.
        CustomNetworkManager.instance.LeaveGame();
    }


    public void OnClientRemoved()
    {
        Debug.Log("[CLIENT] : A client has disconnected.");
    }
}
