using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Transition instance for the camera, will trigger when use enters/leaves BV
// ----------------------------------------------------------------------------------------------------
public class CameraTransition : Transition
{
    private InstructorCamera m_Camera;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    protected override void Start()
    {
        m_Camera = FindObjectOfType(typeof(InstructorCamera)) as InstructorCamera;

        base.Start();
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Called when user enters the BV
    // ----------------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider _other)
    {
        if (m_Time < Mathf.Epsilon)
        {
            m_Camera.AddCut(this);
        }
        else
        {
            m_Camera.AddTransition(this);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Called when user exits the BV
    // ----------------------------------------------------------------------------------------------------
    void OnTriggerExit(Collider _other)
    {
        if (m_Time < Mathf.Epsilon)
        {
            m_Camera.ReleasePriorityCut(this);
        }
        else
        {
            m_Camera.ReleasePriority(this);
        }
    }
}
