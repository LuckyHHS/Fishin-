using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject endPoint;  
    public int segmentCount = 10;
    public float sagAmount = 0.45f; 

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        UpdateLinePositions();
    }

    void UpdateLinePositions()
    {
        // Check if start and end points are valid
        if (startPoint == null || endPoint == null)
        {
            lineRenderer.positionCount = 0; // Clear the line if points are missing
            return;
        }

        // Set the exact number of points needed
        lineRenderer.positionCount = segmentCount;

        Vector3 start = startPoint.transform.position;
        Vector3 end = endPoint.transform.position;

        // Calculate the points along the line
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1); // Normalized position along the line
            Vector3 point = Vector3.Lerp(start, end, t);

            // Add sag effect
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
            point.y -= sag;

            lineRenderer.SetPosition(i, point);
        }
        
        // Ensure the line ends precisely at the end point
        lineRenderer.SetPosition(segmentCount - 1, end);
    }
}
