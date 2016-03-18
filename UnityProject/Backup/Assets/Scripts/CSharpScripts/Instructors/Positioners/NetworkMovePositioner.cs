using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Moves along a network
// ----------------------------------------------------------------------------------------------------
public class NetworkMovePositioner : Positioner
{
    private NetworkCursor m_Cursor = new NetworkCursor();

    public Network m_Network;
    public float m_Speed;
    public Network.Mode m_Mode = Network.Mode.Forward;
    public bool m_ReverseDirection = false;

    // ----------------------------------------------------------------------------------------------------
    // Inits the cursor
    // ----------------------------------------------------------------------------------------------------
    public override void Init(Transform _target)
    {
        m_Cursor.Init(m_Network, m_ReverseDirection);

        base.Init(_target);
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns a position that moves in a specified mode and direction on a network
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetPosition()
    {
        m_Cursor.MoveFor(m_Speed * Time.deltaTime, m_Mode, ref m_ReverseDirection);
        Debug.DrawLine(m_Cursor.Position, m_Cursor.Position + Vector3.up);
        return m_Cursor.Position;
    }
}
