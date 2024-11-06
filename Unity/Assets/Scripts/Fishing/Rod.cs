using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class Rod : Tool
{
    //* This is the class for a rod.

    // PUBLICS
    [SerializeField] GameObject bobberPrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject rodTip;
    [SerializeField] GameObject cosmeticTip;
    [SerializeField] float arcHeight = 5f;

    // PRIVATES
    private GameObject bobber;
    private GameObject line;

    public override void UseRod(float power)
    {
        ThrowBobber(power);
        UserData.data.TotalCasts += 1;
    }

    void Start()
    {
        toolGameObject = gameObject;
    }


    public void ThrowBobber(float power)
    {
        // Check if there is a bobber.
        if (bobber)
        {
            Destroy(bobber);
        }
        if (line)
        {
            Destroy(line);
        }

        // Clear spawned objects.
        ToolHandler.instance.spawnedObjects.Clear();

        // Instantiate the bobber at the start point (rod tip position)
        bobber = Instantiate(bobberPrefab, rodTip.transform.position, Quaternion.identity);

        // Set up the line.
        line = Instantiate(linePrefab);
        line.GetComponent<FishingLine>().startPoint = cosmeticTip;
        line.GetComponent<FishingLine>().endPoint = bobber;

        // Add spawned objects.
        ToolHandler.instance.spawnedObjects.Add(bobber);
        ToolHandler.instance.spawnedObjects.Add(line);

        // Get the Rigidbody component of the instantiated bobber
        Rigidbody bobberRigidbody = bobber.GetComponent<Rigidbody>();

        // Calculate the end position based on the power and forward direction of the rodTip
        Vector3 endPoint = rodTip.transform.position + rodTip.transform.right * power;

        // Calculate the initial velocity needed to reach endPoint with the desired arc height
        Vector3 velocity = CalculateLaunchVelocity(rodTip.transform.position, endPoint, arcHeight);

        // Apply the calculated velocity to the bobber's Rigidbody
        bobberRigidbody.velocity = velocity;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 startPoint, Vector3 endPoint, float arcHeight)
    {
        float gravity = Physics.gravity.y;
        
        // Calculate horizontal and vertical distances
        Vector3 direction = endPoint - startPoint;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        float horizontalDistance = horizontalDirection.magnitude;
        float verticalDistance = direction.y;

        // Calculate the launch angle to reach the desired arc height
        float heightDifference = arcHeight - verticalDistance;
        float initialVerticalVelocity = Mathf.Sqrt(-2 * gravity * heightDifference);
        float timeToPeak = initialVerticalVelocity / -gravity;
        float totalFlightTime = 2 * timeToPeak; // Assumes symmetric ascent and descent

        // Calculate the horizontal velocity needed to cover horizontal distance in the total flight time
        float horizontalVelocity = horizontalDistance / totalFlightTime;

        // Combine vertical and horizontal components into a single velocity vector
        Vector3 launchVelocity = horizontalDirection.normalized * horizontalVelocity;
        launchVelocity.y = initialVerticalVelocity;

        return launchVelocity;
    }
}
