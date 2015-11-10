using UnityEngine;
using System.Collections.Generic;

public class MoveCharacter : State {

	#region Fields

	private LevelFSM levelfsm;

	/// <summary>
	/// The positions to move the character to
	/// </summary>
	private List<Vector3> tilePositions;

	/// <summary>
	/// This increments to the next element in /tilePositions/ as the character
	/// reaches each tile
	/// </summary>
	private Vector3 currentTarget;

	/// <summary>
	/// This is checked to see whether or not we have reachs our final tile in
	/// the list
	/// </summary>
	private bool finishedMoving = false;

	/// <summary>
	/// The current character, cached from the fsm for quicker typing
	/// </summary>
	private Character cha;
	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	/// <summary>
	/// Finds the vector3 position of each tile in the path and initialize some 
	/// variables
	/// </summary>
	public override void OnEnter()
	{
		if ( levelfsm.LineContainer != null )
			GameObject.Destroy(levelfsm.LineContainer.gameObject);

		cha = levelfsm.CurrentlySelectedCharacter;

		finishedMoving = false;

		tilePositions = new List<Vector3>();

		foreach ( HexMath.Offset offset in levelfsm.moveTileCoords )
		{
			Vector2 world = HexMath.OffsetToWorld( offset, levelfsm.PointRadius, levelfsm.OffsetType );
			tilePositions.Add( new Vector3( world.x, 0f, world.y ) );
		}

		currentTarget = tilePositions[1];
	}

	/// <summary>
	/// Moves the character towards the target tile
	/// </summary>
	public override void OnUpdate()
	{
		// Set the amount to move
		float deltaMove = cha.MoveAnimSpeed * Time.deltaTime;
		Vector3 direction = currentTarget - cha.transform.position;
		// Clamp if we are closer to the correct position than our move
		deltaMove = Mathf.Clamp(deltaMove, 0f, direction.magnitude);
		cha.transform.position += direction.normalized * deltaMove;

		// If we are within .1 units of the center of the tile
		if ( Mathf.Abs(cha.transform.position.sqrMagnitude - currentTarget.sqrMagnitude) < .1f )
		{
			int index = tilePositions.IndexOf( currentTarget );

			// If we have more tiles in the path, we move on to the next one
			if ( index < tilePositions.Count - 1 )
				currentTarget = tilePositions[index + 1];
			// Else we say we're done moving
			else
			{
				Debug.Log( "Done Moving" );
				finishedMoving = true;
			}
		}
	}

	/// <summary>
	/// Transition: ListenForInput
	/// - If the character is done moving
	/// </summary>
	public override void CheckTransitions()
	{
		if ( finishedMoving == true )
			levelfsm.AttemptTransition( LevelFSMStates.ListenForInput );
	}

	/// <summary>
	/// Removes the character from the old tile and places it in the new one
	/// </summary>
	public override void OnExit()
	{
		// Reset the coordinate list
		levelfsm.moveTileCoords = null;

		// Remove the character from the old tile
		levelfsm.CurrentlySelectedTile.OccupiedBy = null;

		// Add the character to the new tile
		HexMath.Offset offset = HexMath.OffsetRound(new Vector2(currentTarget.x, currentTarget.z), levelfsm.PointRadius, levelfsm.OffsetType);
		levelfsm.tileDictionary[new Vector3(offset.x, 0f, offset.y)].OccupiedBy = levelfsm.CurrentlySelectedCharacter;
	}

	#endregion
}
