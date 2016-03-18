using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Network made of simple linear segments, no interpolation or smoothing
// ----------------------------------------------------------------------------------------------------
public class LinearNetwork : Network
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	protected override void Start()
    {
        base.Start();
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Moves to the end of the network
    // ----------------------------------------------------------------------------------------------------
    void End(ref PositionInformation _positionInformation, bool _reverseDirection)
    {
        if (_reverseDirection)
        {
            MoveToStart(ref _positionInformation);
        }
        else
        {
            MoveToEnd(ref _positionInformation);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Loops to the other side of the network
    // ----------------------------------------------------------------------------------------------------
    void Loop(ref PositionInformation _positionInformation, float _distance, ref bool _reverseDirection)
    {
        _distance -= _reverseDirection ? -_positionInformation.m_TotalDistance : _positionInformation.m_TotalDistance - m_Length;

        if (_reverseDirection)
        {
            MoveToEnd(ref _positionInformation);
        }
        else
        {
            MoveToStart(ref _positionInformation);
        }

        // Continues recursively
        Advance(ref _positionInformation, _distance, Mode.Loop, ref _reverseDirection);
    }

    // ----------------------------------------------------------------------------------------------------
    // Reverses direction on the network
    // ----------------------------------------------------------------------------------------------------
    void Reverse(ref PositionInformation _positionInformation, float _distance, ref bool _reverseDirection)
    {
        _distance -= _reverseDirection ? -_positionInformation.m_TotalDistance : _positionInformation.m_TotalDistance - m_Length;

        if (_reverseDirection)
        {
            MoveToStart(ref _positionInformation);
        }
        else
        {
            MoveToEnd(ref _positionInformation);
        }

        _reverseDirection = !_reverseDirection;

        // Continues recursively
        Advance(ref _positionInformation, _distance, Mode.BackAndForth, ref _reverseDirection);
    }

    // ----------------------------------------------------------------------------------------------------
    // Advances on a network using specified distance, mode and direction
    // ----------------------------------------------------------------------------------------------------
    public override void Advance(ref PositionInformation _positionInformation, float _distance, Network.Mode _mode, ref bool _reverseDirection)
    {
        _positionInformation.m_TotalDistance += _reverseDirection ? -_distance : _distance;

        // Reached the end of the network
        if (_positionInformation.m_TotalDistance < 0.0f || _positionInformation.m_TotalDistance > m_Length)
        {
            switch (_mode)
            {
                case Mode.Forward:
                    End(ref _positionInformation, _reverseDirection);
                    break;
                case Mode.Loop:
                    Loop(ref _positionInformation, _distance, ref _reverseDirection);
                    break;
                case Mode.BackAndForth:
                    Reverse(ref _positionInformation, _distance, ref _reverseDirection);
                    break;
            }
        }
        else
        {
            _positionInformation.m_SegmentDistance += _reverseDirection ? -_distance : _distance;

            while (_positionInformation.m_SegmentDistance < 0.0f || _positionInformation.m_SegmentDistance > m_SegmentLength[_positionInformation.m_Segment])
            {
                _positionInformation.m_SegmentDistance -= _reverseDirection ? -m_SegmentLength[_positionInformation.m_Segment - 1] : m_SegmentLength[_positionInformation.m_Segment];
                _positionInformation.m_Segment += _reverseDirection ? -1 : 1;

                Assertion.Assert(_positionInformation.m_Segment != -1, "Bad segment number");
            }

            _positionInformation.m_SegmentRatio = _positionInformation.m_SegmentDistance / m_SegmentLength[_positionInformation.m_Segment];
            _positionInformation.m_Position = m_Nodes[_positionInformation.m_Segment] + (m_SegmentDirection[_positionInformation.m_Segment] * _positionInformation.m_SegmentDistance);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Projects a position on the network
    // ----------------------------------------------------------------------------------------------------
    public override PositionInformation ProjectOnNetwork(Vector3 _position)
    {
        PositionInformation projection = new PositionInformation();

        float closestSquareDistance = m_MaxSquareDistanceCheck;
        int closestIndex = -1;
        float currentSquareDistance;
        for (int i = 0; i < m_NodeCount; i++)
        {
            currentSquareDistance = Vector3.SqrMagnitude(_position - m_Nodes[i]);
            if (currentSquareDistance < closestSquareDistance)
            {
                closestIndex = i;
                closestSquareDistance = currentSquareDistance;
            }
        }

        if (closestIndex != -1)
        {
            Vector3 node1 = closestIndex > 0 ? m_Nodes[closestIndex - 1] : m_Nodes[closestIndex];
            Vector3 node2 = m_Nodes[closestIndex];
            Vector3 node3 = closestIndex < m_NodeCount - 1 ? m_Nodes[closestIndex + 1] : m_Nodes[closestIndex];
            Vector3 point1 = Math.ProjectPointOnSegment(node1, node2, _position), point2 = Math.ProjectPointOnSegment(node2, node3, _position);

            if (Vector3.SqrMagnitude(_position - point1) < Vector3.SqrMagnitude(_position - point2))
            {
                projection.m_Position = point1;
                projection.m_Segment = closestIndex - 1;
            }
            else
            {
                projection.m_Position = point2;
                projection.m_Segment = closestIndex;
            }

            projection.m_SegmentDistance = (projection.m_Position - m_Nodes[projection.m_Segment]).magnitude;
            projection.m_SegmentRatio = projection.m_SegmentDistance / m_SegmentLength[projection.m_Segment];

            for (int i = 0; i < projection.m_Segment; i++)
            {
                projection.m_TotalDistance += m_SegmentLength[i];
            }

            projection.m_TotalDistance += projection.m_SegmentDistance;
        }

        return projection;
    }
}
