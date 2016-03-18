using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Emulates the iPhoneInput module on PC
// ----------------------------------------------------------------------------------------------------
public static class iPhoneInputSim
{
    // Used to remember which tap was last (mouse or keyboard) since once button down have been made, there is no way to know
    private static iPhoneTouchSim.FingerId m_LastPcFingerId = iPhoneTouchSim.FingerId.None;
    private static iPhoneTouchSim[] m_Touches = new iPhoneTouchSim[2];
    private static int m_TouchCount = 0;

    // Real iPhone module compatibility
    public static int touchCount
    {
        get { return m_TouchCount; }
    }

    // Real iPhone module compatibility
    public static iPhoneTouchSim[] touches
    {
        get { return m_Touches; }
    }

    // Real iPhone module compatibility
    public static iPhoneTouchSim GetTouch(int _index)
    {
        return touches[_index];
    }

    // ----------------------------------------------------------------------------------------------------
    // Must be called at the beginning of each frame so the touches are updated
    // ----------------------------------------------------------------------------------------------------
    public static void Tick()
    {
        KeyboardUtility.UpdateKeyboardTouchPosition();

        UpdateTouches();
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns an array of iPhoneTouchSim representing all touches that are currently active
    // ----------------------------------------------------------------------------------------------------
    private static void UpdateTouches()
    {
        // Put the touches in an array to have same code behavior as on iPhone
        m_TouchCount = 0;
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            m_Touches[m_TouchCount] = new iPhoneTouchSim(GetDeltaPosition(m_TouchCount, Input.mousePosition), Input.mousePosition, iPhoneTouchSim.FingerId.Mouse, MouseUtility.GetMousePhase());
            MouseUtility.LastPosition = m_Touches[m_TouchCount].position;
            m_TouchCount++;
        }

        // Also move the jump button to the suggested position so it does not interact with other control setups
        if (KeyboardUtility.KeyboardTouched())
        {
            m_Touches[m_TouchCount] = new iPhoneTouchSim(GetDeltaPosition(m_TouchCount, KeyboardUtility.Position), KeyboardUtility.Position, iPhoneTouchSim.FingerId.Arrows, KeyboardUtility.GetKeyboardPhase());
            KeyboardUtility.LastPosition = m_Touches[m_TouchCount].position;
            m_TouchCount++;
        }

        // On the iPhone, taps are ordered
        if (Input.GetMouseButtonDown(0))
        {
            m_LastPcFingerId = iPhoneTouchSim.FingerId.Mouse;
        }
        else if (KeyboardUtility.KeyboardJustPressed())
        {
            m_LastPcFingerId = iPhoneTouchSim.FingerId.Arrows;
        }

        // Swap mouse & keyboard if mouse is last
        if (m_TouchCount == 2 && m_LastPcFingerId == iPhoneTouchSim.FingerId.Mouse)
        {
            iPhoneTouchSim touch = m_Touches[0];
            m_Touches[0] = m_Touches[1];
            m_Touches[1] = touch;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns a vector representing the difference between last touch's position and current position
    // ----------------------------------------------------------------------------------------------------
    private static Vector2 GetDeltaPosition(int _touchIndex, Vector2 _newTouchPosition)
    {
        if (_touchIndex == 0)
        {
            return MouseUtility.GetDeltaPosition(_newTouchPosition);
        }
        else if (_touchIndex == 1)
        {
            return KeyboardUtility.GetDeltaPosition(_newTouchPosition);
        }

        return new Vector2(0.0f, 0.0f);
    }
}
