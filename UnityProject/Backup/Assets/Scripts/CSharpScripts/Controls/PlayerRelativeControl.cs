using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Controls relative to the player orientation
// ----------------------------------------------------------------------------------------------------
public class PlayerRelativeControl : CharacterControl
{
    public Joystick m_MoveJoystick;
    public Joystick m_RotateJoystick;
    public Transform m_CameraPivot;
    public Vector2 m_RotationSpeed = new Vector2(50.0f, 25.0f);

    private Transform m_ThisTransform;
    private SecondPersonPositioner m_DefaultPositioner;
    private Vector3 m_CameraVelocity;
    private float m_InitialCameraDistance;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;
        m_DefaultPositioner = FindObjectOfType(typeof(SecondPersonPositioner)) as SecondPersonPositioner;
        m_InitialCameraDistance = m_DefaultPositioner.m_Offset.z;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {

	}

    // ----------------------------------------------------------------------------------------------------
    // Gets the movement to apply to the player
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetMovement(CharacterController _controller, float _forwardSpeed, float _backwardSpeed, float _sidestepSpeed)
    {
        Vector3 movement = m_ThisTransform.TransformDirection(new Vector3(m_MoveJoystick.m_Position.x, 0.0f, m_MoveJoystick.m_Position.y));
        movement.y = 0.0f;
        movement.Normalize();

        Vector3 cameraTarget = Vector3.zero;
        Vector2 absJoyPos = new Vector2(Mathf.Abs(m_MoveJoystick.m_Position.x), Mathf.Abs(m_MoveJoystick.m_Position.y));

        if (absJoyPos.y > absJoyPos.x)
        {
            if (m_MoveJoystick.m_Position.y > 0)
            {
                movement *= _forwardSpeed * absJoyPos.y;
            }
            else
            {
                movement *= _backwardSpeed * absJoyPos.y;
                cameraTarget.z = m_MoveJoystick.m_Position.y * 0.75f;
            }
        }
        else
        {
            movement *= _sidestepSpeed * absJoyPos.x;
            cameraTarget.x = -m_MoveJoystick.m_Position.x * 0.5f;
        }

        if (_controller.isGrounded)
        {
            Vector2 camRotation = m_RotateJoystick.m_Position;
            camRotation.x *= m_RotationSpeed.x;
            camRotation.y *= m_RotationSpeed.y;
            camRotation *= Time.deltaTime;

            m_ThisTransform.Rotate(0.0f, camRotation.x, 0.0f, Space.World);
            m_DefaultPositioner.VerticalAngle += camRotation.y;
        }
        else
        {
            MainCharacter mainCharacter = GetComponent<MainCharacter>();
            cameraTarget.z = -mainCharacter.m_JumpSpeed * 0.25f;
        }

        Vector3 pos = m_DefaultPositioner.m_Offset;
        pos.x = Mathf.SmoothDamp(pos.x, cameraTarget.x, ref m_CameraVelocity.x, 0.3f);
        pos.z = Mathf.SmoothDamp(pos.z, cameraTarget.z + m_InitialCameraDistance, ref m_CameraVelocity.z, 0.5f);
        m_DefaultPositioner.m_Offset = pos;

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
