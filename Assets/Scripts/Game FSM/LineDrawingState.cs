using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LineDrawingState : State
{
	private GameFSM gameFSM;

	public void Initialize( MonoFSM callingfsm )
	{
		gameFSM = (GameFSM)callingfsm;
	}

	public override void OnUpdate()
    {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		List<HexMath.Cube> hexes = HexMath.GetCubesInLine(new HexMath.Cube(0, 0, 0), HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(mousePos.x, mousePos.z), .5f, HexMath.OffsetType.OddR))));

		GameObject.Destroy(gameFSM.lineContainer.gameObject);
		gameFSM.lineContainer = new GameObject("Line Continer").GetComponent<Transform>();

		foreach(var cube in hexes)
		{
			HexMath.Offset offset = HexMath.CubeToOddR(cube);
			Vector2 world = HexMath.OffsetToWorld(offset, .5f, HexMath.OffsetType.OddR);
			Tile t = (Tile)GameObject.Instantiate(gameFSM.tileToDraw, new Vector3(world.x, 0f, world.y), Quaternion.Euler(0, 30f, 0));
			t.transform.parent = gameFSM.lineContainer;
		}
	}
}
