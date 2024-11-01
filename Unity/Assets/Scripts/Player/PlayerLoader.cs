using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerLoader : NetworkBehaviour
{
    //* Functions are called when player joins a server.

    [Header("Objects")]
    [SerializeField] private GameObject[] OnLoadObjects;
    
    public void PlayerLoaded()
    {
        if (!isLocalPlayer) return;

        // Load some objects.
        foreach (GameObject obj in OnLoadObjects)
        {
            obj.SetActive(true);
        }
    }
}
