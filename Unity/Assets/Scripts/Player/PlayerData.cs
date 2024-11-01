using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerData : NetworkBehaviour
{
    //* This holds players data.
    [SyncVar] 
    public string name;
}
