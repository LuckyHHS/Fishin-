using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerToolHandler : NetworkBehaviour
{
    //* This is the server part of the tool handler.
    public static ServerToolHandler instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            Debug.Log("Instance set up.");
            instance = this;
        }
    }

    [Command]
    public void SpawnTool(GameObject toolGameObject, GameObject transformGameobject)
    {
        Debug.Log("Server called.");
        // Spawn the tool.
        GameObject tool = Instantiate(toolGameObject, transformGameobject.transform);

        // Set properties.
        tool.transform.localPosition = Vector3.zero;
        tool.transform.localRotation = Quaternion.identity;
        
        // Network spawn the tool.
        NetworkServer.Spawn(tool, connectionToClient);

        // Notify client
        TargetUpdateTool(connectionToClient, tool);

        Debug.Log("Server spawned tool.");
    }

    [Command]
    public void DestroyTool(GameObject toolGameObject)
    {
        // Destroy the tool on the server.
        NetworkServer.Destroy(toolGameObject);

        Debug.Log("Server destroyed object.");
    }

    [TargetRpc][Client]
    private void TargetUpdateTool(NetworkConnection target, GameObject tool)
    {
        Debug.Log("Target rpc updating tool data.");
        ToolHandler.instance.UpdateData(tool);
    }
}
