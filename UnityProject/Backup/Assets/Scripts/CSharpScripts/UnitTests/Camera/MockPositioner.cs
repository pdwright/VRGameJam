using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Fakes a positioner by returning a position according to object's name
// ----------------------------------------------------------------------------------------------------
public class MockPositioner : Positioner
{
    private Vector3 m_Coordinates;

    // ----------------------------------------------------------------------------------------------------
    // Sets a fake position using the name
    // ----------------------------------------------------------------------------------------------------
    public override void Init(Transform _target)
    {
        if (name.EndsWith("C00"))
        {
            m_Coordinates = new Vector3(0.0f, 0.0f);
        }
        else if (name.EndsWith("C01"))
        {
            m_Coordinates = new Vector3(0.0f, 1.0f);
        }
        else if (name.EndsWith("C10"))
        {
            m_Coordinates = new Vector3(1.0f, 0.0f);
        }
        else
        {
            Assertion.Assert(name.EndsWith("C11"), "Bad name for MockPositioner");
            m_Coordinates = new Vector3(1.0f, 1.0f);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns the fake position
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        return m_Coordinates;
    }
}
