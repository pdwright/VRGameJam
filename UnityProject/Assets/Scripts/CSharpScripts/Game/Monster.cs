using UnityEngine;
using System.Collections;

public class Monster : StateMachine {

	private enum States : int
	{
		Wandering,

		Count
	}

	Monster()
	{
		InitStateMachine((int)States.Count);

		SetStateFn((int)States.Wandering, UpdateWandering);

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
		var playerPos = MazeManager.instance.ToIndex(MazeManager.player.transform.position);

		var curTargetX = mMonsterData.GetPosX();
		var curTargetY = mMonsterData.GetPosY();

		var curTargetPos = MazeManager.instance.GetCellCenterPos(curTargetX, curTargetY);

		if((curTargetPos - transform.position).magnitude < 0.2f)
			mMonsterData.Wander(playerPos[0], playerPos[1], 0.3f);

		MoveTo(curTargetPos, 5.0f);

		var bestPath = mMonsterData.mMaze.FindBestPath(mMonsterData.GetPosX(), mMonsterData.GetPosY(), playerPos[0], playerPos[1]);
		MazeData.Util.DrawPath(mMonsterData.GetPosX(), mMonsterData.GetPosY(), bestPath, MazeManager.instance.mWallSize);
	}

	private void MoveTo(Vector3 _pos, float _speed)
	{
		transform.position = Vector3.MoveTowards(transform.position, _pos, _speed * Time.deltaTime);

		Vector3 diffToTarget = _pos - transform.position;
		diffToTarget.y = 0.0f;
		transform.forward = Vector3.RotateTowards(transform.forward, diffToTarget, Mathf.PI * 2.0f * Time.deltaTime, 0.3f);
	}


	private MazeData.Monster mMonsterData;
}
