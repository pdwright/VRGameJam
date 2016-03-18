using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Positioner following a target with an offset, able to turn around it at custom angles
// ----------------------------------------------------------------------------------------------------
public class TargetRelativePositioner : Positioner
{
    public float m_ZoomMin = -3.0f;
    public float m_ZoomMax = 3.0f;
    public Vector3 m_Offset = new Vector3(0.0f, 5.2f, -6.0f);
    public float m_MinVerticalAngle = -25.0f;
    public float m_MaxVerticalAngle = 30.0f;

    private float m_Zoom = 0.0f;
    private float m_VerticalAngle = 0.0f;
    private float m_HorizontalAngle = 0.0f;

    public float VerticalAngle
    {
        get { return m_VerticalAngle; }
        set { m_VerticalAngle = Mathf.Clamp(value, m_MinVerticalAngle, m_MaxVerticalAngle); }
    }

    public float HorizontalAngle
    {
        get { return m_HorizontalAngle; }
        set { m_HorizontalAngle = value; }
    }

    public float Zoom
    {
        get { return m_Zoom; }
        set { m_Zoom = Mathf.Clamp(value, m_ZoomMin, m_ZoomMax); }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns the target's position with an offset, rotation and zoom
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        Quaternion rotation = Quaternion.AngleAxis(m_HorizontalAngle, Vector3.up);
        Vector3 offset = rotation * m_Offset;

        Vector3 towardsTarget = -offset.normalized;

        Vector3 axis = Vector3.Cross(towardsTarget, Vector3.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(m_VerticalAngle, axis);
        offset = verticalRotation * offset;

        return m_TargetTransform.position + offset + (m_Zoom * towardsTarget);
    }
}
