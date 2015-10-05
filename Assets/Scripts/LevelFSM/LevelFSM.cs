using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelFSM : MonoFSM
{

	#region Editor Interface

	[SerializeField]
	private Transform TileMaster;

	[SerializeField]
	private Tile selectionTilePrefab;

	#endregion

	#region Public Interface

	public Dictionary<Vector3, Tile> tileDictionary { get; private set; }

	public Tile CurrentlySelectedTile;

	[HideInInspector]
	public Character CurrentlySelectedCharacter;

	[HideInInspector]
	public Transform LineContainer;

	[HideInInspector]
	public Tile SelectionTilePrefab { get { return selectionTilePrefab; } }

	#endregion

	#region Shared Fields

	public List<HexMath.Cube> moveTileCoords = null;

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

	protected override void Initialize()
	{
		FillDictionary();
	}

	#endregion

	#region Private

	private void FillDictionary()
	{
		tileDictionary = new Dictionary<Vector3, Tile>();

		Component[] tiles = TileMaster.GetComponentsInChildren( typeof( Tile ) );

		foreach ( Tile tile in tiles )
		{
			Vector3 pos = tile.transform.position;
			HexMath.Offset offset = HexMath.OffsetRound( new Vector2( pos.x, pos.z ), .5f, HexMath.OffsetType.OddR );
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

public class ListenForInputState : State
{
	#region Fields

	private LevelFSM levelfsm;

	private Tile tileHit;

	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	public override void OnEnter()
	{
		tileHit = null;
	}

	public override void OnUpdate()
	{
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			if ( Physics.Raycast( ray, out hit ) )
			{
				HexMath.Offset offset = HexMath.OffsetRound( new Vector2( hit.point.x, hit.point.z ), .5f, HexMath.OffsetType.OddR );
				Vector3 position = new Vector3( offset.x, 0f, offset.y );
				Debug.Log( levelfsm.tileDictionary.ContainsKey( position ) );
				levelfsm.tileDictionary.TryGetValue( position, out tileHit );
			}
		}
	}

	public override void CheckTransitions()
	{
		if ( tileHit != null && tileHit.OccupiedBy != null )
		{
			levelfsm.CurrentlySelectedCharacter = tileHit.OccupiedBy;
			levelfsm.CurrentlySelectedTile = tileHit;
			levelfsm.Transition( "SelectCharacterTargetTileState" );
		}
	}

	#endregion
}

public class SelectCharacterTargetTileState : State
{
	#region Fields

	private LevelFSM levelfsm;

	private Tile selectedTile;
	private HexMath.Cube lastCube;

	private HexMath.Cube selectedTileCubeCoords;

	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	public override void OnEnter()
	{
		selectedTile = levelfsm.CurrentlySelectedTile;
		selectedTileCubeCoords = HexMath.RoundWorldToCube( new Vector2( selectedTile.transform.position.x, selectedTile.transform.position.z ), .5f, HexMath.OffsetType.OddR );
		levelfsm.moveTileCoords = new List<HexMath.Cube>();
	}

	public override void OnUpdate()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		if ( Physics.Raycast( ray, out hit ) )
		{
			HexMath.Cube cubeHit = HexMath.RoundWorldToCube( new Vector2( hit.point.x, hit.point.z ), .5f, HexMath.OffsetType.OddR );

			if ( cubeHit != lastCube && HexMath.CubeDistance( selectedTileCubeCoords, cubeHit ) <= levelfsm.CurrentlySelectedCharacter.Movement )
			{
				lastCube = cubeHit;

				if ( levelfsm.LineContainer != null )
					GameObject.Destroy( levelfsm.LineContainer.gameObject );

				levelfsm.LineContainer = new GameObject( "Line Continer" ).GetComponent<Transform>();

				levelfsm.moveTileCoords = HexMath.GetHexInLine( selectedTileCubeCoords, cubeHit );

				foreach ( var cube in levelfsm.moveTileCoords )
				{
					HexMath.Offset offset = HexMath.CubeToOddR( cube );
					Vector2 world = HexMath.OffsetToWorld( offset, .5f, HexMath.OffsetType.OddR );
					Tile t = (Tile)GameObject.Instantiate( levelfsm.SelectionTilePrefab, new Vector3( world.x, 0.2f, world.y ), Quaternion.Euler( 0, 30f, 0 ) );
					t.transform.parent = levelfsm.LineContainer;
				}
			}
		}
	}

	public override void CheckTransitions()
	{
		//If the user clicks a valid tile
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			if ( Physics.Raycast( ray, out hit ) )
			{
				HexMath.Cube hitCube = HexMath.RoundWorldToCube( new Vector2( hit.point.x, hit.point.z ), .5f, HexMath.OffsetType.OddR );

				if ( HexMath.CubeDistance( selectedTileCubeCoords, hitCube ) <= levelfsm.CurrentlySelectedCharacter.Movement )
				{
					GameObject.Destroy( levelfsm.LineContainer.gameObject );
					levelfsm.Transition( "MoveCharacter" ); 
				}
			}
		}
	}

	#endregion
}

public class MoveCharacter : State
{
	#region Fields

	private LevelFSM levelfsm;

	private List<Vector3> tilePositions;

	private Vector3 currentTarget;

	private bool finishedMoving = false;

	#endregion

	#region State Overrides

	public override void Initialize(MonoFSM callingfsm)
	{
		levelfsm = (LevelFSM)callingfsm;
	}

	public override void OnEnter()
	{
		finishedMoving = false;

		tilePositions = new List<Vector3>();

		foreach ( HexMath.Cube cube in levelfsm.moveTileCoords )
		{
			Vector2 world = HexMath.CubeToWorld( cube, .5f, HexMath.OffsetType.OddR );
			tilePositions.Add( new Vector3( world.x, 0f, world.y ) );
		}

		currentTarget = tilePositions[1];
	}

	public override void OnUpdate()
	{
		Character cha = levelfsm.CurrentlySelectedCharacter;
		float deltaMove = cha.MoveAnimSpeed * Time.deltaTime;
		Vector3 direction = currentTarget - cha.transform.position;
		deltaMove = Mathf.Clamp(deltaMove, 0f, direction.magnitude);
		cha.transform.position += direction.normalized * deltaMove;

		if ( Mathf.Abs(cha.transform.position.sqrMagnitude - currentTarget.sqrMagnitude) < .1f )
		{
			int index = tilePositions.IndexOf( currentTarget );

			if ( index < tilePositions.Count - 1 )
				currentTarget = tilePositions[index + 1];
			else
			{
				Debug.Log( "Done Moving" );
				finishedMoving = true;
			}
		}
	}

	public override void CheckTransitions()
	{
		if ( finishedMoving == true )
			levelfsm.Transition( "ListenForInput" );
	}

	public override void OnExit()
	{
		// Reset the coordinate list
		levelfsm.moveTileCoords = null;

		// Remove the character from the old tile
		levelfsm.CurrentlySelectedTile.OccupiedBy = null;

		// Add the character to the new tile
		HexMath.Offset offset = HexMath.OffsetRound(new Vector2(currentTarget.x, currentTarget.z), .5f, HexMath.OffsetType.OddR);
		levelfsm.tileDictionary[new Vector3(offset.x, 0f, offset.y)].OccupiedBy = levelfsm.CurrentlySelectedCharacter;
	}

	#endregion
}