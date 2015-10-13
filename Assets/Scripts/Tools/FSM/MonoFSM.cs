using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Implement this to have a functioning finite state machine that is also a monobehaviour
/// </summary>
public abstract class MonoFSM : MonoBehaviour {

	/// <summary>
	/// This is a list of states in the fsm
	/// </summary>
	public Dictionary<string, State> states;

	/// <summary>
	/// This represents what state the machine is currently in
	/// </summary>
	State currentState = null;

	/// <summary>
	/// This calls initialize on each state in the fsm. 
	/// </summary>
	protected virtual void Start()
	{
		SetStates();

		Initialize();

		foreach ( State state in states.Values )
		{
			state.Initialize(this);
		}

		currentState = states.Values.ElementAt( 0 );
	}

	protected abstract void SetStates();

	protected virtual void Initialize() { }

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

	public void Transition(string toState)
	{
		currentState.OnExit();
		if ( states.TryGetValue( toState, out currentState ) )
		{
			currentState.OnEnter();
		}
		else
			throw new Exception( "State " + toState + " is not defined, check if your string is correct." );
	}
}
