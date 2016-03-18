using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Static class for math functions
// ----------------------------------------------------------------------------------------------------
public static class Math
{
    // ----------------------------------------------------------------------------------------------------
    // Gets a sinus equivalent of a Lerp factor
    // ----------------------------------------------------------------------------------------------------
    public static float GetSinEquivalent(float _percent)
    {
        _percent *= Mathf.PI;
        _percent -= Mathf.PI / 2;
        _percent = Mathf.Sin(_percent);
        _percent += 1;
        _percent /= 2;
        return _percent;
    }

    // ----------------------------------------------------------------------------------------------------
    // Projects a point on a segment
    // ----------------------------------------------------------------------------------------------------
    // _p1 is the first point on the segment
    // _p2 is the second point on the segment
    // _point is the position to be projected
    // ----------------------------------------------------------------------------------------------------
    public static Vector3 ProjectPointOnSegment(Vector3 _p1, Vector3 _p2, Vector3 _point)
    {
        // Vector from _p1 to _point
        Vector3 p1ToPoint = _point - _p1;

        // Unit Vector from _p1 to _p2
        Vector3 segmentDirection = (_p2 - _p1).normalized;

        // Intersection point distance from _p1
        float distanceFromP1 = Vector3.Dot(segmentDirection, p1ToPoint);

        // Check to see if the point is on the line, if not then return the end point
        if (distanceFromP1 < 0.0f)
        {
            return _p1;
        }
        else if (distanceFromP1 * distanceFromP1 > (_p2 - _p1).sqrMagnitude)
        {
            return _p2;
        }

        // Get the distance to move from point _p1
        segmentDirection *= distanceFromP1;

        // Move from point _p1 to the nearest point on the segment
        return _p1 + segmentDirection;
    }
}
