using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System;
using Random = UnityEngine.Random;

public struct CreateCharacterMessage : NetworkMessage
{
    public string name;
}

public class CustomNetworkManager : NetworkManager
{
    //* This script handles players leaving and joining, and just events happening.

    /*
    CLIENT - Means that it is run ONLY on that persons computer
    CLIETS - Means that it runs on all the clients in the game.
    SERVER - Means that it is runs on the server
    */

    // PUBLICS
    [Header("Objects")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Vector3[] SpawnPoints;
    [SerializeField] private GameObject networkEvent;
    public static CustomNetworkManager instance;
    public bool gracefulDisconnect = false;
    

    // PRIVATES
    private bool sceneLoaded;

    void Awake()
    {
        // Set singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(networkEvent);
        }
    }
    
    
    // CONNECTIONS =================



    // Called when the server is started - SERVER
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Server initialization.
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        Debug.Log("<color=#FF0000>[SERVER] : Server started</color>");
    }

    // Called when a new client connects. - CLIENT
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // Create the character create message.
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = SteamFriends.GetPersonaName().ToString(),
        };

        // Send the message to the server
        NetworkClient.Send(characterMessage);
        Debug.Log("<color=#00FF00>[CLIENT] : We have connected.</color>");
    }

    // Called when you disconnect from the server - CLIENT
    public override void OnClientDisconnect()
    {
        Debug.Log("<color=#00FF00>[CLIENT] : Sucessfully disconnected.</color>");
        
        // Check if meant to leave.
        if (!gracefulDisconnect)
        {
            Notification.showMessage.Invoke("The host has stopped hosting the server.");
        }

        // Reset
        gracefulDisconnect = false;

        // Exit to main menu.
        LeaveGame();
        

        // Disconnect.
        base.OnClientDisconnect();
    }
    
    // Called when a player leaves - SERVER.
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("<color=#FF0000>[SERVER] : Player has left.</color>");
    }

    // Called when a player joins - SERVER.
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("<color=#FF0000>[SERVER] : Player has joined.</color>");
    }

    // Called on the server when the server is stopped.
    public override void OnStopServer()
    {
        Debug.Log("<color=#FF0000>[SERVER] : Server has been stopped.</color>");
    }

    // Called on the clients when an error happened - CLIENT
    public override void OnClientError(TransportError error, string reason)
    {
        base.OnClientError(error, reason);
        Debug.LogWarning("<color=#00FF00>[CLIENT ERROR] : " + error.ToString() + " - " + reason + "</color>");
        String message = "Unable to connect to the server.";

        // Listen for error cases.
        switch (error)
        {
            case TransportError.DnsResolve:
                Notification.showMessage.Invoke(message);
                break;
            case TransportError.Refused:
                Notification.showMessage.Invoke(message);
                break;
            case TransportError.Timeout:
                Notification.showMessage.Invoke("Connection timed out.");
                break;
            case TransportError.Congestion:
                Notification.showMessage.Invoke(message);
                break;
            case TransportError.InvalidReceive:
                Notification.showMessage.Invoke("Invalid packet recieved.");
                break;
            case TransportError.InvalidSend:
                Notification.showMessage.Invoke("Invalid data sent.");
                break;
            case TransportError.ConnectionClosed:
                Notification.showMessage.Invoke(message);
                break;
            case TransportError.Unexpected:
                Notification.showMessage.Invoke("Unexpected error: " + reason);
                break;
            default:
                Notification.showMessage.Invoke("Unknown error: " + reason);
                break;
        }
    }

    // Called on the client when transport errors happen _ CLIENT
    public override void OnClientTransportException(Exception exception) { 
        Notification.showMessage.Invoke("Transport Error: " + exception.Message);
    }


    // OTHER FUNCTIONS ================


    // A function to create a new character.
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {

        // Spawn player prefab.
        GameObject gameobject = Instantiate(PlayerPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Length)], Quaternion.identity);

        // Apply data from the message 
        PlayerData player = gameobject.GetComponent<PlayerData>();
        player.name = message.name;

        // Set this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }

    // Leaves the game.
    public void LeaveGame()
    {
        // Load main menu.
        SceneManager.LoadScene("MainMenu");

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Allow for hosting.
        this.gameObject.GetComponent<SteamLobbies>().startedHosting = false;
    }
}

