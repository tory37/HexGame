using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSelection : MonoBehaviour {

	#region Editor Interface

	[Header("Tile Data")]
	[SerializeField] private GameObject selectionTile;
	[SerializeField] private HexMath.OffsetType offsetType;

	[Header("Tile Selection")]
	[SerializeField] private float selectionDistance;
	[SerializeField] private LayerMask selectionMask;

	Dictionary<Vector2, Tile> levelTiles;

	#endregion

	private void Start()
	{
		SetTileDictionary();
	}

	private void Update()
	{
		ShowSelectionTile();
	}

	private void SetTileDictionary()
	{
		foreach (Tile tile in transform.GetComponentsInChildren<Tile>())
		{
			levelTiles.Add()
		}
	}

	private void ShowSelectionTile()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, selectionDistance, selectionMask))
		{
			selectionTile
		}
	}

	
}
