using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	#region Editor Interface

	[Header("Base Stats")]
	[SerializeField] private int height;

	#endregion

	#region Public Interface

	public int Height { get { return height; } }

	#endregion

}
