using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    //* Destroys after a certain amount of time.

    // PUBLICS
    public float TimeBeforeDestroy = 1f;

    void Start()
    {
        Destroy(gameObject, TimeBeforeDestroy);
    }
}
