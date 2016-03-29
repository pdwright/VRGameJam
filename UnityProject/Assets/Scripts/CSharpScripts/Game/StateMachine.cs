using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {

	public delegate void UpdateFn();
	public delegate void EnterFn(int _prevState);
	public delegate void ExitFn(int _nextState);

	const int INVALID_STATE = -1;

	protected StateMachine(int _stateCount)
	{
		mCurrentState = mNextState = INVALID_STATE;

		mUpdateFn = new UpdateFn[_stateCount];
		mEnterFn = new EnterFn[_stateCount];
		mExitFn = new ExitFn[_stateCount];
	}

	protected void SetNextState(int _nextState)
	{
		mNextState = _nextState;
	}

	protected void SetStateFn(int _state, UpdateFn _update, EnterFn _enter = null, ExitFn _exit = null)
	{
		Debug.Assert(_state != INVALID_STATE);
		Debug.Assert(_state < mUpdateFn.Length);
		
		mUpdateFn[_state] = _update;
		mEnterFn[_state] = _enter;
		mExitFn[_state] = _exit;
	}

	// Update is called once per frame
	void Update () 
	{
		//update current state
		if(mCurrentState != INVALID_STATE)
		{
			if(mUpdateFn[mCurrentState] != null)
				mUpdateFn[mCurrentState]();
		}

		//transit to next state if needed
		if(mCurrentState != mNextState)
		{
			var lastState = mCurrentState;

			//assign current state so GetCurrentState returns correct value
			mCurrentState = mNextState;
			
			if(lastState != INVALID_STATE && mExitFn[lastState] != null)
				mExitFn[lastState](mNextState);
			if(mNextState != INVALID_STATE && mEnterFn[mNextState] != null)
				mEnterFn[mNextState](lastState);
		}
	}

	private UpdateFn[] mUpdateFn;
	private EnterFn[] mEnterFn;
	private ExitFn[] mExitFn;

	private int mCurrentState;
	private int mNextState;
}
