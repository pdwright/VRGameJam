using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Cursor moving on a network
// ----------------------------------------------------------------------------------------------------
[System.Serializable]
public class NetworkCursor
{
    private Network m_Network;
    private Network.PositionInformation m_PositionInformation;

    public Vector3 Position
    {
        get { return m_PositionInformation.m_Position; }
    }

    // ----------------------------------------------------------------------------------------------------
    // Starts the cursor at the beginning or the end of the network
    // ----------------------------------------------------------------------------------------------------
    public void Init(Network _network, bool reverseDirection)
    {
        m_Network = _network;
        m_PositionInformation = new Network.PositionInformation();

        if (reverseDirection)
        {
            m_Network.MoveToEnd(ref m_PositionInformation);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Moves for a specified distance, mode and direction
    // ----------------------------------------------------------------------------------------------------
    public void MoveFor(float _distance, Network.Mode _mode, ref bool _reverseDirection)
    {
        m_Network.Advance(ref m_PositionInformation, _distance, _mode, ref _reverseDirection);
    }
}
