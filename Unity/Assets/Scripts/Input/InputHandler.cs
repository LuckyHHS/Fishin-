using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    //* This script handles the input.

    // PUBLICS
    public static GameInput input {get; set;}
    
    void Awake() 
    {
        // Create input and enable it.
        input = new GameInput();
        input.Enable();
    }

    public void EnableInput()
    {
        input.Enable();
    }

    public void DisableInput()
    {
        input.Disable();
    }
}
