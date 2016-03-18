using UnityEngine;
using System.Collections;

public static class MouseUtility
{
    private static Vector2 m_LastTouchPosition = new Vector2(-1.0f, -1.0f);

    public static Vector2 LastPosition
    {
        set { m_LastTouchPosition = value; }
    }

    // ----------------------------------------------------------------------------------------------------
    // Get the iPhone compatible phase of the mouse.
    // iPhoneTouchPhaseSim.Began : If mouse just got touched
    // iPhoneTouchPhaseSim.Stationary : If mouse is currently not moving
    // iPhoneTouchPhaseSim.Moved : If mouse is currently moving
    // iPhoneTouchPhaseSim.Ended : If mouse was just released
    // ----------------------------------------------------------------------------------------------------
    public static iPhoneTouchPhaseSim.Phase GetMousePhase()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return iPhoneTouchPhaseSim.Ended;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            return iPhoneTouchPhaseSim.Began;
        }
        else if (m_LastTouchPosition == (Vector2)Input.mousePosition)
        {
            return iPhoneTouchPhaseSim.Stationary;
        }
        else
        {
            return iPhoneTouchPhaseSim.Moved;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns a vector representing the difference between last touch's position and current position
    // ----------------------------------------------------------------------------------------------------
    public static Vector2 GetDeltaPosition(Vector2 _newTouchPosition)
    {
        if (m_LastTouchPosition[0] != -1.0f && m_LastTouchPosition[1] != -1.0f)
        {
            return _newTouchPosition - m_LastTouchPosition;
        }

        return new Vector2(0.0f, 0.0f);
    }
}
