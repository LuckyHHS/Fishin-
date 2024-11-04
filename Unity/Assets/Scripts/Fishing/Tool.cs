using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum ToolTypes
{
    Rod,
}

public class Tool : NetworkBehaviour
{
    //* This is a tool class, tools inherit from it and change tool types at the top.
    
    public string toolName;
    public ToolTypes toolType;
    public GameObject toolGameObject;

    public virtual void UseRod(float power)
    { }

    public virtual void Use()
    { }
}
