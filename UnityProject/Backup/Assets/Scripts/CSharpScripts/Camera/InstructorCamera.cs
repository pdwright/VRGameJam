using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Camera that uses instructors to update its matrix
// ----------------------------------------------------------------------------------------------------
public class InstructorCamera : InstructorProcessor, IInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // For UnitTesting purposes, should never be used elsewhere
    // ----------------------------------------------------------------------------------------------------
    public void __TestInit() { Start(); }
    public Transform __TestThisTransform { get { return m_ThisTransform; } }
    // ----------------------------------------------------------------------------------------------------

    private Transform m_ThisTransform;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;

        InitContainer(m_Containers[m_Priority]);

        m_Position = GetPosition(m_Containers[m_Priority]);
        m_Position = GetCollidedPosition(m_Containers[m_Priority], m_Position);
        m_ThisTransform.position = m_Position;

        m_Orientation = GetOrientation(m_Containers[m_Priority], m_Position);
        m_Banking = GetBanking(m_Containers[m_Priority]);
        m_ThisTransform.rotation = Quaternion.LookRotation(m_Orientation, m_Banking);
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
    void Update()
    {
        Tick(Time.deltaTime);
    }

    // ----------------------------------------------------------------------------------------------------
    // Update with specified DT
    // ----------------------------------------------------------------------------------------------------
	public void Tick(float _dt)
    {
        // Mandatory for static iPhoneInputSim module to work
        iPhoneInputSim.Tick();

        IncrementTransitionCompletions(_dt);

        m_Position = GetPosition(m_Containers[m_Priority]);
        m_Position = GetCollidedPosition(m_Containers[m_Priority], m_Position);
        ProcessTransitionsPosition();

        m_ThisTransform.position = m_Position;

        m_Orientation = GetOrientation(m_Containers[m_Priority], m_Position);
        m_Banking = GetBanking(m_Containers[m_Priority]);
        ProcessTransitionsOrientation();

        m_ThisTransform.rotation = Quaternion.LookRotation(m_Orientation, m_Banking);
	}
}
