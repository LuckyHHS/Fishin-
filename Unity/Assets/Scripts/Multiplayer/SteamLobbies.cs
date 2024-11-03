using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Edgegap;

public class SteamLobbies : MonoBehaviour
{
    //* This handles the lobby system.

    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    

    public List<CSteamID> serverIds = new();

    // Variables
    public ulong CurrentLobbyID;
    public bool startedHosting = false;
    private const string HostAddressKey = "HostAddress";
    public static SteamLobbies instance;
    public bool tryingJoin = false;
    private String wantedServerName;
    public GameObject LoadingScreenObject;
    public TextMeshProUGUI LoadingScreenText;

    // Events
    public static Action<int, String> CreateLobby;

    // PERSISTING OBJECTS
    public GameObject SwitchingSceneObject;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    

    void Start()
    {
        // See if steam is open.
        if(!SteamManager.Initialized) { return;}

        // Connect callbacks
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        

        // Connect events
        CreateLobby += HostLobby;
    }

    public void HostLobby(int type, string serverName)
    {
        // Check if we are already hosting.
        if (startedHosting) { return;}
        startedHosting = true;
        wantedServerName = serverName;

        // Set hosting object to true.
        LoadingScreenObject.SetActive(true);
        LoadingScreenText.text = "Creating server...";

        // Check if public or private.
        Debug.Log("<color=#00FF00>Starting a " + (type == 0 ? "public" : (type == 1 ? "friends only" : "private")) +" server.</color>");
        if (type == 0)
        {
            // Create a new lobby.
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, CustomNetworkManager.instance.maxConnections);
        }
        else if (type == 1)
        {
            // Create a new lobby.
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, CustomNetworkManager.instance.maxConnections);
        }
        
        
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        if (tryingJoin) { return;}
        tryingJoin = true;

        // Request join.
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    // Functions
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // Check if the lobby was created successfully.
        if (callback.m_eResult != EResult.k_EResultOK) { 
     
            // Set hosting object to false.
            LoadingScreenObject.SetActive(false); 
            startedHosting = false;
            Notification.showMessage.Invoke("Unable to create the server.");
        return;}

        Debug.Log("<color=#FFFF00>[STEAM] : Lobby created successfully.</color>");

        String lobbyName;
        if (wantedServerName == String.Empty || wantedServerName.Length > 22) {
            lobbyName = SteamFriends.GetPersonaName().ToString() + "'s Server";
        } else
        {
            lobbyName = wantedServerName;
        }

        // Set lobby data.
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", lobbyName);

          // Set hosting object to true.
        LoadingScreenObject.SetActive(false);

        // Start hosting.
        CustomNetworkManager.instance.StartHost();
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("<color=#FFFF00>[STEAM] : Request to join a lobby.</color>");

        // Set hosting object to true.
        LoadingScreenObject.SetActive(true);
        LoadingScreenText.text = "Joining server...";
        
        // Join the lobby.
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
            
        // Everyone
        CurrentLobbyID = callback.m_ulSteamIDLobby;

         LoadingScreenObject.SetActive(false);

        // Check if is not client.
        if (NetworkServer.active)
        {
            return;
        }

        // Check if we already connected to a server.
        if (NetworkServer.active) { return;}

        // Set the host address.
        CustomNetworkManager.instance.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), HostAddressKey);

        // Start client.
        CustomNetworkManager.instance.StartClient();
    }

    public void GetLobbiesList()
    {
        // Clear ids/
        if(serverIds.Count > 0)
        {
            serverIds.Clear();
        }

        // Get lobbies 60 amount.
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(50); 
        
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        // Destroy the current servers.
        if(ServerHandler.instance.Servers.Count > 0)
        {
            ServerHandler.instance.DestroyServers();
        }

        // Get list of all lobbies
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            // Add the server and lobby.
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            serverIds.Add(lobbyId);

            // Request lobby data
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }

      

    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        ServerHandler.instance.DisplayServers(serverIds, result);
    }
}

