using UnityEngine;
using System.Collections;

public class ListenForInputState : State {

	#region Fields

	private LevelFSM levelfsm;

	/// <summary>
	/// This is the tile we last hit with the raycast from the mouse
	/// </summary>
	private Tile tileHit;

	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	/// <summary>
	/// Sets tileHit to null.
	/// </summary>
	public override void OnEnter()
	{
		tileHit = null;
	}

	/// <summary>
	/// If the user clicks with the mouse, it finds the offset of that tile
	/// and assigns tileHit the correct tile based on the levelfsm tileDictionary
	/// </summary>
	public override void OnUpdate()
	{
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			tileHit = levelfsm.FindTileUnderMouse();
			Debug.Log(tileHit);
			Debug.Log("Offset; " + HexMath.OffsetRound(new Vector2(tileHit.transform.position.x, tileHit.transform.position.z), levelfsm.PointRadius, levelfsm.OffsetType));
		}
	}

	/// <summary>
	/// Transition: SelectCharacterTargetTileState
	/// - If update found a tile in range, and its got a character on it
	/// </summary>
	public override void CheckTransitions()
	{
		if ( tileHit != null && tileHit.OccupiedBy != null )
		{
			levelfsm.AttemptTransition(LevelFSMStates.SelectCharacterTargetTileState );
		}
	}

	/// <summary>
	/// Set the fsm current tile to the one just clicked on, and 
	/// the current character to the one occupying that tile
	/// </summary>
	public override void OnExit()
	{
		levelfsm.CurrentlySelectedCharacter = tileHit.OccupiedBy;
		levelfsm.CurrentlySelectedTile = tileHit;
	}

	#endregion
}
