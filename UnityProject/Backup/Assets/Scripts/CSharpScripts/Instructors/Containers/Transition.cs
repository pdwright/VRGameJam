using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Set of instructors along with parameters to go from another set to this one
// ----------------------------------------------------------------------------------------------------
public class Transition : InstructorContainer, ITransition
{
    // ----------------------------------------------------------------------------------------------------
    // For UnitTesting purposes, should never be used elsewhere
    // ----------------------------------------------------------------------------------------------------
    public void __TestInit() { Start(); }
    public void __TestSetPriority(int _priority) { m_Priority = _priority; }
    public void __TestSetTime(float _time) { m_Time = _time; }
    // ----------------------------------------------------------------------------------------------------

    public float m_Time = 1.4f;
    public int m_Priority = ContainerPriority.Zone1;

    public virtual float Time
    {
        get { return m_Time; }
    }

    public int Priority
    {
        get { return m_Priority; }
    }

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	protected virtual void Start()
    {
        // Can optionally specify positioner/orienter
        if (m_Positioner == null)
        {
            m_Positioner = GetComponent<Positioner>();
        }

        if (m_Orienter == null)
        {
            m_Orienter = GetComponent<Orienter>();
        }

        Assertion.Assert(m_Positioner != null, "Missing Positioner on Transition object.");
        Assertion.Assert(m_Orienter != null, "Missing Orienter on Transition object.");
	}
}
