using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Controls the player using a ray on the environment
// ----------------------------------------------------------------------------------------------------
public class TapControl : CharacterControl
{
    enum ControlState
    {
        WaitingForFirstTouch,
        WaitingForSecondTouch,
        MovingCharacter,
        WaitingForMovement,
        ZoomingCamera,
        RotatingCamera,
        WaitingForNoFingers
    }

    public float m_MinimumTimeUntilMove = 0.25f;
    public bool m_ZoomEnabled = true;
    public float m_ZoomEpsilon = 25.0f;
    public bool m_RotateEnabled = true;
    public float m_RotateEpsilon = 10.0f;
    public GUITexture m_JumpButton;
    public float m_MinimumDistanceToMove = 1.0f;
    public float m_ZoomRate = 1.0f;
    public TargetRelativePositioner m_TargetRelativePositioner;

    private ControlState m_State = ControlState.WaitingForFirstTouch;
    private int[] m_FingerDown = new int[2];
    private Vector2[] m_FingerDownPosition = new Vector2[2];
    private int[] m_FingerDownFrame = new int[2];
    private float m_FirstTouchTime;
    private Transform m_ThisTransform;
    private Vector3 m_TargetLocation;
    private bool m_Moving = false;
    private float m_RotationTarget;
    private float m_RotationVelocity;
    private Camera m_CameraComponent;

    // ----------------------------------------------------------------------------------------------------
	// Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;
        m_CameraComponent = (FindObjectOfType(typeof(InstructorCamera)) as InstructorCamera).GetComponent<Camera>();
        m_FaceMovementDirection = true;

        ResetControlState();
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
        int touchCount = iPhoneInputSim.touchCount;

