using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		mInitialRot = transform.localEulerAngles;

		//mTargetPos = new Vector3(0.0f, 0.0f, 0.0f);

		mTargetObjTransform = transform.Find("Target");
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.forward = Vector3.forward;
		transform.up = Quaternion.AngleAxis(mInitialRot.x, transform.parent.right) * transform.up;
		transform.up = Quaternion.AngleAxis(mInitialRot.z, transform.parent.forward) * transform.up;

		UpdateTarget();
	}

	private static float SignedAngle(Vector3 _from, Vector3 _to)
	{
		float angle = Vector3.Angle(_from, _to);
		return Vector3.Cross(_from, _to).y < 0.0f ? -angle : angle;
	}

	private static float ToSignedAngle(float _angle)
	{
		if(_angle <= 180.0f)
			return _angle;
		return _angle - 360.0f;
	}

	private void UpdateTarget()
	{
		if(mTargetPos == null)
			return;
		
		Vector3 diffToTarget = mTargetPos - transform.position;
		diffToTarget.y = 0.0f;
		if(diffToTarget.magnitude < 0.5f)
			return;

		var curForward = mTargetObjTransform.forward;
		curForward.y = 0.0f;

		float signedAngle = SignedAngle(curForward, diffToTarget);

		mTargetObjTransform.Rotate(new Vector3(0.0f, signedAngle, 0.0f));
	}

	private Vector3 mInitialRot;
	private Vector3 mTargetPos {get; set;}
	private Transform mTargetObjTransform;
}
