using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Sets animation speed from user parameter
// ----------------------------------------------------------------------------------------------------
public class AnimationSpeed : MonoBehaviour
{
    public Animation m_AnimationTarget;
    public float m_Speed = 1.0f;

    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {
        m_AnimationTarget[m_AnimationTarget.clip.name].speed = m_Speed;
	}
}
