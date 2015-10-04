using UnityEngine;
using System.Collections;

/// <summary>
/// Implement this to make a class a state
/// </summary>
public interface IState
{
	/// <summary>
	/// Use this function to do anything you would need to do in
	/// start for this state, and also to set a reference to the 
	/// FSM if you need.
	/// </summary>
	/// <param name="callingfsm"></param>
	void Initialize(MonoFSM callingfsm);
	void OnEnter();
	void OnUpdate();
	void OnFixedUpdate();
	void OnLateUpdate();
	void OnExit();
	void CheckTransitions();
}
