using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Controls relative to camera orientation
// ----------------------------------------------------------------------------------------------------
public class CameraRelativeControl : CharacterControl
{
    // Move
    public Joystick m_MoveJoystick;

    // Camera rotation
    public Joystick m_RotateJoystick;
    public Transform m_CameraPivot;
    public Vector2 m_RotationSpeed = new Vector2(50.0f, 25.0f);

    private TargetRelativePositioner m_DefaultPositioner;
    private Transform m_CameraTransform;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_DefaultPositioner = FindObjectOfType(typeof(TargetRelativePositioner)) as TargetRelativePositioner;
        m_CameraTransform = (FindObjectOfType(typeof(InstructorCamera)) as InstructorCamera).transform;
        m_FaceMovementDirection = true;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
        Vector2 camRotation = m_RotateJoystick.m_Position;
        camRotation.x *= m_RotationSpeed.x;
        camRotation.y *= m_RotationSpeed.y;
        camRotation *= Time.deltaTime;

        m_DefaultPositioner.HorizontalAngle += camRotation.x;
        m_DefaultPositioner.VerticalAngle += camRotation.y;
	}

    // ----------------------------------------------------------------------------------------------------
    // Gets the movement to apply to the player
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetMovement(CharacterController _controller, float _forwardSpeed, float _backwardSpeed, float _sidestepSpeed)
    {
        Vector3 movement = m_CameraTransform.TransformDirection(new Vector3(m_MoveJoystick.m_Position.x, 0.0f, m_MoveJoystick.m_Position.y));
        movement.y = 0.0f;
        movement.Normalize();

        Vector2 absJoyPos = new Vector2(Mathf.Abs(m_MoveJoystick.m_Position.x), Mathf.Abs(m_MoveJoystick.m_Position.y));
        movement *= _forwardSpeed * Mathf.Max(absJoyPos.x, absJoyPos.y);

        return movement;
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets whether the player starts a jump
    // ----------------------------------------------------------------------------------------------------
    public override bool GetJump(CharacterController _controller)
    {
        if (_controller.isGrounded)
        {
            if (m_RotateJoystick.m_TapCount == 2)
            {
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Stuff to do on game end
    // ----------------------------------------------------------------------------------------------------
    void OnEndGame()
    {
        m_MoveJoystick.Disable();
        m_RotateJoystick.Disable();
        this.enabled = false;
    }
}
