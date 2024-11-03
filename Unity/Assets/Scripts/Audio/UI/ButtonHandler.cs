using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    //* This just handles button clicks.
    
    // PUBLICS
    [SerializeField] private AudioClip[] HoverSounds;
    [SerializeField] private AudioClip[] ClickSounds;
    
    // PRIVATES
    private AudioSource source;
    public static ButtonHandler instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        // Get source
        source = GetComponent<AudioSource>();
    }

    public void Play(int id, bool hovered)
    {
        // Check which sound to play.
        if (hovered) 
        {
            source.PlayOneShot(HoverSounds[id - 1]);
        }
        else
        {
            source.PlayOneShot(ClickSounds[id - 1]);
        }
    }
}
