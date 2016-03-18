using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Control used to move forward and jump over holes
// ----------------------------------------------------------------------------------------------------
public class AutoJumpControl : CharacterControl
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        m_FaceMovementDirection = true;
	}

    // ----------------------------------------------------------------------------------------------------
    // Update is called once per frame
    // ----------------------------------------------------------------------------------------------------
	void Update()
    {

	}

    // ----------------------------------------------------------------------------------------------------
    // Moves forward
    // ----------------------------------------------------------------------------------------------------
    public override Vector3 GetMovement(CharacterController _controller, float _forwardSpeed, float _backwardSpeed, float _sidestepSpeed)
    {
        return new Vector3(0.0f, 0.0f, 1.0f) * _forwardSpeed;
    }

    // ----------------------------------------------------------------------------------------------------
    // Automatically detect holes and jump
    // ----------------------------------------------------------------------------------------------------
    public override bool GetJump(CharacterController _controller)
    {
	    RaycastHit hit;
        Vector3 start = _controller.gameObject.transform.position + _controller.gameObject.transform.forward;
        Vector3 end = start + Vector3.down * 4;

        if (Physics.Linecast(start, end, out hit))
        {
            return false;
        }
        else
        {
            return true;
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
