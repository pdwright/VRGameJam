using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Fakes an orienter by returning an orientation according to object's name
// ----------------------------------------------------------------------------------------------------
public class MockOrienter : Orienter
{
    private Vector3 m_Orientation;

    // ----------------------------------------------------------------------------------------------------
    // Sets a fake orientation using the name
    // ----------------------------------------------------------------------------------------------------
    public override void Init(Transform _target)
    {
        if (name.EndsWith("C00"))
        {
            m_Orientation = Vector3.forward;
        }
        else if (name.EndsWith("C01"))
        {
            m_Orientation = Vector3.up;
        }
        else if (name.EndsWith("C10"))
        {
            m_Orientation = Vector3.left;
        }
        else
        {
            Assertion.Assert(name.EndsWith("C11"), "Bad name for MockOrienter");
            m_Orientation = Vector3.down;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns the fake orientation
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetOrientation(Vector3 _position)
    {
        return m_Orientation;
    }
}
