using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PortalManager : MonoBehaviour 
{
	private class PortalData
	{
		private float ACTIVE_DURATION = 30.0f;
		
		public PortalData(GameObject _obj)
		{
			obj = _obj;
		}

		public void Activate()
		{
			obj.SetActive(true);
			timeActive = 0.0f;
		}

		public void Deactivate()
		{
			obj.SetActive(false);
			timeActive = 0.0f;
		}

		public bool Update()
		{
			timeActive += Time.deltaTime;
			return timeActive > ACTIVE_DURATION;
		}

		private GameObject obj = null;
		private float timeActive = 0.0f;

		public Vector3 pos 
		{
			get {return obj.transform.position;}
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		var mazeManager = MazeManager.instance;

		Func<int, int, GameObject> spawnPortal = (i, j) =>
		{
			var portalPos = mazeManager.GetCellCenterPos(i, j);
			var portalCopy = (GameObject)UnityEngine.Object.Instantiate(sourcePortal.gameObject, portalPos, Quaternion.identity);
			portalCopy.SetActive(false);
			return portalCopy;
		};

		mPortals = new List<PortalData>();

		var mazeWidth = mazeManager.mMazeData.Width();
		var mazeHeight = mazeManager.mMazeData.Height();

		mPortals.Add(new PortalData(spawnPortal(2,2)));
		mPortals.Add(new PortalData(spawnPortal(mazeWidth / 2, mazeHeight / 2)));
		mPortals.Add(new PortalData(spawnPortal(mazeWidth - 2, mazeHeight - 2)));

		ActivateRandomPortal();
	}

	private void ActivateRandomPortal()
	{
		foreach(var portal in mPortals)
			portal.Deactivate();

		int randIndex = UnityEngine.Random.Range(0, mPortals.Count);
		mActivePortal = mPortals[randIndex];

		mActivePortal.Activate();
		Compass.instance.mTargetPos = mActivePortal.pos;
	}

	// Update is called once per frame
	void Update () 
	{
		if(mActivePortal.Update())
			ActivateRandomPortal();
	}

	private List<PortalData> mPortals;
	private PortalData mActivePortal;

	public Transform sourcePortal;
}
