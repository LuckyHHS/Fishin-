using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class JoinFriendButton : MonoBehaviour
{
    //* This literally just opens up steam menu to join someone.

    public void JoinFriend()
    {
        SteamFriends.ActivateGameOverlay("Friends");
    }
}
