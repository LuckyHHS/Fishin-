using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

public class ToolHandler : MonoBehaviour
{
    //* This just holds the current tool you are using and equip and unequip.

    // PUBLICS
    
    public Tool tool; // Your current tool class, which is off of the toolGameObject.
    public GameObject toolGameobject; // The game object that holds the tool class.
    public GameObject[] inventory; // The objects in your inventory, each one of these gameobjects is a prefab.
    public int currentToolId; // The ID of the current tool.

    public static ToolHandler instance { get; set; }
    public GameObject playerObject;
    public GameObject toolHolder;
    public List<GameObject> spawnedObjects;
    public FishingHandler handler; 

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


  
    public void Unequip()
    {
        if (tool == null) { return;}

        // Call the server to destroy the tool.
        DestroyTool();

        // Reset properties
        tool = null;
        currentToolId = 0;

        // Remove any fishing currently.
        if (handler.waitingForFish != null)
        {
            StopCoroutine(handler.waitingForFish);
            handler.waitingForFish = null;
        }
    }

 
    public void Equip(int key)
    {
        if (tool != null) { return;}

        // Call the server to spawn in the tool.
        SpawnTool(key);

        // Set the properties
        currentToolId = key;
    }



  
    public void EquipItem(int key)
    {
        if (handler.reeling || handler.playingReelAnimation) { return; }

        // Check if intenory item exits.
        if (key > inventory.Length || key < 0) { return; }

        // Listen for keycodes, and check if there an tool in that slot.
        if (inventory[key - 1] != null)
        {
            // If there is a tool currently, unequip it.
            if (tool != null)
            {
                if (key == currentToolId) {
                    // Unequip it.
                    Unequip();
                    return;
                }

                // Unequip it.
                Unequip();

                // Equip item 1.
                Equip(key);
            }
            else
            {
                // Equip item 1.
                Equip(key);
            }   
        }
    }

 

   
    public void SpawnTool(int key)
    {
        GameObject toolGameObject =inventory[key - 1];

        // Spawn the tool.
        GameObject tooled = Instantiate(toolGameObject, toolHolder.transform);

        // Set properties.
        tooled.transform.localPosition = Vector3.zero;
        tooled.transform.localRotation = Quaternion.identity;
        

        // Notify client
        toolGameobject = tooled;
        tool = toolGameobject.GetComponent<Tool>();       
    }

  
    public void DestroyTool()
    {

        // First remove all spawned objects.
        foreach (GameObject networkedObject in spawnedObjects)
        {
            if (networkedObject == null) { continue;}
            Destroy(networkedObject);
        }

        // Clear it.
        spawnedObjects.Clear();
        
        // Destroy the tool on the server.
        Destroy(toolGameobject);
    }

    
}
