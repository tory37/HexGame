using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFSM : MonoFSM {

	#region Editor Interface

	public Tile tileToDraw;

	public Transform lineContainer;

	#endregion

	protected override void Start()
	{
		base.states = new List<IState>
		{
			new RangeState(),
			new TestingState()
		};

		base.Start();
	}
	
}