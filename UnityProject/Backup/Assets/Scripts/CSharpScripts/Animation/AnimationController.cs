using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Used to apply animation to character depending on its speed parameters
// Not the base practice in the game industry...
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(Animation))]
public class AnimationController : MonoBehaviour
{
    public Animation m_AnimationTarget;
    public float m_MaxForwardSpeed = 6.0f;
    public float m_MaxBackwardSpeed = 3.0f;
    public float m_MaxSidestepSpeed = 4.0f;

    private Transform m_ThisTransform;
    private CharacterController m_Character;
    private bool m_Jumping = false;
    private float m_MinUpwardSpeed = 2.0f;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_ThisTransform = transform;
        m_Character = GetComponent<CharacterController>();

	    m_AnimationTarget.wrapMode = WrapMode.Loop;
        m_AnimationTarget["jump"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["jump-land"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["run-land"].wrapMode = WrapMode.ClampForever;
        m_AnimationTarget["LOSE"].wrapMode = WrapMode.ClampForever;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
	    Vector3 characterVelocity = m_Character.velocity;
        Vector3 horizontalVelocity = characterVelocity;
        horizontalVelocity.y = 0;
        float speed = horizontalVelocity.magnitude;
        float upwardsMotion = Vector3.Dot(m_ThisTransform.up, characterVelocity);

        if (!m_Character.isGrounded && upwardsMotion > m_MinUpwardSpeed)
        {
            m_Jumping = true;
        }

        if (m_AnimationTarget.IsPlaying("run-land") && m_AnimationTarget["run-land"].normalizedTime < 1.0f && speed > 0)
        {
            // Continue running, nothing to do
        }
        else if (m_AnimationTarget.IsPlaying("jump-land"))
        {
            if (m_AnimationTarget["run-land"].normalizedTime >= 1.0f)
            {
                m_AnimationTarget.Play("idle");	
            }
        }
        else if (m_Jumping)
        {
            if (m_Character.isGrounded)
            {
                if (speed > 0)
                {
                    m_AnimationTarget.Play("run-land");
                }
                else
                {
                    m_AnimationTarget.Play("jump-land");
                }

                m_Jumping = false;
            }
            else
            {
                m_AnimationTarget.Play("jump");
            }
        }
        else if (speed > 0.0f)
        {
            float forwardMotion = Vector3.Dot(m_ThisTransform.forward, horizontalVelocity);
            float sidewaysMotion = Vector3.Dot(m_ThisTransform.right, horizontalVelocity);
            float t = 0.0f;

            if (Mathf.Abs(forwardMotion) > Mathf.Abs(sidewaysMotion))
            {
                if (forwardMotion > 0)
                {
                    t = Mathf.Clamp(Mathf.Abs(speed / m_MaxForwardSpeed), 0.0f, m_MaxForwardSpeed);
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
                    t = Mathf.Clamp(Mathf.Abs(speed / m_MaxBackwardSpeed), 0.0f, m_MaxBackwardSpeed);
                    m_AnimationTarget["runback"].speed = Mathf.Lerp(0.25f, 1.0f, t);
                    m_AnimationTarget.CrossFade("runback");
                }
            }
            else
            {
                t = Mathf.Clamp(Mathf.Abs(speed / m_MaxSidestepSpeed ), 0.0f, m_MaxSidestepSpeed);

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
	}

    // ----------------------------------------------------------------------------------------------------
    // Stuff to do on game end
    // ----------------------------------------------------------------------------------------------------
    void OnEndGame()
    {
        this.enabled = false;
    }
}
