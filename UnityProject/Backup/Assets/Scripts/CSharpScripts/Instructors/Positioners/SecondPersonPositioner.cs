using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Positioner following a target with an offset, always snapped to the target's orientation
// ----------------------------------------------------------------------------------------------------
public class SecondPersonPositioner : Positioner
{
    public Vector3 m_Offset = new Vector3(0.0f, 5.2f, -6.0f);
    public float m_MinVerticalAngle = -25.0f;
    public float m_MaxVerticalAngle = 40.0f;

    private float m_VerticalAngle = 0.0f;

    public float VerticalAngle
    {
        get { return m_VerticalAngle; }
        set { m_VerticalAngle = Mathf.Clamp(value, m_MinVerticalAngle, m_MaxVerticalAngle); }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns a position relative to the target's orientation
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        Vector3 axis = Vector3.Cross(m_TargetTransform.forward, Vector3.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(m_VerticalAngle, axis);
        Vector3 offset = verticalRotation * m_Offset;

        return m_TargetTransform.TransformPoint(offset);
    }
}
