using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	#region Editor Interface

	[Header("Base Stats")]
	[SerializeField] private int height;
    [SerializeField] private int movement;
	[SerializeField] private float moveAnimSpeed;

	#endregion

	#region Public Interface

	public int Height { get { return height; } }
    public int Movement { get { return movement; } }

	public float MoveAnimSpeed { get { return moveAnimSpeed; } }

	#endregion

}
