using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolHandler : NetworkBehaviour
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


    [Client]
    public void Unequip()
    {
        if (tool == null) { return;}
        Debug.Log("Unequipping");

        // Call the server to destroy the tool.
        ServerToolHandler.instance.DestroyTool(toolGameobject);

        // Reset properties
        tool = null;
        currentToolId = 0;
    }

    [Client]
    public void Equip(int key)
    {
        
        if (tool != null) { return;}
        Debug.Log("Equipping " + (isClient ? "client" : "what"));

        // Call the server to spawn in the tool.
        ServerToolHandler.instance.SpawnTool(inventory[key - 1], toolHolder);

        // Set the properties
        currentToolId = key;
    }

    [Client]
    public void UpdateData(GameObject targetTool)
    {
        // Set up properties.
        toolGameobject = targetTool;
        tool = targetTool.GetComponent<Tool>();

        Debug.Log("Updated data.");
    }

    [Client]
    public void EquipItem(int key)
    {
        //! THIS IS ALWAYS FALSE, AND THE CODE IS RUNNIONG AS CLIENT?
         if (!isLocalPlayer) {Debug.Log("Not local player"); return;};

        // Listen for keycodes, and check if there an tool in that slot.
        if (inventory[key - 1] != null)
        {
            // If there is a tool currently, unequip it.
            if (tool != null && currentToolId != key)
            {
                Unequip();
            }

            // Equip item 1.
            Equip(key);
        }
        else
        {
            Debug.Log("No item in that slot.");
        }
    }
}
