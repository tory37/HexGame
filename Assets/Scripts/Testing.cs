using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Testing : MonoBehaviour {

	void Update()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 50, Color.red);
		if ( Input.GetMouseButtonDown(0) )
		{
			if ( Physics.Raycast(ray, out hit) )
			{
				Debug.Log("Offset: " + HexMath.OffsetRound(new Vector2(hit.point.x, hit.point.z), .5f, HexMath.OffsetType.OddR));
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Dictionary<Vector3, string> tiles = LevelLoader.Instance.GetLevelData("testing");
			foreach (var tile in tiles)
			{
				Debug.Log(tile.Key + ": " + tile.Value);
			}
		}
	}
}
