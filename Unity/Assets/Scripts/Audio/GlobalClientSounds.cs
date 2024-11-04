using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GlobalClientSounds : NetworkBehaviour
{
    //* This allows for a global sound to be played.
    
    public static Action<float, Vector3, int> PlaySound;
    public GameObject soundPrefab;
    public AudioClip[] clipIds;

    void Start()
    {
        PlaySound += Play;
    }

    public void Play(float Volume, Vector3 Position, int id)
    {
        
        CmdPlaySound(Volume, Position, id);
    }

    [Server]
    private void CmdPlaySound(float Volume, Vector3 Position, int id)
    {
     
        PlaySoundRPC(Volume, Position, id);
    }

    [ClientRpc]
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
