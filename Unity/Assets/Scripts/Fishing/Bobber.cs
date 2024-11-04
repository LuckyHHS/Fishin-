using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField] private float floatHeight = 0.5f; // Height above water
    [SerializeField] private float noiseScale = 15f; // Scale for noise
    [SerializeField] private float waveHeight = 0.05f; // Wave height
    [SerializeField] private float waveSpeed = 0.03f; // Speed of wave movement
    [SerializeField] private Vector3 Offset;
    private bool isFloating = false;
    private Transform waterSurface; // Reference to the water surface transform

    void OnTriggerEnter(Collider collision)
    {
        // Check if the bobber has hit the water or another object
        if (collision.gameObject.CompareTag("Water"))
        {
            isFloating = true;
            Destroy(GetComponent<Rigidbody>());
            waterSurface = collision.transform; // Assuming the water surface is the parent of the collider
        }
        else
        {
            Destroy(GetComponent<Rigidbody>());
            transform.position = transform.position + Offset;
        }
       
    }

    void Update()
    {
        if (isFloating && waterSurface)
        {
            // Update bobber position to float with the water
            float waterHeight = GetWaterHeight();
            Vector3 newPosition = new Vector3(transform.position.x, waterHeight + floatHeight, transform.position.z);
            transform.position = newPosition;
        }
    }

    private float GetWaterHeight()
    {
        // Get the position of the bobber to sample the noise at this location
        Vector3 position = transform.position;

        // Calculate noise value based on position
        float xNoise = position.x / noiseScale;
        float zNoise = position.z / noiseScale;

        // Using Perlin noise for smooth gradient noise
        float noiseValue = Mathf.PerlinNoise(xNoise, zNoise);
        
        // Calculate the water height
        float height = noiseValue * waveHeight;

        // Add wave effect based on time and direction
        float waveOffset = Mathf.Sin(Time.time * waveSpeed + (xNoise + zNoise)) * waveHeight;
        height += waveOffset;

        return height;
    }

}
