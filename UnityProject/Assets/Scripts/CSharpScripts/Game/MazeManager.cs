using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeManager : MonoBehaviour
{
	static int WIDTH = 30;
	static int HEIGHT = 30;
	static float SHUFFLE_CLOSED_DOOR_RATIO = 0.3f;

    // Use this for initialization
    void Awake()
    {
		Debug.Assert(instance == null);
		instance = this;
		
		mMazeData = new MazeData.Maze(WIDTH, HEIGHT);
		mMazeData.Braid(true);

		//mMonster = new MazeData.Monster(mMazeData, WIDTH - 2, HEIGHT - 2);

		mfWallSize = sourceWalls[0].GetComponent<Renderer>().bounds.size.x;

		mWalls = MazeData.Util.CreateWalls(mMazeData, sourceWalls);
		mDoors = MazeData.Util.CreateDoors(mMazeData, sourceDoor.gameObject);

		ShuffleDoors(SHUFFLE_CLOSED_DOOR_RATIO);
    }

    // Update is called once per frame
    void Update()
    {
        mfShuffleDoorTimer += Time.deltaTime;
		if(mfShuffleDoorTimer > 10.0f)
		{
			ShuffleDoors(SHUFFLE_CLOSED_DOOR_RATIO);
			mfShuffleDoorTimer = 0.0f;
		}

		MazeData.Util.DebugDraw(mMazeData, mfWallSize, null);

		/*mfMonsterMoveTimer += Time.deltaTime;
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



		var player = GameObject.Find("Player");
		mMonsterTargetX = (int)(player.transform.position.x / mfDebugDrawScale);
		mMonsterTargetY = (int)(player.transform.position.z / mfDebugDrawScale);

		var bestPath = mMazeData.FindBestPath(mMonster.GetPosX(), mMonster.GetPosY(), mMonsterTargetX, mMonsterTargetY);
		MazeData.Util.DrawPath(mMonster.GetPosX(), mMonster.GetPosY(), bestPath, mfDebugDrawScale);*/
    }

	private void ShuffleDoors(float _closedDoorRatio)
	{
		mMazeData.ShuffleDoors(_closedDoorRatio);

		int doorIndex = 0;
		mMazeData.ForEachDoor(isOpen => 
		{
			mDoors[doorIndex].SetActive(!isOpen);
			++doorIndex; 
		});
	}

	public Vector3 GetCellCenterPos(int _i, int _j)
	{
		return new Vector3(((float)_i + 0.5f) * mfWallSize, 0.0f, ((float)_j + 0.5f) * mfWallSize);
	}

	public MazeData.Maze mMazeData 
	{
		get;
		private set;
	}

	private float mfShuffleDoorTimer = 0.0f;

	List<GameObject> mWalls;
	List<GameObject> mDoors;

	private float mfWallSize;

	public static MazeManager instance = null;

	public Transform[] sourceWalls;
	public Transform sourceDoor;

	//monster stuff to be deleted
	/*private MazeData.Monster mMonster;

	private int mMonsterTargetX = (int)(WIDTH * 0.8f);
	private int mMonsterTargetY = (int)(HEIGHT * 0.8);

	private float mfMonsterMoveTimer = 0.0f;*/
}

