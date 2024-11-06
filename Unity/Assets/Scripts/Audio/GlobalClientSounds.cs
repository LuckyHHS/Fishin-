using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalClientSounds : MonoBehaviour
{
    //* This allows for a global sound to be played.
    
    public Action<float, Vector3, int> PlaySound;
    public GameObject soundPrefab;
    public AudioClip[] clipIds;
    public static GlobalClientSounds instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);

        }
        else
        {
            instance = this;
        }
    }
    
    void Start()
    {
        PlaySound += Play;
    }

    public void Play(float Volume, Vector3 Position, int id)
    {
        
        PlaySoundRPC(Volume, Position, id);
    }

    private void PlaySoundRPC(float Volume, Vector3 Position, int id)
    {
    
        GameObject obj = Instantiate(soundPrefab);
        obj.transform.position = Position;
        obj.GetComponent<AudioSource>().volume = Volume;
        obj.GetComponent<AudioSource>().clip = clipIds[id];
        obj.GetComponent<AudioSource>().Play();
        Destroy(obj, 10f);
    }
}
