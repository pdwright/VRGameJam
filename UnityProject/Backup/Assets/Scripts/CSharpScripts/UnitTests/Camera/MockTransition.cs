using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Fake class for a camera transition
// ----------------------------------------------------------------------------------------------------
public class MockTransition : CameraTransition
{
    public override float Time
    {
        get { return m_Time; }
    }

    // ----------------------------------------------------------------------------------------------------
    // Sets the transition time according to the transition name
    // ----------------------------------------------------------------------------------------------------
    protected override void Start()
    {
        if (name.Contains("T1"))
        {
            m_Time = 1.0f;
        }
        else
        {
            Assertion.Assert(name.Contains("T2"), "Bad name for MockTransition");
            m_Time = 2.0f;
        }

        base.Start();
    }
}