        if (touchCount == 0)
        {
            ResetControlState();
        }
        else
        {
            int i;
            iPhoneTouchSim touch;
            iPhoneTouchSim[] touches = iPhoneInputSim.touches;

            // Must fix the bad bool design to prevent getting useless touches here
            iPhoneTouchSim touch0 = iPhoneInputSim.GetTouch(0);
            iPhoneTouchSim touch1 = iPhoneInputSim.GetTouch(1);
            bool gotTouch0 = false;
            bool gotTouch1 = false;

            if (m_State == ControlState.WaitingForFirstTouch)
            {
                for (i = 0; i < touchCount; i++)
                {
                    touch = touches[i];

                    if (touch.phase != iPhoneTouchPhaseSim.Ended || touch.phase != iPhoneTouchPhaseSim.Canceled)
                    {
                        m_State = ControlState.WaitingForSecondTouch;
                        m_FirstTouchTime = Time.time;
                        m_FingerDown[0] = touch.fingerId;
                        m_FingerDownPosition[0] = touch.position;
                        m_FingerDownFrame[0] = Time.frameCount;
                        break;
                    }
                }
            }

            if (m_State == ControlState.WaitingForSecondTouch)
            {
                for (i = 0; i < touchCount; i++)
                {
                    touch = touches[i];

                    if (touch.phase != iPhoneTouchPhaseSim.Canceled)
                    {
                        if (touchCount >= 2 && touch.fingerId != m_FingerDown[0])
                        {
                            m_State = ControlState.WaitingForMovement;
                            m_FingerDown[1] = touch.fingerId;
                            m_FingerDownPosition[1] = touch.position;
                            m_FingerDownFrame[1] = Time.frameCount;
                            break;
                        }
                        // Move
                        else if (touchCount == 1)
                        {
                            if (touch.fingerId == m_FingerDown[0] && (Time.time > m_FirstTouchTime + m_MinimumTimeUntilMove || touch.phase == iPhoneTouchPhaseSim.Ended))
                            {
                                m_State = ControlState.MovingCharacter;
                                break;
                            }
                        }
                    }
                }
            }

            if (m_State == ControlState.WaitingForMovement)
            {
                for (i = 0; i < touchCount; i++)
                {
                    touch = touches[i];

                    if (touch.phase == iPhoneTouchPhaseSim.Began)
                    {
                        if (touch.fingerId == m_FingerDown[0] && m_FingerDownFrame[0] == Time.frameCount)
                        {
                            touch0 = touch;
                            gotTouch0 = true;
                        }
                        else if (touch.fingerId != m_FingerDown[0] && touch.fingerId != m_FingerDown[1])
                        {
                            m_FingerDown[1] = touch.fingerId;
                            touch1 = touch;
                            gotTouch1 = true;
                        }
                    }
                    if (touch.phase == iPhoneTouchPhaseSim.Moved || touch.phase == iPhoneTouchPhaseSim.Stationary || touch.phase == iPhoneTouchPhaseSim.Ended)
                    {
                        if (touch.fingerId == m_FingerDown[0])
                        {
                            touch0 = touch;
                            gotTouch0 = true;
                        }
                        else if (touch.fingerId == m_FingerDown[1])
                        {
                            touch1 = touch;
                            gotTouch1 = true;
                        }
                    }
                }

                if (gotTouch0)
                {
                    if (gotTouch1)
                    {
                        Vector3 originalVector = m_FingerDownPosition[1] - m_FingerDownPosition[0];
                        Vector3 currentVector = touch1.position - touch0.position;
                        Vector3 originalDir = originalVector / originalVector.magnitude;
                        Vector3 currentDir = currentVector / currentVector.magnitude;
                        float rotationCos = Vector2.Dot(originalDir, currentDir);

                        if (rotationCos < 1.0f)
                        {
                            float rotationRad = Mathf.Acos(rotationCos);
                            if (rotationRad > m_RotateEpsilon * Mathf.Deg2Rad)
                            {
                                m_State = ControlState.RotatingCamera;
                            }
                        }

                        if (m_State == ControlState.WaitingForMovement)
                        {
                            float deltaDistance = originalVector.magnitude - currentVector.magnitude;
                            if (Mathf.Abs(deltaDistance) > m_ZoomEpsilon)
                            {
                                m_State = ControlState.ZoomingCamera;
                            }
                        }
                    }
                }
                else
                {
                    m_State = ControlState.WaitingForNoFingers;
                }
            }

            if (m_State == ControlState.RotatingCamera || m_State == ControlState.ZoomingCamera)
            {
                for (i = 0; i < touchCount; i++)
                {
                    touch = touches[i];

                    if (touch.phase == iPhoneTouchPhaseSim.Moved || touch.phase == iPhoneTouchPhaseSim.Stationary || touch.phase == iPhoneTouchPhaseSim.Ended)
                    {
                        if (touch.fingerId == m_FingerDown[0])
                        {
                            touch0 = touch;
                            gotTouch0 = true;
                        }
                        else if (touch.fingerId == m_FingerDown[1])
                        {
                            touch1 = touch;
                            gotTouch1 = true;
                        }
                    }
                }
                if (gotTouch0)
                {
                    if (gotTouch1)
                    {
                        CameraControl(touch0, touch1);
                    }
                }
                else
                {
                    m_State = ControlState.WaitingForNoFingers;
                }
            }
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Update at end of frame
    // ----------------------------------------------------------------------------------------------------
    void LateUpdate()
    {
        m_TargetRelativePositioner.HorizontalAngle = Mathf.SmoothDampAngle(m_TargetRelativePositioner.HorizontalAngle, m_RotationTarget, ref m_RotationVelocity, 0.3f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets the movement to apply to the player
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetMovement(CharacterController _controller, float _forwardSpeed, float _backwardSpeed, float _sidestepSpeed)
    {
        int touchCount = iPhoneInputSim.touchCount;

        if (touchCount >= 1 && m_State == ControlState.MovingCharacter)
        {
            iPhoneTouchSim touch0 = iPhoneInputSim.GetTouch(0);

            // Do not recompute target location if we start a jump or we will head towards the jump button
            if (!GetJump(_controller) && touch0.phase != iPhoneTouchPhaseSim.Began)
            {
                // Move
                RaycastHit hit;
                Ray ray = m_CameraComponent.ScreenPointToRay(new Vector3(touch0.position.x, touch0.position.y));

                Debug.DrawRay(ray.origin, ray.direction * 10.0f, Color.yellow);

                if (Physics.Raycast(ray, out hit))
                {
                    float touchDist = (transform.position - hit.point).magnitude;

                    if (touchDist > m_MinimumDistanceToMove)
                    {
                        m_TargetLocation = hit.point;
                    }

                    m_Moving = true;
                }
            }
        }

        Vector3 movement = Vector3.zero;

        if (m_Moving)
        {
            movement = m_TargetLocation - m_ThisTransform.position;
            movement.y = 0.0f;
            float dist = movement.magnitude;
            if (dist < 1.0f)
            {
                m_Moving = false;
            }
            else
            {
                movement = movement.normalized * _forwardSpeed;
            }
        }

        return movement;
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets whether the player starts a jump
    // ----------------------------------------------------------------------------------------------------
    public override bool GetJump(CharacterController _controller)
    {
        int touchCount = iPhoneInputSim.touchCount;

        if (touchCount >= 1 && m_State == ControlState.MovingCharacter)
        {
            if (_controller.isGrounded)
            {
                // Not currently moving, single tap on the jump button
                if (m_JumpButton.HitTest(iPhoneInputSim.GetTouch(0).position))
                {
                    return true;
                }
                // Movement in progress, second tap on jump button
                else if (touchCount == 2 && m_JumpButton.HitTest(iPhoneInputSim.GetTouch(1).position))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Camera control (zoom/rotate)
    // ----------------------------------------------------------------------------------------------------
    void CameraControl(iPhoneTouchSim _touch0, iPhoneTouchSim _touch1)
    {
        if (m_RotateEnabled && m_State == ControlState.RotatingCamera)
        {
            Vector2 currentVector = _touch1.position - _touch0.position;
            Vector2 currentDir = currentVector / currentVector.magnitude;
            Vector2 lastVector = (_touch1.position - _touch1.deltaPosition) - (_touch0.position - _touch0.deltaPosition);
            Vector2 lastDir = lastVector / lastVector.magnitude;
            float rotationCos = Vector2.Dot(currentDir, lastDir);

            if (rotationCos < 1)
            {
                Vector3 currentVector3 = new Vector3(currentVector.x, currentVector.y);
                Vector3 lastVector3 = new Vector3(lastVector.x, lastVector.y);
                float rotationDirection = Vector3.Cross(currentVector3, lastVector3).normalized.z;
                float rotationRad = Mathf.Acos(rotationCos);
                m_RotationTarget += rotationRad * Mathf.Rad2Deg * rotationDirection;

                if (m_RotationTarget < 0.0f)
                {
                    m_RotationTarget += 360.0f;
                }
                else if (m_RotationTarget >= 360.0f)
                {
                    m_RotationTarget -= 360.0f;
                }
            }
        }
        else if (m_ZoomEnabled && m_State == ControlState.ZoomingCamera)
        {
            float touchDistance = (_touch1.position - _touch0.position).magnitude;
            float lastTouchDistance = ((_touch1.position - _touch1.deltaPosition) - (_touch0.position - _touch0.deltaPosition)).magnitude;
            float deltaPinch = touchDistance - lastTouchDistance;

            m_TargetRelativePositioner.Zoom += deltaPinch * m_ZoomRate * Time.deltaTime;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Resets the state machine
    // ----------------------------------------------------------------------------------------------------
    void ResetControlState()
    {
        m_State = ControlState.WaitingForFirstTouch;
        m_FingerDown[0] = -1;
        m_FingerDown[1] = -1;
    }

    // ----------------------------------------------------------------------------------------------------
    // Stuff to do on game end
    // ----------------------------------------------------------------------------------------------------
    void OnEndGame()
    {
        this.enabled = false;
    }
}
