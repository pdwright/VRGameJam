using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Joystick visual component and behavior that responds to player touches
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(GUITexture))]
public class Joystick : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------
    // Boundaries for the joystick touch
    // ----------------------------------------------------------------------------------------------------
    public class Boundary
    {
        private Vector2 m_Min = Vector2.zero;
        private Vector2 m_Max = Vector2.zero;

        public Vector2 Min
        {
            get { return m_Min; }
            set { m_Min = value; }
        }

        public Vector2 Max
        {
            get { return m_Max; }
            set { m_Max = value; }
        }
    }

    private GUITexture m_Gui;
    private Rect m_DefaultRect;
    private Vector2 m_GuiTouchOffset;
    private Boundary m_GuiBoundary;
    private Vector2 m_GuiCenter;
    private int m_LastFingerId = -1;
    
    // Static stuff
    private static Joystick[] m_Joysticks;
    private static bool m_EnumeratedJoysticks = false;
    private static float m_TapTimeDelta = 0.3f;
    private static float m_TapTimeWindow;

    // Inspector variables
    public Vector2 m_DeadZone = Vector2.zero;
    public Vector2 m_Position = Vector2.zero;
    public int m_TapCount;
    public bool m_Normalize = false;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    void Start()
    {
        m_Gui = GetComponent<GUITexture>();

        // Get where the GUI Texture was originally placed
        m_DefaultRect = m_Gui.pixelInset;

        // Get our offset for center instead of corner
        m_GuiTouchOffset.x = m_DefaultRect.width * 0.5f;
        m_GuiTouchOffset.y = m_DefaultRect.height * 0.5f;

        m_GuiBoundary = new Boundary();

        Vector2 min = new Vector2(m_DefaultRect.x - m_GuiTouchOffset.x, m_DefaultRect.y - m_GuiTouchOffset.y);
        Vector2 max = new Vector2(m_DefaultRect.x + m_GuiTouchOffset.x, m_DefaultRect.y + m_GuiTouchOffset.y);
        m_GuiBoundary.Min = min;
        m_GuiBoundary.Max = max;

        m_GuiCenter.x = m_DefaultRect.x + m_GuiTouchOffset.x;
        m_GuiCenter.y = m_DefaultRect.y + m_GuiTouchOffset.y;
    }

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
    void Update()
    {
        if (!m_EnumeratedJoysticks)
        {
            m_Joysticks = FindObjectsOfType(typeof(Joystick)) as Joystick[];
            m_EnumeratedJoysticks = true;
        }

        // Check for double taps
        if (m_TapTimeWindow > 0)
        {
            m_TapTimeWindow -= Time.deltaTime;
        }
        else
        {
            m_TapCount = 0;
        }

        int touchCount = iPhoneInputSim.touchCount;
        if (touchCount == 0)
        {
            Reset();
        }
        else
        {
            for (int i = 0; i < touchCount; i++)
            {
                iPhoneTouchSim touch = iPhoneInputSim.GetTouch(i);

                if (m_Gui.HitTest(touch.position))
                {
                    if (m_LastFingerId == -1 || m_LastFingerId != touch.fingerId)
                    {
                        m_LastFingerId = touch.fingerId;

                        if (m_TapTimeWindow > 0)
                        {
                            m_TapCount++;
                        }
                        else
                        {
                            m_TapCount = 1;
                            m_TapTimeWindow = m_TapTimeDelta;
                        }

                        // Latching the new touch
                        foreach (Joystick joystick in m_Joysticks)
                        {
                            if (joystick != this)
                            {
                                joystick.LatchedFinger(touch.fingerId);
                            }
                        }
                    }
                }

                if (m_LastFingerId == touch.fingerId)
                {
                    Vector2 guiTouchPos = touch.position - m_GuiTouchOffset;

                    Rect pixelInset = m_Gui.pixelInset;
                    pixelInset.x = Mathf.Clamp(guiTouchPos.x, m_GuiBoundary.Min.x, m_GuiBoundary.Max.x);
                    pixelInset.y = Mathf.Clamp(guiTouchPos.y, m_GuiBoundary.Min.y, m_GuiBoundary.Max.y);
                    m_Gui.pixelInset = pixelInset;

                    // Finger just released
                    if (touch.phase == iPhoneTouchPhaseSim.Ended)
                    {
                        Reset();
                    }
                }
            }
        }

        // Get joystick position clamped and deadzoned
        m_Position.x = (m_Gui.pixelInset.x + m_GuiTouchOffset.x - m_GuiCenter.x) / m_GuiTouchOffset.x;
        m_Position.y = (m_Gui.pixelInset.y + m_GuiTouchOffset.y - m_GuiCenter.y) / m_GuiTouchOffset.y;

        float absoluteX = Mathf.Abs(m_Position.x);
        float absoluteY = Mathf.Abs(m_Position.y);

        if (absoluteX < m_DeadZone.x)
        {
            m_Position.x = 0.0f;
        }
        else if (m_Normalize)
        {
            m_Position.x = Mathf.Sign(m_Position.x) * (absoluteX - m_DeadZone.x) / (1.0f - m_DeadZone.x);
        }

        if (absoluteY < m_DeadZone.y)
        {
            m_Position.y = 0.0f;
        }
        else if (m_Normalize)
        {
            m_Position.y = Mathf.Sign(m_Position.y) * (absoluteY - m_DeadZone.y) / (1.0f - m_DeadZone.y);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Removes the lock on a touch
    // ----------------------------------------------------------------------------------------------------
    void LatchedFinger(int _fingerId)
    {
        if (m_LastFingerId == _fingerId)
        {
            Reset();
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Disables the joystick
    // ----------------------------------------------------------------------------------------------------
    public void Disable()
    {
		gameObject.SetActive(false);
        m_EnumeratedJoysticks = false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Resets the joystick state
    // ----------------------------------------------------------------------------------------------------
    void Reset()
    {
        m_Gui.pixelInset = m_DefaultRect;
        m_LastFingerId = -1;
    }
}
