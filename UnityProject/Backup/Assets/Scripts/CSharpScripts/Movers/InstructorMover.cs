using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Mover using a set of instructors
// ----------------------------------------------------------------------------------------------------
public class InstructorMover : InstructorProcessor
{
    private Transform m_ThisTransform;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;

        m_Containers[m_Priority].m_Positioner.Init(null);
        m_Containers[m_Priority].m_Orienter.Init(null);
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
        m_Position = GetPosition(m_Containers[m_Priority]);
        m_Orientation = GetOrientation(m_Containers[m_Priority], m_Position);
        m_Banking = GetBanking(m_Containers[m_Priority]);

        m_ThisTransform.position = m_Position;
        m_ThisTransform.rotation = Quaternion.LookRotation(m_Orientation, m_Banking);
	}
}
