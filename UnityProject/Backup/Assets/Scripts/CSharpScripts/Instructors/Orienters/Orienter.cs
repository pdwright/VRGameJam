using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// This type of instructor informs its parent on how to orient itself
// ----------------------------------------------------------------------------------------------------
public abstract class Orienter : Instructor
{
    // ----------------------------------------------------------------------------------------------------
    // Returns the processed orientation
    // ----------------------------------------------------------------------------------------------------
    public abstract Vector3 GetOrientation(Vector3 _position);
}
