using UnityEngine;
using System.Collections;

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

		mMonster = new MazeData.Monster(mMazeData);
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
		if(mfMonsterMoveTimer > 0.1f)
		{
			mMonster.Wander(mMonsterTargetX, mMonsterTargetY, 0.5f);

			if(mMonster.GetPosX() == mMonsterTargetX && mMonster.GetPosY() == mMonsterTargetY)
			{
				mMonsterTargetX = Random.Range(0, WIDTH);
				mMonsterTargetY = Random.Range(0, HEIGHT);
			}

			mfMonsterMoveTimer = 0.0f;
		}

		float drawScale = 30.0f;
		MazeData.Util.DebugDraw(mMazeData, drawScale, mMonster);

		var bestPath = mMazeData.FindBestPath(mMonster.GetPosX(), mMonster.GetPosY(), mMonsterTargetX, mMonsterTargetY);
		MazeData.Util.DrawPath(mMonster.GetPosX(), mMonster.GetPosY(), bestPath, drawScale);
    }

	private MazeData.Maze mMazeData;
	private MazeData.Monster mMonster;

	private int mMonsterTargetX = (int)(WIDTH * 0.8f);
	private int mMonsterTargetY = (int)(HEIGHT * 0.8);

	private float mfShuffleDoorTimer = 0.0f;
	private float mfMonsterMoveTimer = 0.0f;
}

