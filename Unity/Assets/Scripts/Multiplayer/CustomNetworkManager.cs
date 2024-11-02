using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public struct CreateCharacterMessage : NetworkMessage
{
    public string name;
}

public class CustomNetworkManager : NetworkManager
{
    //* This script handles players leaving and joining, and just events happening.

    // PUBLICS
    [Header("Objects")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Vector3[] SpawnPoints;
    [SerializeField] private GameObject networkEvent;
    public static CustomNetworkManager instance;
    

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

    // Called when the server is started - HOST
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Server initialization.
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        Debug.Log("Server started");
    }

    // Called when a new player joins the server. - SERVER
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log("Added player to the network");
    }

    // A function to create a new character.
    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        // Wait for scene load.
        StartCoroutine(WaitForSceneToLoad(conn, message));
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
         Debug.Log("[CLIENT] : A new client has connected.");
    }

    // Wait for scene load.
    private IEnumerator WaitForSceneToLoad(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        // Wait until the specified scene is loaded
        while (SceneManager.GetActiveScene().name != "World")
        {
            // Wait for the next frame
            yield return new WaitForEndOfFrame(); 
        }

        // Scene is now loaded
        Debug.Log("World scene is loaded!");

        // Spawn player prefab.
        GameObject gameobject = Instantiate(PlayerPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Length)], Quaternion.identity);

        // Apply data from the message 
        PlayerData player = gameobject.GetComponent<PlayerData>();
        player.name = message.name;

        // Set this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);

        Debug.Log("Created player character.");
    }

    // Called when a client leaves - CLIENT
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        NetworkEvents.OnClientRemovedEvent.Invoke();
    }

    // Called when the server is stopped 
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("[SERVER] : Server has stopped");

        // Call that the host left on the server.
        NetworkEvents.OnHostLeaveEvent.Invoke();
    }

    public void LeaveGame()
    {
        // Load main menu
        SceneManager.LoadScene("MainMenu");

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Allow for hosting.
        this.gameObject.GetComponent<SteamLobbies>().startedHosting = false;
    }
}

