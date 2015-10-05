using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelFSM : MonoFSM
{

    #region Editor Interface

    [SerializeField] private Transform TileMaster;

    [SerializeField] private Tile selectionTilePrefab;

    #endregion

    #region Public Interface

    public Dictionary<Vector3, Tile> tileDictionary { get; private set; }

    public Tile CurrentTile;

    public Character CurrentCharacter;

    public Transform LineContainer;

    public Tile SelectionTilePrefab { get { return selectionTilePrefab; } }

    #endregion

    #region Overrides

    protected override void SetStates()
	{
		states = new List<IState>
		{
			new ListenForInputState()
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

		Component[] tiles = TileMaster.GetComponentsInChildren(typeof (Tile));

		foreach (Tile tile in tiles )
		{
			Vector3 pos = tile.transform.position;
			HexMath.Offset offset = HexMath.OffsetRound(new Vector2(pos.x, pos.z), .5f, HexMath.OffsetType.OddR);
			int height = (int)(pos.y / .2f);
			Vector3 key = new Vector3(offset.x, height, offset.y);
			if ( !tileDictionary.ContainsKey(key) )
				tileDictionary.Add(key, tile);
			else
				Destroy(tile.gameObject);
		}
    }

    #endregion
}

public class ListenForInputState : IState
{
    #region Fields

    private LevelFSM levelfsm;

    #endregion

    #region IState Overrides

    public void Initialize(MonoFSM callingfsm)
    {
        levelfsm = (LevelFSM)callingfsm;
    }

    public void OnEnter() { }

    public void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                HexMath.Offset offset = HexMath.OffsetRound(new Vector2(hit.point.x, hit.point.z), .5f, HexMath.OffsetType.OddR);
                Vector3 position = new Vector3(offset.x, 0f, offset.y);
                Debug.Log(levelfsm.tileDictionary.ContainsKey(position));
            }
        }
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnExit() { }

    public void CheckTransitions() { }

    #endregion
}

public class MoveCharacterState : IState
{
    #region Fields

    private LevelFSM levelfsm;

    private Tile lastTile;
    private HexMath.Cube lastCube;

    private HexMath.Cube currentTileCube;

    #endregion

    #region IState Overrides

    public void Initialize(MonoFSM callingfsm)
    {
        levelfsm = (LevelFSM)callingfsm;
    }

    public void OnEnter() 
    {
        lastTile = levelfsm.CurrentTile;
        currentTileCube = HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(levelfsm.CurrentTile.transform.position, .5f, HexMath.OffsetType.OddR)));
    }

    public void OnUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            HexMath.Cube cubeHit = HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(hit.point.x, hit.point.z), .5f, HexMath.OffsetType.OddR)));
            
            if (cubeHit != lastCube && HexMath.CubeDistance(currentTileCube, cubeHit) <= levelfsm.CurrentCharacter.Movement)
            {
                if (levelfsm.LineContainer != null)
                    GameObject.Destroy(levelfsm.LineContainer);

                levelfsm.LineContainer = new GameObject("Line Continer").GetComponent<Transform>();

                List<HexMath.Cube> hexes = HexMath.GetHexInLine(currentTileCube, cubeHit);


                foreach (var cube in hexes)
                {
                    HexMath.Offset offset = HexMath.CubeToOddR(cube);
                    Vector2 world = HexMath.OffsetToWorld(offset, .5f, HexMath.OffsetType.OddR);
                    Tile t = (Tile)GameObject.Instantiate(levelfsm.SelectionTilePrefab, new Vector3(world.x, 0.01f, world.y), Quaternion.Euler(0, 30f, 0));
                    t.transform.parent = levelfsm.LineContainer;
                }
            }
        }
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnExit() { }

    public void CheckTransitions() { }

    #endregion
}