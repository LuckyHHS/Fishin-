using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    //* This is for notifications or erros on main menu.
    
    // PUBLIC
    [Header("Objects")]
    [SerializeField] private GameObject NotificationPrefab;
    public static Action<string> showMessage;
    public static Notification instance;

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
    void Start()
    {
        // Listen for messages
        showMessage += OnShowMessage;

        // Dont destroy
        DontDestroyOnLoad(this);
    }

    public void OnShowMessage(string message)
    {
        // Create the object and change the text of the object.
        GameObject notification = Instantiate(NotificationPrefab);
        notification.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
    }
}
