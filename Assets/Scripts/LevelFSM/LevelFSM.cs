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

	[SerializeField]
	private float pointRadius;

	[SerializeField]
	private float tileHeight;

	[Header("The current offset pattern of the game board.")]
	[SerializeField]
	private HexMath.OffsetType offsetType;

	#endregion

	#region Public Interface

	/// <summary>
	/// The dictionary of all tiles whose key is a vector3 defined by offset tile positions (row, vertical height, columns)
	/// </summary>
	public Dictionary<Vector3, Tile> tileDictionary { get; private set; }

	public int MaxRows { get; private set; }
	public int MinRows { get; private set; }
	public int MaxColumns { get; private set; }
	public int MinColumns { get; private set; }
	public int MaxHeight { get; private set; }
	public int MinHeight { get; private set; }

	public float PointRadius { get { return pointRadius; } }
	public float TileHeight { get { return tileHeight; } }

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

	public Tile FindTileUnderMouse()
	{
		Tile tileHit = null;

		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 direction = ray.direction;
		Vector3 camRotation = Camera.main.transform.rotation.eulerAngles;

		Func<bool> CheckColumn;
		Func<bool> CheckRow;
		Func<bool> CheckHeight;

		Debug.Log("Camera:");
		Debug.Log("X: " + camRotation.x + ", Y: " + camRotation.y);

		if ( camRotation.x >= 0 && camRotation.x <= 180 )
			CheckHeight = () => position.y >= MinHeight;
		else
			CheckHeight = () => position.y <= MaxHeight;

		if ( camRotation.y >= 0 && camRotation.y < 180 )
			CheckColumn = () => position.x <= MaxColumns;
		else
			CheckColumn = () => position.x >= MinColumns;

		if ( camRotation.y >= 90 && camRotation.y < 270 )
			CheckRow = () => position.z >= MinRows;
		else
			CheckRow = () => position.z <= MaxRows;

		//Check to see if we start inside a tile (we shouldnt though)
		tileHit = CheckForTile(position);

		while ( tileHit == null && CheckColumn() && CheckRow() && CheckHeight() )
		{
			position += direction * tileHeight;
			tileHit = CheckForTile(position);
		}

		return tileHit;
	}

	public Tile CheckForTile(Vector3 position)
	{
		Tile tileHit = null;

		HexMath.Offset offset = HexMath.OffsetRound(new Vector2(position.x, position.z), pointRadius, offsetType);
		int height = Mathf.FloorToInt(position.y / tileHeight);

		tileDictionary.TryGetValue(new Vector3(offset.x, height, offset.y), out tileHit);

		return tileHit;
	}

    public List<Vector3> FindPath(Vector3 start, int movement)
    {
        HexMath.Offset startO = HexMath.OffsetRound(new Vector2(start.x, start.z), pointRadius, offsetType);

        List<Vector3> vistited = new List<Vector3>();
        vistited.Add(start);

        List<Vector3> fringes = new List<Vector3>();
        fringes.Add(start);

        //Func<HexMath.Offset, bool> = (offset) =>  

        for (int i = 1; i <= movement; i++)
        {
            List<HexMath.Offset> fringes = HexMath.GetOffsetsInRange(startO, CurrentlySelectedCharacter.Movement, offsetType);


        }

    }

	//public Tile FindTileUnderMouse()
	//{
	//	Tile tileHit = null;
	//	RaycastHit hit;
	//	Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
	//	if ( Physics.Raycast(ray, out hit) )
	//	{
	//		HexMath.Offset offset = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), pointRadius, OffsetType );
	//		Vector3 position = new Vector3( offset.x, 0f, offset.y );
	//		Debug.Log(tileDictionary.ContainsKey(position));
	//		tileDictionary.TryGetValue(position, out tileHit);
	//	}

	//	return tileHit;
	//}

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

		if ( tiles != null )
		{

			Vector3 pos = tiles[0].transform.position;
			HexMath.Offset offset = HexMath.OffsetRound( new Vector2( pos.x, pos.z ), pointRadius, offsetType );
			int height = (int)(pos.y / tileHeight);

			MaxRows = MinRows = offset.y;
			MaxColumns = MinColumns = offset.x;
			MaxHeight = MinHeight = height;

			foreach ( Tile tile in tiles )
			{
				pos = tile.transform.position;
				offset = HexMath.OffsetRound(new Vector2(pos.x, pos.z), pointRadius, offsetType);
				height = Mathf.FloorToInt(pos.y / tileHeight);
				Vector3 key = new Vector3( offset.x, height, offset.y );
				if ( !tileDictionary.ContainsKey(key) )
				{
					tileDictionary.Add(key, tile);
					CheckRowColHeight(offset, height);
				}
				else
					Destroy(tile.gameObject);
			}
		}

		Debug.Log("Height: " + MinHeight + ", " + MaxHeight + "| Column: " + MinColumns + ", " + MaxColumns + "| Rows: " + MinRows + ", " + MaxRows);
	}

	private void CheckRowColHeight(HexMath.Offset offset, int height)
	{
		if ( offset.x > MaxColumns )
			MaxColumns = offset.x;
        else if ( offset.x < MinColumns )
			MinColumns = offset.x;

		if ( offset.y > MaxRows )
			MaxRows = offset.y;
		else if ( offset.y < MinRows )
			MinRows = offset.y;

		if ( height > MaxHeight )
			MaxHeight = height;
		else if ( height < MinHeight )
			MinHeight = height;
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
			tileHit = levelfsm.FindTileUnderMouse();
			Debug.Log(tileHit);
			Debug.Log("Offset; " + HexMath.OffsetRound(new Vector2(tileHit.transform.position.x, tileHit.transform.position.z), levelfsm.PointRadius, levelfsm.OffsetType) + "| Height: " + Mathf.FloorToInt(tileHit.transform.position.y / levelfsm.TileHeight));
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
    private int selectedTileHeight;

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
            int height = Mathf.FloorToInt(tileHit.transform.position.y / levelfsm.TileHeight);

            int heightDiff = height - selectedTileHeight;

			// If the offset coordinate we hit is not the same as the last one and within the characters range, redraw the tiles in between the two locations
			if ( offsetHit != lastCoord && heightDiff < levelfsm.CurrentlySelectedCharacter.Height && HexMath.OffsetDistance(selectedTileCoords, offsetHit, levelfsm.OffsetType) <= levelfsm.CurrentlySelectedCharacter.Movement )
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
					levelfsm.Transition( "MoveCharacter" ); 
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
			levelfsm.moveTileCoords = HexMath.GetOffsetsInLine(selectedTileCoords, lastCoord, levelfsm.OffsetType);



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
		HexMath.Offset offset = HexMath.OffsetRound(new Vector2(currentTarget.x, currentTarget.z), levelfsm.PointRadius, levelfsm.OffsetType);
		levelfsm.tileDictionary[new Vector3(offset.x, 0f, offset.y)].OccupiedBy = levelfsm.CurrentlySelectedCharacter;
	}

	#endregion
}