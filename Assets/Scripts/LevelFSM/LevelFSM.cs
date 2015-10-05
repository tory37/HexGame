using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelFSM : MonoFSM {

	[SerializeField] private Transform TileMaster;

	public Dictionary<Vector3, Tile> tileDictionary { get; private set; }

	protected override void SetStates()
	{
		states = new List<IState>
		{
			new ListenForInputState()
		};
	}

	protected override void Initialize()
	{
		FillDictionary();
	}

	private void FillDictionary()
	{
		tileDictionary = new Dictionary<Vector3, Tile>();

		Component[] tiles = TileMaster.GetComponentsInChildren(typeof (Tile));

		foreach (Tile tile in tiles )
		{
			Vector3 pos = tile.transform.position;
			HexMath.Offset offset = HexMath.OffsetRound(new Vector2(pos.x, pos.z), .5f, HexMath.OffsetType.OddR);
			int height = (int)(pos.y / .2f);
			Vector3 key = new Vector3(offset.x, height, offset.y);
			if ( !tileDictionary.ContainsKey(key) )
				tileDictionary.Add(key, tile);
			else
				Destroy(tile.gameObject);
		}
	}
}
