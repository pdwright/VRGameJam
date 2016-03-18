using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Various keyboard utility functions to be used by other modules
// ----------------------------------------------------------------------------------------------------
public static class KeyboardUtility
{
    // 80, 80 is the center of the left joystick
    private static Vector2 m_KeyboardTouchPosition = new Vector2(80, 80);
    private static Vector2 m_LastKeyboardPosition = new Vector2(-1.0f, -1.0f);

    public static Vector2 Position
    {
        get { return m_KeyboardTouchPosition; }
    }

    public static Vector2 LastPosition
    {
        set { m_LastKeyboardPosition = value; }
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if any managed key is touched on the keyboard. A key that is being released must count as a
    // pressed key and will use the iPhoneTouchPhaseSim "Ended"
    // ----------------------------------------------------------------------------------------------------
    public static bool KeyboardTouched()
    {
        bool keyPressed = Input.GetKey("up") || Input.GetKey("down") || Input.GetKey("left") || Input.GetKey("right");
        bool keyBeingReleased = Input.GetKeyUp("up") || Input.GetKeyUp("down") || Input.GetKeyUp("left") || Input.GetKeyUp("right");
        bool spaceKeyTouched = Input.GetKey("space") || Input.GetKeyUp("space");

        return keyPressed || keyBeingReleased || spaceKeyTouched;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if the whole keyboard has just been released. If a key is "Up", must make sure every other
    // key is not pressed
    // ----------------------------------------------------------------------------------------------------
    public static bool KeyboardJustReleased()
    {
        if (Input.GetKeyUp("up"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyUp("down"))
        {
            return !Input.GetKey("up") && !Input.GetKey("left") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyUp("left"))
        {
            return !Input.GetKey("down") && !Input.GetKey("up") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyUp("right"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("space");
        }

        if (Input.GetKeyUp("space"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("right");
        }

        return false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if the whole keyboard has just been pressed. If a key is "Down", must make sure every other
    // key is not pressed
    // ----------------------------------------------------------------------------------------------------
    public static bool KeyboardJustPressed()
    {
        if (Input.GetKeyDown("up"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyDown("down"))
        {
            return !Input.GetKey("up") && !Input.GetKey("left") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyDown("left"))
        {
            return !Input.GetKey("down") && !Input.GetKey("up") && !Input.GetKey("right") && !Input.GetKey("space");
        }

        if (Input.GetKeyDown("right"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("space");
        }

        if (Input.GetKeyDown("space"))
        {
            return !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("right");
        }

        return false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Get the iPhone compatible phase of the keyboard.
    // iPhoneTouchPhaseSim.Began : If keyboard just got touched
    // iPhoneTouchPhaseSim.Moved : If keyboard is currently moving
    // iPhoneTouchPhaseSim.Ended : If keyboard was just released
    // ----------------------------------------------------------------------------------------------------
    public static iPhoneTouchPhaseSim.Phase GetKeyboardPhase()
    {
        if (KeyboardJustReleased())
        {
            return iPhoneTouchPhaseSim.Ended;
        }
        else if (KeyboardJustPressed())
        {
            return iPhoneTouchPhaseSim.Began;
        }
        else
        {
            return iPhoneTouchPhaseSim.Moved;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Check for position reset if nothing is touched and the keyboard was not just released
    // Moves keyboard position in an appropriate way
    // ----------------------------------------------------------------------------------------------------
    public static void UpdateKeyboardTouchPosition()
    {
        // Reset the keyboard position to center of joystick (maybe we could have a cursor later on but
        // we would need another key to hold to "push" on the cursor and it would probably be a pain)
        if (!KeyboardTouched() && !KeyboardJustReleased())
        {
            m_KeyboardTouchPosition = new Vector2(80, 80);
        }

        // Move the cursor and snap it back on the axis if an arrow is released
        if (Input.GetKey("up"))
        {
            m_KeyboardTouchPosition.y = Mathf.Min(m_KeyboardTouchPosition.y + 1, 120);
        }
        else if (Input.GetKeyUp("up"))
        {
            m_KeyboardTouchPosition.y = Mathf.Min(m_KeyboardTouchPosition.y, 80);
        }

        if (Input.GetKey("down"))
        {
            m_KeyboardTouchPosition.y = Mathf.Max(m_KeyboardTouchPosition.y - 1, 40);
        }
        else if (Input.GetKeyUp("down"))
        {
            m_KeyboardTouchPosition.y = Mathf.Max(m_KeyboardTouchPosition.y, 80);
        }

        if (Input.GetKey("right"))
        {
            m_KeyboardTouchPosition.x = Mathf.Min(m_KeyboardTouchPosition.x + 1, 120);
        }
        else if (Input.GetKeyUp("right"))
        {
            m_KeyboardTouchPosition.x = Mathf.Min(m_KeyboardTouchPosition.x, 80);
        }

        if (Input.GetKey("left"))
        {
            m_KeyboardTouchPosition.x = Mathf.Max(m_KeyboardTouchPosition.x - 1, 40);
        }
        else if (Input.GetKeyUp("left"))
        {
            m_KeyboardTouchPosition.x = Mathf.Max(m_KeyboardTouchPosition.x, 80);
        }

        // Spacebar overrides the position to the jump button position
        if (Input.GetKey("space"))
        {
            m_KeyboardTouchPosition = new Vector2(410 + 32, 6 + 32);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Returns a vector representing the difference between last touch's position and current position
    // ----------------------------------------------------------------------------------------------------
    public static Vector2 GetDeltaPosition(Vector2 _newTouchPosition)
    {
        if (m_LastKeyboardPosition[0] != -1.0f && m_LastKeyboardPosition[1] != -1.0f)
        {
            return _newTouchPosition - m_LastKeyboardPosition;
        }

        return new Vector2(0.0f, 0.0f);
    }
}
