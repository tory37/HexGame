using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	#region Editor Interface

	[Header("Base Stats")]
	[SerializeField] private int height;
    [SerializeField] private int movement;
	[SerializeField] private float moveAnimSpeed;
    [SerializeField] private bool hasFlying;

	#endregion

	#region Public Interface

	public int Height { get { return height; } }
    public int Movement { get { return movement; } }
    public bool HasFlying { get { return hasFlying; } }

	public float MoveAnimSpeed { get { return moveAnimSpeed; } }

	#endregion

}
