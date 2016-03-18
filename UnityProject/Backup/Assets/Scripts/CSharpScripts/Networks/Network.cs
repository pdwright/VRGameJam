using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Base class for all networks
// ----------------------------------------------------------------------------------------------------
public abstract class Network : MonoBehaviour
{
    protected Vector3[] m_Nodes;
    protected float[] m_SegmentLength;
    protected Vector3[] m_SegmentDirection;
    protected int m_NodeCount;
    protected const float m_MaxSquareDistanceCheck = 10000.0f;
    protected float m_Length;

    // ----------------------------------------------------------------------------------------------------
    // Cursor movement mode
    // ----------------------------------------------------------------------------------------------------
    public enum Mode
    {
        Forward,
        Loop,
        BackAndForth
    }

    // ----------------------------------------------------------------------------------------------------
    // All necessary stuff for position information
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class PositionInformation
    {
        public Vector3 m_Position = Vector3.zero;
        public int m_Segment = 0;
        public float m_SegmentRatio = 0.0f;
        public float m_TotalDistance = 0.0f;
        public float m_SegmentDistance = 0.0f;
    }

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	protected virtual void Start()
    {
        m_NodeCount = transform.GetChildCount();
        m_Nodes = new Vector3[m_NodeCount];
        m_SegmentLength = new float[m_NodeCount];
        m_SegmentDirection = new Vector3[m_NodeCount];

        if (m_NodeCount > 0)
        {
            m_Nodes[0] = transform.GetChild(0).transform.position;
            for (int i = 1; i < m_NodeCount; i++)
            {
                m_Nodes[i] = transform.GetChild(i).transform.position;
                m_SegmentLength[i - 1] = (m_Nodes[i] - m_Nodes[i - 1]).magnitude;
                m_SegmentDirection[i - 1] = (m_Nodes[i] - m_Nodes[i - 1]).normalized;
                m_Length += m_SegmentLength[i - 1];
            }

            m_SegmentLength[m_NodeCount - 1] = 0.0f;
            m_SegmentDirection[m_NodeCount - 1] = Vector3.zero;
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
    void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Moves position information to the beginning of the network
    // ----------------------------------------------------------------------------------------------------
    public void MoveToStart(ref PositionInformation _positionInformation)
    {
        _positionInformation.m_Position = m_Nodes[0];
        _positionInformation.m_Segment = 0;
        _positionInformation.m_SegmentRatio = 0.0f;
        _positionInformation.m_TotalDistance = 0.0f;
        _positionInformation.m_SegmentDistance = 0.0f;
    }

    // ----------------------------------------------------------------------------------------------------
    // Moves the position information to the end of the network
    // ----------------------------------------------------------------------------------------------------
    public void MoveToEnd(ref PositionInformation _positionInformation)
    {
        _positionInformation.m_Position = m_Nodes[m_NodeCount - 1];
        _positionInformation.m_Segment = m_NodeCount - 2;
        _positionInformation.m_SegmentRatio = 1.0f;
        _positionInformation.m_TotalDistance = m_Length;
        _positionInformation.m_SegmentDistance = m_SegmentLength[_positionInformation.m_Segment];
    }

    // ----------------------------------------------------------------------------------------------------
    // Advances on a network using specified distance, mode and direction
    // ----------------------------------------------------------------------------------------------------
    public abstract void Advance(ref PositionInformation _positionInformation, float _distance, Network.Mode _mode, ref bool _reverseDirection);

    // ----------------------------------------------------------------------------------------------------
    // Projects a position on the network
    // ----------------------------------------------------------------------------------------------------
    public abstract PositionInformation ProjectOnNetwork(Vector3 _position);
}
