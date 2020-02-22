using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoExtensions
{
    public static void DrawCircle(Vector3 origin, float radius, float segments)
    {
        Vector3 startRotation = Vector3.right * radius; // Where the first point of the circle starts
        Vector3 lastPosition = origin + startRotation;
        float angle = 0;
        while (angle <= 360)
        {
            angle += 360 / segments;
            Vector3 nextPosition = origin + (Quaternion.Euler(0, angle, 0) * startRotation);
            Gizmos.DrawLine(lastPosition, nextPosition);
            //Gizmos.DrawSphere(nextPosition, 1);

            lastPosition = nextPosition;
        }
    }
}
