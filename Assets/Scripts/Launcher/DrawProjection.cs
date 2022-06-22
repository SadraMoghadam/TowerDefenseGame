using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProjection : MonoBehaviour
{

    public int numPoints = 50;
    public float timeBetweenPoints = 0.1f;
    public LayerMask CollidableLayers;
    
    private LauncherController launcherController;
    private LineRenderer lineRenderer;
    void Start()
    {
        launcherController = GetComponent<LauncherController>();
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update()
    {
        lineRenderer.positionCount = (int)numPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startingPosition = launcherController.ShotPoint.position;
        Vector3 startingVelocity = launcherController.ShotPoint.up * launcherController.BlastPower;
        for (float t = 0; t < numPoints; t += timeBetweenPoints)
        {
            Vector3 newPoint = startingPosition + t * startingVelocity;
            newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y/2f * t * t;
            points.Add(newPoint);

            if(Physics.OverlapSphere(newPoint, 2, CollidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }

        lineRenderer.SetPositions(points.ToArray());
    }
}