using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button1 : MonoBehaviour, IPointerEnterHandler
{
    //* Place this on buttons that you want to have a click sound.
    // PUBLICS
    [SerializeField] private int ID = 1;
    
    // PRIVATES
    private Button button;

    private void Start()
    {
        // Get component and add listener.
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {ButtonHandler.instance.Play(ID, false);});
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play sound when button is hovered over.
        ButtonHandler.instance.Play(ID, true);
    }
}
