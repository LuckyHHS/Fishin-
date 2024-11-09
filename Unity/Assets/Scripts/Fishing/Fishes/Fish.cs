using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Fish")]
public class Fish : ScriptableObject
{
    [Tooltip("The literal gameobject that looks like the fish that you pull out of the water.")] public GameObject FishPrefab;
    [Tooltip("The max distance it can move when moving.")] public float MaxMovePosition = 75f;
    [Tooltip("How fast it moves when it's trying to get to a new spot.")] public float MoveSpeed = 1.5f;
    [Tooltip("How often it moves. Related to 0.51, higher is more chances, 0.5f is like none.")] public float MoveTime = 0.51f;
    [Tooltip("If you set to 0.5, you will reel in 50% slower")] public float DecreaseTime = 1f;
    [Tooltip("If you set to 1.5, you will reel in 50% faster")] public float IncreaseTime = 1f;
    [Tooltip("The fishes name.")]public String Name = "EmptyFish";
    [Tooltip("The average weight of the fish.")] public float averageWeight = 2.0f;
    [Tooltip("The max average weight of the fish.")] public float maxAverageWeight = 6.0f;
}
