using UnityEngine;
using System.Collections;

public class iPhoneTouchSim
{
    // ----------------------------------------------------------------------------------------------------
    // Constructor allows keeping member variables private and have only a get in the properties
    // ----------------------------------------------------------------------------------------------------
    public iPhoneTouchSim(Vector2 _deltaPosition, Vector2 _position, iPhoneTouchSim.FingerId _fingerId, iPhoneTouchPhaseSim.Phase _phase)
    {
        m_DeltaPosition = _deltaPosition;
        m_Position = _position;
        m_FingerId = _fingerId;
        m_Phase = _phase;
    }

    // To be used by iPhoneInputSim class only
    public enum FingerId
    {
        None = -1,
        Mouse = 0,
        Arrows = 1
    }

    // Real iPhone module compatibility
    public iPhoneTouchPhaseSim.Phase phase
    {
        get { return m_Phase; }
    }

    // Real iPhone module compatibility
    public Vector2 position
    {
        get { return m_Position; }
    }

    // Real iPhone module compatibility
    public int fingerId
    {
        get { return (int)m_FingerId; }
    }

    // Real iPhone module compatibility
    public int tapCount
    {
        get { return iPhoneInputSim.touchCount; }
    }

    // Real iPhone module compatibility
    public Vector2 deltaPosition
    {
        get { return m_DeltaPosition; }
    }

    private Vector2 m_DeltaPosition = Vector2.zero;
    private Vector2 m_Position = new Vector2(-1.0f, -1.0f);
    private FingerId m_FingerId = FingerId.None;
    private iPhoneTouchPhaseSim.Phase m_Phase = iPhoneTouchPhaseSim.Began;
}
