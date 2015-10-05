using UnityEngine;
using System.Collections;

/// <summary>
/// Implement this to make a class a state
/// </summary>
public abstract class State
{
	/// <summary>
	/// Use this function to do anything you would need to do in
	/// start for this state, and also to set a reference to the 
	/// FSM if you need.
	/// </summary>
	/// <param name="callingfsm"></param>
    public virtual void Initialize(MonoFSM callingfsm) { }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
    public virtual void CheckTransitions() { }
}
