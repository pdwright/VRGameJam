using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
	static int WIDTH = 30;
	static int HEIGHT = 30;
	static float SHUFFLE_CLOSED_DOOR_RATIO = 0.3f;

    // Use this for initialization
    void Start()
    {
		mMazeData = new MazeData.Maze(WIDTH, HEIGHT);
		mMazeData.Braid(true);
		mMazeData.ShuffleDoors(SHUFFLE_CLOSED_DOOR_RATIO);

		mMonster = new MazeData.Monster(mMazeData, WIDTH - 2, HEIGHT - 2);

		var baseWalls = GameObject.FindGameObjectsWithTag("SourceWall");

		mfDebugDrawScale = baseWalls[0].GetComponent<Renderer>().bounds.size.x;

		mWalls = MazeData.Util.CreateWalls(mMazeData, baseWalls);
    }

    // Update is called once per frame
    void Update()
    {
        mfShuffleDoorTimer += Time.deltaTime;
		if(mfShuffleDoorTimer > 10.0f)
		{
			mMazeData.ShuffleDoors(SHUFFLE_CLOSED_DOOR_RATIO);
			mfShuffleDoorTimer = 0.0f;
		}

		mfMonsterMoveTimer += Time.deltaTime;
		if(mfMonsterMoveTimer > 0.3f)
		{
			mMonster.Wander(mMonsterTargetX, mMonsterTargetY, 0.5f);

			if(mMonster.GetPosX() == mMonsterTargetX && mMonster.GetPosY() == mMonsterTargetY)
			{
				mMonsterTargetX = Random.Range(0, WIDTH);
				mMonsterTargetY = Random.Range(0, HEIGHT);
			}

			mfMonsterMoveTimer = 0.0f;
		}

		MazeData.Util.DebugDraw(mMazeData, mfDebugDrawScale, mMonster);

		var player = GameObject.Find("Player");
		mMonsterTargetX = (int)(player.transform.position.x / mfDebugDrawScale);
		mMonsterTargetY = (int)(player.transform.position.z / mfDebugDrawScale);

		var bestPath = mMazeData.FindBestPath(mMonster.GetPosX(), mMonster.GetPosY(), mMonsterTargetX, mMonsterTargetY);
		MazeData.Util.DrawPath(mMonster.GetPosX(), mMonster.GetPosY(), bestPath, mfDebugDrawScale);
    }

	private MazeData.Maze mMazeData;
	private MazeData.Monster mMonster;

	private int mMonsterTargetX = (int)(WIDTH * 0.8f);
	private int mMonsterTargetY = (int)(HEIGHT * 0.8);

	private float mfShuffleDoorTimer = 0.0f;
	private float mfMonsterMoveTimer = 0.0f;

	List<GameObject> mWalls;

	private float mfDebugDrawScale;
}

