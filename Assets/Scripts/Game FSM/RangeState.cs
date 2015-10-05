using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RangeState : State
{
	private GameFSM gamefsm;

	public void Initialize( MonoFSM callingfsm )
	{
		gamefsm = (GameFSM)callingfsm;
	}

	public void OnEnter()
	{
		
	}

	public void OnUpdate()
	{
		int input = 0;

		if ( Input.GetKeyDown(KeyCode.Alpha1) )
			input = 1;
		else if ( Input.GetKeyDown(KeyCode.Alpha2) )
			input = 2;
		else if ( Input.GetKeyDown(KeyCode.Alpha3) )
			input = 3;
		else if ( Input.GetKeyDown(KeyCode.Alpha4) )
			input = 4;
		else if ( Input.GetKeyDown(KeyCode.Alpha5) )
			input = 5;
		else if ( Input.GetKeyDown(KeyCode.Alpha6) )
			input = 6;
		else if ( Input.GetKeyDown(KeyCode.Alpha7) )
			input = 7;

		if (input != 0)
		{
			List<HexMath.Cube> inRangeCubes = HexMath.GetHexInRange(new HexMath.Cube(3, -1, -2), input);

			GameObject.Destroy(gamefsm.lineContainer.gameObject);
			gamefsm.lineContainer = new GameObject("Line Continer").GetComponent<Transform>();

			foreach(var cube in inRangeCubes)
			{
				HexMath.Offset offset = HexMath.CubeToOddR(cube);
				Vector2 world = HexMath.OffsetToWorld(offset, .5f, HexMath.OffsetType.OddR);
				Tile t = (Tile)GameObject.Instantiate(gamefsm.tileToDraw, new Vector3(world.x, 0f, world.y), Quaternion.Euler(0, 30f, 0));
				t.transform.parent = gamefsm.lineContainer;
			}
		}
	}

	public void OnFixedUpdate()
	{
		
	}

	public void OnLateUpdate()
	{
		
	}
	
	public void OnExit()
	{
		
	}

	public void CheckTransitions()
	{
		
	}
}
