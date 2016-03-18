using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Control of the game's main character
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(CharacterControl))]
public class MainCharacter : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------
    // State machine states
    // ----------------------------------------------------------------------------------------------------
    enum CharacterState
    {
        Movement,
        Jump
    }

    private CharacterState m_State = CharacterState.Movement;
    private CharacterController m_Character;
    private Transform m_ThisTransform;
    private CharacterControl m_Control;
    private Vector3 m_Velocity;

    public float m_JumpSpeed = 16.0f;
    public float m_InAirMultiplier = 0.25f;

    // Animation stuff
    public Animation m_AnimationTarget;
    public float m_ForwardSpeed = 6.0f;
    public float m_BackwardSpeed = 3.0f;
    public float m_SidestepSpeed = 4.0f;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;
        m_Character = GetComponent<CharacterController>();
        m_Control = GetComponent<CharacterControl>();

        m_AnimationTarget.wrapMode = WrapMode.Loop;
        m_AnimationTarget["jump"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["jump-land"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["run-land"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["LOSE"].wrapMode = WrapMode.ClampForever;

        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn)
        {
            m_ThisTransform.position = spawn.transform.position;
        }

        switch (m_State)
        {
            case CharacterState.Movement:
                StateMovementConstructor();
                break;
            case CharacterState.Jump:
                StateJumpConstructor();
                break;
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
		// Mandatory for static iPhoneInputSim module to work
        iPhoneInputSim.Tick();
		
        switch (m_State)
        {
            case CharacterState.Movement:
                StateMovementUpdate();
                StateMovementTransitions();
                break;
            case CharacterState.Jump:
                StateJumpUpdate();
                StateJumpTransitions();
                break;
        }
	}

    // ----------------------------------------------------------------------------------------------------
    // Goes from current state to specified state calling constructor and destructor
    // ----------------------------------------------------------------------------------------------------
    void SwitchState(CharacterState _state)
    {
        switch (m_State)
        {
            case CharacterState.Movement:
                StateMovementDestructor();
                break;
            case CharacterState.Jump:
                StateJumpDestructor();
                break;
        }

        switch (_state)
        {
            case CharacterState.Movement:
                StateMovementConstructor();
                break;
            case CharacterState.Jump:
                StateJumpConstructor();
                break;
        }

        m_State = _state;
    }

    // ----------------------------------------------------------------------------------------------------
    // Makes character face direction it is moving to
    // ----------------------------------------------------------------------------------------------------
    void FaceMovementDirection()
    {
        Vector3 horizontalVelocity = m_Character.velocity;
        horizontalVelocity.y = 0.0f;

        if (horizontalVelocity.magnitude > 0.1f)
        {
            m_ThisTransform.forward = horizontalVelocity.normalized;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Movement
    // ----------------------------------------------------------------------------------------------------
    void StateMovementConstructor()
    {
    }

    void StateMovementUpdate()
    {
        Vector3 movement = m_Control.GetMovement(m_Character, m_ForwardSpeed, m_BackwardSpeed, m_SidestepSpeed);

        float movementSquare = movement.sqrMagnitude;

        if (m_AnimationTarget.IsPlaying("run-land") && m_AnimationTarget["run-land"].normalizedTime < 1.0f && movementSquare > 0)
        {
            // Let the run animation finish
        }
        else if (m_AnimationTarget.IsPlaying("jump-land"))
        {
            if (movementSquare > 0.0f)
            {
                m_AnimationTarget.CrossFade("run");
            }
            else if (m_AnimationTarget["jump-land"].normalizedTime >= 1.0f)
            {
                m_AnimationTarget.Play("idle");
            }
        }
        else if (movementSquare > 0.0f)
        {
            float speed = movement.magnitude;
            float forwardMotion = Vector3.Dot(m_ThisTransform.forward, movement);
            float sidewaysMotion = Vector3.Dot(m_ThisTransform.right, movement);
            float t = 0.0f;

            if (Mathf.Abs(forwardMotion) > Mathf.Abs(sidewaysMotion))
            {
                if (forwardMotion > 0)
                {
                    t = Mathf.Clamp(Mathf.Abs(speed / m_ForwardSpeed), 0.0f, m_ForwardSpeed);
                    m_AnimationTarget["run"].speed = Mathf.Lerp(0.25f, 1.0f, t);

                    if (m_AnimationTarget.IsPlaying("run-land") || m_AnimationTarget.IsPlaying("idle"))
                    {
                        m_AnimationTarget.Play("run");
                    }
                    else
                    {
                        m_AnimationTarget.CrossFade("run");
                    }
                }
                else
                {
                    t = Mathf.Clamp(Mathf.Abs(speed / m_BackwardSpeed), 0.0f, m_BackwardSpeed);
                    m_AnimationTarget["runback"].speed = Mathf.Lerp(0.25f, 1.0f, t);
                    m_AnimationTarget.CrossFade("runback");
                }
            }
            else
            {
                t = Mathf.Clamp(Mathf.Abs(speed / m_SidestepSpeed), 0.0f, m_SidestepSpeed);

                if (sidewaysMotion > 0)
                {
                    m_AnimationTarget["runright"].speed = Mathf.Lerp(0.25f, 1.0f, t);
                    m_AnimationTarget.CrossFade("runright");
                }
                else
                {
                    m_AnimationTarget["runleft"].speed = Mathf.Lerp(0.25f, 1.0f, t);
                    m_AnimationTarget.CrossFade("runleft");
                }
            }
        }
        else
        {
            m_AnimationTarget.CrossFade("idle");
        }

        // Applied after animation computation since it uses horizontal movement without DT
        movement += Physics.gravity;
        movement *= Time.deltaTime;

        m_Character.Move(movement);

        if (m_Control.FaceMovementDirection)
        {
            FaceMovementDirection();
        }
    }

    void StateMovementTransitions()
    {
        if (m_Control.GetJump(m_Character))
        {
            SwitchState(CharacterState.Jump);
        }
    }

    void StateMovementDestructor()
    {
    }

    // ----------------------------------------------------------------------------------------------------
    // Jump
    // ----------------------------------------------------------------------------------------------------
    void StateJumpConstructor()
    {
        m_Velocity = m_Character.velocity;
        m_Velocity.y = m_JumpSpeed;

        m_Character.Move(m_Velocity * Time.deltaTime);

        m_AnimationTarget.Play("jump");
    }

    void StateJumpUpdate()
    {
        Vector3 movement = m_Control.GetMovement(m_Character, m_ForwardSpeed, m_BackwardSpeed, m_SidestepSpeed);

        m_Velocity.y += Physics.gravity.y * Time.deltaTime;
        movement.x *= m_InAirMultiplier;
        movement.z *= m_InAirMultiplier;

        movement += m_Velocity;
        movement += Physics.gravity;
        movement *= Time.deltaTime;

        m_Character.Move(movement);

        if (m_Control.FaceMovementDirection)
        {
            FaceMovementDirection();
        }
    }

    void StateJumpTransitions()
    {
        if (m_Character.isGrounded)
        {
            SwitchState(CharacterState.Movement);
        }
    }

    void StateJumpDestructor()
    {
        Vector3 horizontalVelocity = m_Velocity;
        horizontalVelocity.y = 0.0f;

        if (horizontalVelocity.sqrMagnitude > 0)
        {
            m_AnimationTarget.Play("run-land");
        }
        else
        {
            m_AnimationTarget.Play("jump-land");
        }

        m_Velocity = Vector3.zero;
    }
}
