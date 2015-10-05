using UnityEngine;
using System.Collections;
using System;

public class ListenForInputState : IState
{
	private LevelFSM levelfsm;

	public void Initialize( MonoFSM callingfsm )
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	public void OnEnter() { }

	public void OnUpdate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if ( Physics.Raycast(ray, out hit) )
			{
				HexMath.Offset offset = HexMath.OffsetRound(new Vector2(hit.point.x, hit.point.z), .5f, HexMath.OffsetType.OddR);
				Vector3 position = new Vector3(offset.x, 0f, offset.y);
				Debug.Log(levelfsm.tileDictionary.ContainsKey(position));
			}
		}
	}

	public void OnFixedUpdate(){}

	public void OnLateUpdate() { }

	public void OnExit(){}

	public void CheckTransitions() { }
}
