using UnityEngine;
using System.Collections;

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
	}
}
