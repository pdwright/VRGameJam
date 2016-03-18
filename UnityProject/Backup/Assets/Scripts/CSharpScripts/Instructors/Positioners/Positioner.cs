using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// This type of instructor informs its parent on how to position itself
// ----------------------------------------------------------------------------------------------------
public abstract class Positioner : Instructor
{
    // ----------------------------------------------------------------------------------------------------
    // Returns the processed position
    // ----------------------------------------------------------------------------------------------------
    public abstract Vector3 GetPosition();
}
