using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationCanvas : MonoBehaviour
{
    //* Put this on canvas that show the notifications.

    // PUBLICS
    public TextMeshProUGUI text;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
