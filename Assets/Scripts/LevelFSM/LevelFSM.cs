using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelFSM : MonoFSM
{

	#region Editor Interface

	[Header("The parent of all the tiles containers")]
	[SerializeField]
	private Transform TileMaster;

	[Header("The prefab that displays over tile when they are selected, or in selection line, etc.")]
	[SerializeField]
	private Tile selectionTilePrefab;

	[Header("The current offset pattern of the game board.")]
	[SerializeField]
	private HexMath.OffsetType offsetType;

	#endregion

	#region Public Interface

	/// <summary>
	/// The dictionary of all tiles whose key is a vector3 defined by offset tile positions (row, vertical height, columns)
	/// </summary>
	public Dictionary<Vector3, Tile> tileDictionary { get; private set; }

	/// <summary>
	/// This represents the tile that is currently being worked with, if we are in a state concerned with that.
	/// It is in the machine to be passed between states
	/// </summary>
	public Tile CurrentlySelectedTile;

	/// <summary>
	/// This is the character that is currently being worked with, if we are in a state concerned with that.
	/// It is in the machine to be passed between states
	/// </summary>
	[HideInInspector]
	public Character CurrentlySelectedCharacter;

	/// <summary>
	/// This is the Transform that contains the tiles that draw lines when moving characters.
	/// This should be destroyed and null when we are not working with line drawing
	/// </summary>
	[HideInInspector]
	public Transform LineContainer;

	/// <summary>
	/// The prefab that displays over tile when they are selected, or in selection line, etc.
	/// </summary>
	public Tile SelectionTilePrefab { get { return selectionTilePrefab; } }

	/// <summary>
	/// The current offset pattern of the game board.
	/// </summary>
	public HexMath.OffsetType OffsetType { get { return offsetType; } }	

	/// <summary>
	/// This is a list of coordinates used for moving a character, if we are in a state that needs this.
	/// This is in the machine to be shared between states
	/// </summary>
	public List<HexMath.Offset> moveTileCoords = null;

	public Quaternion TileRotation
	{
		get	{
			if ( offsetType == HexMath.OffsetType.EvenR || offsetType == HexMath.OffsetType.OddR )
				return Quaternion.Euler(0f, 30f, 0f);
			else
				return Quaternion.identity;
		}
	}

		#endregion

		#region Overrides

		protected override void SetStates()
	{
		states = new Dictionary<string, State>
		{
			{ "ListenForInput", new ListenForInputState() },
			{ "SelectCharacterTargetTileState", new SelectCharacterTargetTileState() },
			{ "MoveCharacter", new MoveCharacter() }
		};
	}

	/// <summary>
	/// Calls the function that fills the tile dictionary with the level tiles
	/// </summary>
	protected override void Initialize()
	{
		FillDictionary();
	}

	#endregion

	#region Private
	
	/// <summary>
	/// Goes through every tranform that is a child of TileMaster and, if it has
	/// a Tile component, adds an entry in the tile dictionary representing 
	/// that tile
	/// </summary>
	private void FillDictionary()
	{
		tileDictionary = new Dictionary<Vector3, Tile>();

		Component[] tiles = TileMaster.GetComponentsInChildren( typeof( Tile ) );

		foreach ( Tile tile in tiles )
		{
			Vector3 pos = tile.transform.position;
			HexMath.Offset offset = HexMath.OffsetRound( new Vector2( pos.x, pos.z ), .5f, offsetType );
			int height = (int)(pos.y / .2f);
			Vector3 key = new Vector3( offset.x, height, offset.y );
			if ( !tileDictionary.ContainsKey( key ) )
				tileDictionary.Add( key, tile );
			else
				Destroy( tile.gameObject );
		}
	}

	#endregion
}

/// <summary>
/// This state listens for a click, and checks to see if the input 
/// was on an existing tile that was occupied by a character
/// </summary>
public class ListenForInputState : State
{
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
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			if ( Physics.Raycast( ray, out hit ) )
			{
				HexMath.Offset offset = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), .5f, levelfsm.OffsetType );
				Vector3 position = new Vector3( offset.x, 0f, offset.y );
				Debug.Log( levelfsm.tileDictionary.ContainsKey( position ) );
				levelfsm.tileDictionary.TryGetValue( position, out tileHit );
			}
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
			levelfsm.Transition( "SelectCharacterTargetTileState" );
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

