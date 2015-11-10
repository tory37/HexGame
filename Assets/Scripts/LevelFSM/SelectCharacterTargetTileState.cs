using UnityEngine;
using System.Collections.Generic;

public class SelectCharacterTargetTileState : State {

	#region Fields

	private LevelFSM levelfsm;

	/// <summary>
	/// The Tile component of the tile object being worked with
	/// </summary>
	private Tile selectedTile;

	/// <summary>
	/// The offset coordinates of the tile object being worked with
	/// </summary>
	private HexMath.Offset selectedTileCoords;

	/// <summary>
	/// The cube coordinate of the last cube hit by our raycast
	/// when moving in search of a target tile
	/// </summary>
	private HexMath.Offset lastCoord;

	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	/// <summary>
	/// Does initialization for the state
	/// </summary>
	public override void OnEnter()
	{
		// Initialize these variables
		selectedTile = levelfsm.CurrentlySelectedTile;
		selectedTileCoords = HexMath.OffsetRound( new Vector2( selectedTile.transform.position.x, selectedTile.transform.position.z ), levelfsm.PointRadius, levelfsm.OffsetType );
		levelfsm.moveTileCoords = new List<HexMath.Offset>();

		lastCoord = selectedTileCoords;

		DrawPath(selectedTileCoords, selectedTileCoords);
	}

	/// <summary>
	/// Draws the lines from starting tile to target under mouse if in character's range
	/// </summary>
	public override void OnUpdate()
	{
		// Round that hit location to a offset coordinate
		Tile tileHit = levelfsm.FindTileUnderMouse();

		if ( tileHit != null && tileHit.OccupiedBy == null)
		{
			HexMath.Offset offsetHit = HexMath.OffsetRound(new Vector2(tileHit.transform.position.x, tileHit.transform.position.z), levelfsm.PointRadius, levelfsm.OffsetType);

			// If the offset coordinate we hit is not the same as the last one and within the characters range, redraw the tiles in between the two locations
			if ( offsetHit != lastCoord && HexMath.OffsetDistance(selectedTileCoords, offsetHit, levelfsm.OffsetType) <= levelfsm.CurrentlySelectedCharacter.Movement )
			{
				lastCoord = offsetHit;

				DrawPath(selectedTileCoords, offsetHit);
			}
		}
	}

	/// <summary>
	/// Transition: MoveCharacter
	/// - If you clicked on a tile within range this frame
	/// </summary>
	public override void CheckTransitions()
	{
		//If the user clicks a valid tile
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			if ( Physics.Raycast( ray, out hit ) )
			{
				HexMath.Offset hitOffset = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), levelfsm.PointRadius, levelfsm.OffsetType );

				int distance = HexMath.OffsetDistance( selectedTileCoords, hitOffset, levelfsm.OffsetType );
				if ( distance > 0 && distance <= levelfsm.CurrentlySelectedCharacter.Movement )
				{
					levelfsm.AttemptTransition( LevelFSMStates.MoveCharacter ); 
				}
			}
		}
	}

	private void DrawPath(HexMath.Offset a, HexMath.Offset b)
	{
		// Create a new line container and give it the starting tile to
		// draw
		if ( levelfsm.LineContainer != null )
			GameObject.Destroy( levelfsm.LineContainer.gameObject );

		levelfsm.LineContainer = new GameObject( "Line Continer" ).GetComponent<Transform>();

		// If what we hit was the starting tile, then just draw one over that
		if ( a == b )
			levelfsm.moveTileCoords = new List<HexMath.Offset> { a };
		// Else get the tiles between the two points
		else
			levelfsm.moveTileCoords = HexMath.GetHexInLine(selectedTileCoords, lastCoord, levelfsm.OffsetType);

		// Draw em
		foreach ( var offset in levelfsm.moveTileCoords )
		{
			Vector2 world = HexMath.OffsetToWorld( offset, levelfsm.PointRadius, levelfsm.OffsetType );
			Tile t = (Tile)GameObject.Instantiate( levelfsm.SelectionTilePrefab, new Vector3( world.x, levelfsm.TileHeight, world.y ), levelfsm.TileRotation );
			t.transform.parent = levelfsm.LineContainer;
		}
	}

	#endregion
}
