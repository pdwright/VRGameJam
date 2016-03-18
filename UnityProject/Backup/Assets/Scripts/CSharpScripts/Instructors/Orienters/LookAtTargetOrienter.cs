using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Looks at a specified target
// ----------------------------------------------------------------------------------------------------
public class LookAtTargetOrienter : Orienter
{
    // ----------------------------------------------------------------------------------------------------
    // Returns a vector towards the target
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetOrientation(Vector3 _position)
    {
        Vector3 orientation = m_TargetTransform.position + (Vector3.up * 1.5f) - _position;
        orientation.Normalize();
        return orientation;
    }
}
