using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    //* This just holds the current tool you are using and equip and unequip.

    // PUBLICS
    public static ToolHandler instance { get; set; }
    public Tool tool;

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

    
    public void Equip()
    {

    }

    public void Unequip()
    {

    }
}
