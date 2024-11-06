using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostButton : MonoBehaviour
{
    //* This button just controls starting the game.
 
    public void StartHosting()
    {
        // Start host
        SceneManager.LoadScene("World");
    }

   
   
}
