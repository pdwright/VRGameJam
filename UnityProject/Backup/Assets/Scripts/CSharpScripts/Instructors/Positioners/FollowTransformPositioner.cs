using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Follows a specified transform's position
// ----------------------------------------------------------------------------------------------------
public class FollowTransformPositioner : Positioner
{
    // ----------------------------------------------------------------------------------------------------
    // Returns the transform's position
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        return m_TargetTransform.position;
    }
}
