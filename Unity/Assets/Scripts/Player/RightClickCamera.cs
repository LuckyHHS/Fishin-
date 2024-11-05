using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RightClickCamera : MonoBehaviour, AxisState.IInputAxisProvider
{
    //* Prevents right clicks from triggering camera movement;
    
     public float GetAxisValue(int axis)
    {
        // No input unless right mouse is down
        if (!Input.GetMouseButton(1))
            return 0;

        switch (axis)
        {
            case 0: return Input.GetAxis("Mouse X");
            case 1: return Input.GetAxis("Mouse Y");
            default: return 0;
        }
    }

}
