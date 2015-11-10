using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameFSM : MonoFSM {

	#region Editor Interface

	public Tile tileToDraw;

	public Transform lineContainer;

	#endregion

	//protected override void SetStates()
	//{
	//	states = new Dictionary<string,State>
	//	{
	//		{ "LineDrawingState", new LineDrawingState()}
	//	};
	//}
}