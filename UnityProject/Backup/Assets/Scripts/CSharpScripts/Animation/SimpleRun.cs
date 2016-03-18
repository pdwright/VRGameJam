using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Used to apply animation to character depending on its speed parameters
// Not the base practice in the game industry...
// ----------------------------------------------------------------------------------------------------
[RequireComponent(typeof(Animation))]
public class SimpleRun : MonoBehaviour
{
    public Animation m_AnimationTarget;
    public float m_MaxForwardSpeed = 6.0f;

    private Transform m_ThisTransform;
    private Vector3 m_LastPosition;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    void Start()
    {
        m_ThisTransform = transform;
        m_LastPosition = m_ThisTransform.position;

        m_AnimationTarget.wrapMode = WrapMode.Loop;
    }

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
    void Update()
    {
        Vector3 characterVelocity = (m_ThisTransform.position - m_LastPosition) / Time.deltaTime;
        m_LastPosition = m_ThisTransform.position;
        Vector3 horizontalVelocity = characterVelocity;
        horizontalVelocity.y = 0;
        float speed = horizontalVelocity.magnitude;

        if (speed > Mathf.Epsilon)
        {
            float forwardMotion = Vector3.Dot(m_ThisTransform.forward, horizontalVelocity);
            float t = 0.0f;

            if (forwardMotion > 0)
            {
                t = Mathf.Clamp(Mathf.Abs(speed / m_MaxForwardSpeed), 0.0f, m_MaxForwardSpeed);
                m_AnimationTarget["run"].speed = Mathf.Lerp(0.25f, 1.0f, t);

                if (m_AnimationTarget.IsPlaying("idle"))
                {
                    m_AnimationTarget.Play("run");
                }
                else
                {
                    m_AnimationTarget.CrossFade("run");
                }
            }
        }
        else
        {
            m_AnimationTarget.CrossFade("idle");
        }
    }
}
