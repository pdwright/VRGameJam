using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Projects a target on a network
// ----------------------------------------------------------------------------------------------------
public class NetworkProjectionPositioner : Positioner
{
    public Network m_Network;
    public bool m_SmoothPosition = true;

    private Vector3 m_LastPosition;

    // ----------------------------------------------------------------------------------------------------
    // Initialization of the positioner, always called before instructor use
    // ----------------------------------------------------------------------------------------------------
    public override void Init(Transform _target)
    {
        base.Init(_target);

        m_LastPosition = m_Network.ProjectOnNetwork(m_TargetTransform.position).m_Position;
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns the projected position on the network
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        if (m_SmoothPosition)
        {
            m_LastPosition = Vector3.Lerp(m_LastPosition, m_Network.ProjectOnNetwork(m_TargetTransform.position).m_Position, Time.deltaTime);
            return m_LastPosition;
        }
        else
        {
            return m_Network.ProjectOnNetwork(m_TargetTransform.position).m_Position;
        }
    }
}
