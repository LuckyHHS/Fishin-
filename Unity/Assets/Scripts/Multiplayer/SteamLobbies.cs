using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SteamLobbies : MonoBehaviour
{
    //* This handles the lobby system.

    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    // Variables
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    // Events
    public static Action CreateLobby;
    private bool startedHosting = false;

    void Start()
    {
        // See if steam is open.
        if(!SteamManager.Initialized) { return;}

        // Get manager.
        manager = GetComponent<CustomNetworkManager>();

        // Connect callbacks
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        // Connect events
        CreateLobby += HostLobby;
    }

    public void HostLobby()
    {
        // Check if we are already hosting.
        if (startedHosting) { return;}
        startedHosting = true;

        // Create a new lobby.
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    // Functions
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // Check if the lobby was created successfully.
        if (callback.m_eResult != EResult.k_EResultOK) { return;}

        Debug.Log("Lobby created successfully.");

        // Start hosting.
        manager.StartHost();

        // Set lobby data.
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Server");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join a lobby.");
        // Join the lobby.
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // Everyone
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        // Check if is not client.
        if (NetworkServer.active)
        {
            return;
        }

        // Check if we already connected to a server.
        if (NetworkServer.active) { return;}

        // Set the host address.
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), HostAddressKey);

        // Start client.
        manager.StartClient();
    }
}

