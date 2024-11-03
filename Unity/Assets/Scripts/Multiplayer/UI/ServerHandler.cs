using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

// Helper class to store server item information
public class ServerItemInfo
{
    public GameObject ServerGameObject;
    public int PlayerCount;
}

public class ServerHandler : MonoBehaviour
{
    //* This handles the server list and stuff.
    
    // Publics
    public static ServerHandler instance;
    public GameObject ServerPrefab;
    public GameObject ServerContentList;
    public GameObject RefreshIcon;

    public List<GameObject> Servers = new();

    private bool refreshing = false;

    // Functions
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

    public void DestroyServers()
    {
        // Destroy the servers
        foreach (GameObject server in Servers)
        {
            Destroy(server);
        }

        // Clear list
        Servers.Clear();
    }

    public void DisplayServers(List<CSteamID> serverIds, LobbyDataUpdate_t result)
    {
        if (SteamLobbies.instance.startedHosting  || SteamLobbies.instance.tryingJoin) return;
         if (result.m_bSuccess == 0 && refreshing == true) {refreshing = false; Notification.showMessage.Invoke("Unable to receive servers at this time."); return;}

        // Servers
        RefreshIcon.SetActive(false);
        
        // Loop through servers.
        for (int i = 0; i < serverIds.Count; i++)
        {
            // Check if the server exists.
            if (serverIds[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                // Create a new server.
                GameObject serverItem = Instantiate(ServerPrefab);

                // Get server class and set data about it.
                ServerItem serverClass = serverItem.GetComponent<ServerItem>();
                if (serverClass)
                {
                    serverClass.lobbyID = (CSteamID)serverIds[i].m_SteamID;
                    serverClass.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)serverIds[i].m_SteamID, "name");
                    serverClass.avaliableSlotsText.text = SteamMatchmaking.GetNumLobbyMembers((CSteamID)serverIds[i].m_SteamID).ToString() + "/" + CustomNetworkManager.instance.maxConnections;
                    
                    // Cal function that set function and name.
                    serverClass.SetLobbyData();

                    // Move item
                    serverItem.transform.SetParent(ServerContentList.transform);
                    serverItem.transform.localScale = Vector3.one;

                    // Add to list of lobbies
                    Servers.Add(serverItem);
                }
                else
                {
                    // Destroy and continue
                    Destroy(serverItem);
                    continue;
                }
            }
        }

        // Sort
        SortLobbiesByPlayerCount();

        // Allow refreshing again.
        refreshing = false; 
    }

     public void SortLobbiesByPlayerCount()
    {
        
        // Check if has any servers.
        if (ServerContentList.transform.childCount == 0)
        {
            Debug.LogWarning("No server items found to sort.");
            return;
        }

        // List to store each GameObject with its player count
        List<ServerItemInfo> serverItems = new List<ServerItemInfo>();

        // Loop through each child of ServerContentList
        foreach (Transform child in ServerContentList.transform)
        {
            ServerItem serverItem = child.GetComponent<ServerItem>();
            if (serverItem != null)
            {
                int playerCount = SteamMatchmaking.GetNumLobbyMembers(serverItem.lobbyID);
                serverItems.Add(new ServerItemInfo { ServerGameObject = child.gameObject, PlayerCount = playerCount });
            }
        }

        // Sort the list by player count in descending order
        serverItems.Sort((a, b) => b.PlayerCount.CompareTo(a.PlayerCount));

        // Rearrange the GameObjects in ServerContentList based on sorted order
        for (int i = 0; i < serverItems.Count; i++)
        {
            serverItems[i].ServerGameObject.transform.SetSiblingIndex(i);
           
        }
    }

    public void GetListOfServers()
    {
                if (SteamLobbies.instance.startedHosting || SteamLobbies.instance.tryingJoin) return;
        if (refreshing == false) { refreshing = true;}

        // Show icon.
        RefreshIcon.SetActive(true);
        
        // Get all lobbies.
        SteamLobbies.instance.GetLobbiesList();
    }
}
