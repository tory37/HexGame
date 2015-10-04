using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implement this to have a functioning finite state machine that is also a monobehaviour
/// </summary>
public abstract class MonoFSM : MonoBehaviour {

	/// <summary>
	/// This is a list of states in the fsm
	/// </summary>
	public List<IState> states;

	/// <summary>
	/// This represents what state the machine is currently in
	/// </summary>
	IState currentState = null;

	/// <summary>
	/// This calls initialize on each state in the fsm. 
	/// </summary>
	protected virtual void Start()
	{
		foreach ( IState state in states )
		{
			state.Initialize(this);
		}

		currentState = states[0];
	}

	protected void Update()
	{
		currentState.OnUpdate();
	} 

	protected void FixedUpdate()
	{
		currentState.OnFixedUpdate();
	} 

	protected void LateUpdate()
	{
		currentState.OnLateUpdate();
		currentState.CheckTransitions();
	}

	protected void Transition(IState toState)
	{
		currentState.OnExit();
		currentState = toState;
		currentState.OnEnter();
	}
}
