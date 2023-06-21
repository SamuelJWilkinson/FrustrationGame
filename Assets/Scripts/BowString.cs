using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowString : MonoBehaviour
{
    [SerializeField] private Transform endpoint1, endpoint2;

    private LineRenderer lr;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void CreateString(Vector3? midPosition) {
        // When we don't care about mid position, we only need two points in linePoints
        // When we are drawing the bow backwards we need three points
        Vector3[] linePoints = new Vector3[midPosition == null ? 2 : 3];
        linePoints[0] = endpoint1.localPosition;
        if (midPosition != null) {
            linePoints[1] = transform.InverseTransformPoint(midPosition.Value);
        }
        // Grap the last point in linePoints
        linePoints[^1] = endpoint2.localPosition;
        lr.positionCount = linePoints.Length;
        lr.SetPositions(linePoints);
    }
    
    private void Start() {
        CreateString(null);
    }
}