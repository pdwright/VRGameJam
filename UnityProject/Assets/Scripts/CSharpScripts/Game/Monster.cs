using UnityEngine;
using System.Collections;

public class Monster : StateMachine {

	static float DETECTION_RADIUS = 10.0f;

	private enum States : int
	{
		Wandering,
		HomingIn,

		Count
	}

	Monster()
	{
		InitStateMachine((int)States.Count);

		SetStateFn((int)States.Wandering, UpdateWandering);
		SetStateFn((int)States.HomingIn, UpdateHomingIn);

		SetNextState((int)States.Wandering);
	}

	// Use this for initialization
	void Start () 
	{
		int startX = MazeManager.instance.mMazeData.Width() - 2;
		int startY = MazeManager.instance.mMazeData.Height() - 2;
		
		mMonsterData = new MazeData.Monster(MazeManager.instance.mMazeData, startX, startY);

		var startPos = MazeManager.instance.GetCellCenterPos(startX, startY);
		transform.position = startPos;
	}

	private void UpdateWandering()
	{
		Vector3 playerPos = MazeManager.player.transform.position;
		
		MoveWithData(playerPos, 5.0f, true);

		if((playerPos - transform.position).magnitude < DETECTION_RADIUS)
			SetNextState((int)States.HomingIn);

		DrawShortestPathToPlayer();
	}

	private void UpdateHomingIn()
	{
		Vector3 playerPos = MazeManager.player.transform.position;

		MoveWithData(playerPos, 5.0f, false);

		DrawShortestPathToPlayer();
	}

	private void DrawShortestPathToPlayer()
	{
		var targetPos = MazeManager.instance.ToIndex(MazeManager.player.transform.position);
		var bestPath = mMonsterData.mMaze.FindBestPath(mMonsterData.GetPosX(), mMonsterData.GetPosY(), targetPos[0], targetPos[1]);
		MazeData.Util.DrawPath(mMonsterData.GetPosX(), mMonsterData.GetPosY(), bestPath, MazeManager.instance.mWallSize);

		MazeData.Util.DrawCircle(transform.position, DETECTION_RADIUS, Color.red);
	}

	private void MoveWithData(Vector3 _target, float _speed, bool _wander)
	{
		var targetPos = MazeManager.instance.ToIndex(MazeManager.player.transform.position);
		
		var curTargetX = mMonsterData.GetPosX();
		var curTargetY = mMonsterData.GetPosY();

		var curTargetPos = MazeManager.instance.GetCellCenterPos(curTargetX, curTargetY);

		if((curTargetPos - transform.position).magnitude < 0.2f)
		{
			if(_wander)
				mMonsterData.Wander(targetPos[0], targetPos[1], 0.3f);
			else
				mMonsterData.HomeIn(targetPos[0], targetPos[1]);
		}

		MoveToPos(curTargetPos, _speed);
	}

	private void MoveToPos(Vector3 _pos, float _speed)
	{
		transform.position = Vector3.MoveTowards(transform.position, _pos, _speed * Time.deltaTime);

		Vector3 diffToTarget = _pos - transform.position;
		diffToTarget.y = 0.0f;
		transform.forward = Vector3.RotateTowards(transform.forward, diffToTarget, Mathf.PI * 2.0f * Time.deltaTime, 0.3f);
	}


	private MazeData.Monster mMonsterData;
}
