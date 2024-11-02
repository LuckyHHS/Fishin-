using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerLoader : NetworkBehaviour
{
    //* Functions are called when player joins a server.

    [Header("Objects")]
    [SerializeField] private GameObject[] OnLoadObjects;
    [SerializeField] private GameObject[] OnLoadHideObjects;
    
    public void PlayerLoaded()
    {
        if (!isLocalPlayer) return;

        // Load some objects.
        foreach (GameObject obj in OnLoadObjects)
        {
            obj.SetActive(true);
        }
        // Hide some objects.
        foreach (GameObject obj in OnLoadHideObjects)
        {
            obj.SetActive(false);
        }
    }
}
