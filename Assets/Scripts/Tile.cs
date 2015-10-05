using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	public bool Obstructed { get { return obstructred; } }
	[SerializeField] private bool obstructred;

    [SerializeField] public Character occupiedBy = null;

	public void Instantiate(HexMath.Cube position)
	{

	}
}