using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Simple mover that follows a transform
// ----------------------------------------------------------------------------------------------------
public class FollowTransform : MonoBehaviour
{
    public Transform m_TargetTransform;
    public bool m_FaceForward;

    private Transform m_ThisTransform;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
        m_ThisTransform.position = m_TargetTransform.position;

        if (m_FaceForward)
        {
            m_ThisTransform.forward = m_TargetTransform.forward;
        }
	}
}
