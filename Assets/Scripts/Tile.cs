using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	#region Editor Interface

	
	[SerializeField] private bool obstructred;
    [SerializeField] private Character occupiedBy = null;


	#endregion 

	#region Public Interface

	public bool Obstructed { get { return obstructred; } }

	public Character OccupiedBy { get { return occupiedBy; } set { occupiedBy = value; } }

	#endregion

	public void Instantiate(HexMath.Cube position)
	{

	}
}