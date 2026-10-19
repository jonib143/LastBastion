using System;
using UnityEngine;

[Serializable]
public class CubicPath
{
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;

    public bool IsValid()
    {
        return p0 != null && p1 != null && p2 != null && p3 != null;
    }

    public Vector3 GetPosition(float t)
    {
        return Bezier.Cubic(p0.position, p1.position, p2.position, p3.position, t);
    }

    public Vector3 GetTangent(float t)
    {
        return Bezier.CubicTangent(p0.position, p1.position, p2.position, p3.position, t);
    }

    // approximates curve length by summing segment distances
    public float GetLength(int segments = 20)
    {
        if (!IsValid()) return 0f;

        float totalLength = 0f;
        Vector3 prev = p0.position;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 current = GetPosition(t);
            totalLength += (current - prev).magnitude;
            prev = current;
        }

        return totalLength;
    }

    public void DrawGizmo(Color color, int segments = 20)
    {
        if (!IsValid()) return;

        Gizmos.color = color;
        Vector3 prev = p0.position;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 current = GetPosition(t);
            Gizmos.DrawLine(prev, current);
            prev = current;
        }
    }
}