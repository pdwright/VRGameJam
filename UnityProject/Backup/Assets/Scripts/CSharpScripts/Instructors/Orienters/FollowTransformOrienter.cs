using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Follows the orientation of a specified transform
// ----------------------------------------------------------------------------------------------------
public class FollowTransformOrienter : Orienter
{
    // ----------------------------------------------------------------------------------------------------
    // Returns the transform's orientation
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetOrientation(Vector3 _position)
    {
        return m_TargetTransform.forward;
    }
}
