using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// The base class for all instructors. An instructor is configurable object giving information on how
// to place a moveable object, this information can be position, orientation, banking, etc.
// ----------------------------------------------------------------------------------------------------
public abstract class Instructor : MonoBehaviour
{
    public Transform m_TargetTransform;

    // ----------------------------------------------------------------------------------------------------
    // Always called before instructor use, sets the reference object
    // ----------------------------------------------------------------------------------------------------
    public virtual void Init(Transform _target)
    {
        if (m_TargetTransform == null)
        {
            m_TargetTransform = _target;
        }
    }
}
