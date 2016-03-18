using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Base class for character controls
// ----------------------------------------------------------------------------------------------------
public abstract class CharacterControl : MonoBehaviour
{
    protected bool m_FaceMovementDirection = false;

    public bool FaceMovementDirection
    {
        get { return m_FaceMovementDirection; }
    }

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
	
	}

    // ----------------------------------------------------------------------------------------------------
    // Gets the movement to apply to the player
    // ----------------------------------------------------------------------------------------------------
    public abstract Vector3 GetMovement(CharacterController _controller, float _forwardSpeed, float _backwardSpeed, float _sidestepSpeed);

    // ----------------------------------------------------------------------------------------------------
    // Gets whether the player starts a jump
    // ----------------------------------------------------------------------------------------------------
    public abstract bool GetJump(CharacterController _controller);
}