/// <summary>
/// This state allows the user to select a target tile for the character to
/// move to and draws a line from the current tile to the tile under the mouse,
/// if it is in the chracters range
/// </summary>
public class SelectCharacterTargetTileState : State
{
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
		selectedTileCoords = HexMath.OffsetRound( new Vector2( selectedTile.transform.position.x, selectedTile.transform.position.z ), .5f, levelfsm.OffsetType );
		levelfsm.moveTileCoords = new List<HexMath.Offset>();

		// Create a new line container and give it the starting tile to
		// draw
		if ( levelfsm.LineContainer != null )
			GameObject.Destroy( levelfsm.LineContainer.gameObject );

		levelfsm.LineContainer = new GameObject( "Line Continer" ).GetComponent<Transform>();

		lastCoord = selectedTileCoords;
		Vector2 world = HexMath.OffsetToWorld( selectedTileCoords, .5f, levelfsm.OffsetType );
		Tile t = (Tile)GameObject.Instantiate( levelfsm.SelectionTilePrefab, new Vector3( world.x, 0.2f, world.y ), levelfsm.TileRotation );
		t.transform.parent = levelfsm.LineContainer;
	}

	/// <summary>
	/// Draws the lines from starting tile to target under mouse if in character's range
	/// </summary>
	public override void OnUpdate()
	{
		// If we hit the underlying platform
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		if ( Physics.Raycast( ray, out hit ) )
		{
			// Round that hit location to a offset coordinate
			HexMath.Offset offsetHit = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), .5f, levelfsm.OffsetType );

			// If the offset coordinate we hit is not the same as the last one and within the characters range, redraw the tiles in between the two locations
			if ( offsetHit != lastCoord && HexMath.OffsetDistance( selectedTileCoords, offsetHit, levelfsm.OffsetType ) <= levelfsm.CurrentlySelectedCharacter.Movement )
			{
				lastCoord = offsetHit;

				GameObject.Destroy(levelfsm.LineContainer.gameObject);

				if ( levelfsm.LineContainer != null )
					GameObject.Destroy( levelfsm.LineContainer.gameObject );

				levelfsm.LineContainer = new GameObject("Line Continer").GetComponent<Transform>();

				// If what we hit was the starting tile, then just draw one over that
				if ( lastCoord == selectedTileCoords )
					levelfsm.moveTileCoords = new List<HexMath.Offset> { lastCoord };
				// Else get the tiles between the two points
				else
					levelfsm.moveTileCoords = HexMath.GetHexInLine( selectedTileCoords, lastCoord, levelfsm.OffsetType );

				// Draw em
				foreach ( var offset in levelfsm.moveTileCoords )
				{
					Vector2 world = HexMath.OffsetToWorld( offset, .5f, levelfsm.OffsetType );
					Tile t = (Tile)GameObject.Instantiate( levelfsm.SelectionTilePrefab, new Vector3( world.x, 0.2f, world.y ), levelfsm.TileRotation );
					t.transform.parent = levelfsm.LineContainer;
				}
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
				HexMath.Offset hitOffset = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), .5f, levelfsm.OffsetType );

				int distance = HexMath.OffsetDistance( selectedTileCoords, hitOffset, levelfsm.OffsetType );
				if ( distance > 0 && distance <= levelfsm.CurrentlySelectedCharacter.Movement )
				{
					levelfsm.Transition( "MoveCharacter" ); 
				}
			}
		}
	}

	#endregion
}

/// <summary>
/// This goes through the list of tiles in the fsms list and moves
/// the character to each one incrementally
/// </summary>
public class MoveCharacter : State
{
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
			Vector2 world = HexMath.OffsetToWorld( offset, .5f, levelfsm.OffsetType );
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
			levelfsm.Transition( "ListenForInput" );
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
		HexMath.Offset offset = HexMath.OffsetRound(new Vector2(currentTarget.x, currentTarget.z), .5f, levelfsm.OffsetType);
		levelfsm.tileDictionary[new Vector3(offset.x, 0f, offset.y)].OccupiedBy = levelfsm.CurrentlySelectedCharacter;
	}

	#endregion
}