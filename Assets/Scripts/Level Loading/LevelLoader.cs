using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class LevelLoader : MonoBehaviour {

	#region Editor Interface

	public GameObject TileContainer
	{
		get { return tileContainer; }
		#if UNITY_EDITOR
		set { tileContainer = value; }
		#endif
	}
	[SerializeField]
	private GameObject tileContainer;

	public HexMath.OffsetType Offset
	{
		get { return offset; }
		#if UNITY_EDITOR
		set { offset = value; }
		#endif
	}
	[SerializeField] private HexMath.OffsetType offset;

	public float TilePointRadius
	{
		get { return tilePointRadius; }
		#if UNITY_EDITOR
		set { tilePointRadius = value; }
		#endif
	}
	[SerializeField] private float tilePointRadius;

    public int NumberRows
    {
        get { return numberRows; }
        #if UNITY_EDITOR
        set { numberRows = value; }
        #endif
    }
    [SerializeField]
    private int numberRows;

    public bool GoFromBottom
    {
        get { return goFromBottom; }
        #if UNITY_EDITOR
        set { goFromBottom = value; }
        #endif
    }
    [SerializeField]
    private bool goFromBottom;

	public float TileHeight
	{
		get { return tileHeight; }
		#if UNITY_EDITOR
		set { tileHeight = value; }
		#endif
	}
	[SerializeField] private float tileHeight;

	public string FileName
	{
		get { return fileName; }
		#if UNITY_EDITOR
		set { fileName = value; }
		#endif
	}
	[SerializeField] private string fileName;

	public bool isTileMapExpanded;

    public string EmptyAlias
    {
        get { return emptyAlias; }
        #if UNITY_EDITOR
        set { emptyAlias = value; }
        #endif
    }
    [SerializeField]
    private string emptyAlias;

	public List<string> TileAlias
	{
		get { return tileAlias; }
		#if UNITY_EDITOR
		set { tileAlias = value; }
		#endif
	}
	[SerializeField] private List<string> tileAlias = new List<string>();

	public List<Tile> TilePrefabs
	{
		get { return tilePrefabs; }
		#if UNITY_EDITOR
		set { tilePrefabs = value; }
		#endif
	}
	[SerializeField] private List<Tile> tilePrefabs = new List<Tile>();

	#endregion

	#region Public Interface

	public static LevelLoader Instance {get {return instance;}}
	private static LevelLoader instance;

	/// <summary>
	/// Returns a dictionary that contains the offset coordinates of 
	/// tiles as keys and the string representation of what tile they
	/// are as values
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public Dictionary<Vector3, string> GetLevelData(string fileName)
	{
        Dictionary<Vector3, string> tiles = new Dictionary<Vector3, string>();

        if (goFromBottom)
        {
            try
            {
                int row = numberRows - 1;
                int col = 0;
                int height = 0;

                string line;

                StreamReader file = new StreamReader("Assets/Resources/" + fileName + ".dat");

                while ((line = file.ReadLine()) != null)
                {
                    string[] lineEntries = line.Split(' ');

                    if (lineEntries[0] == "Level:")
                    {
                        height = int.Parse(lineEntries[1]);
                        row = numberRows - 1;
                        continue;
                    }

                    col = 0;

                    foreach (string entry in lineEntries)
                    {
                        tiles.Add(new Vector3(col, row, height), entry);
                        col++;
                    }

                    row--;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }

            return tiles;
        }

        else
        {
            try
            {
                int row = 0;
                int col = 0;
                int height = 0;

                string line;

                StreamReader file = new StreamReader("Assets/Resources/" + fileName + ".dat");

                while ((line = file.ReadLine()) != null)
                {
                    string[] lineEntries = line.Split(' ');

                    if (lineEntries[0] == "Level:")
                    {
                        height = int.Parse(lineEntries[1]);
                        row = 0;
                        continue;
                    }

                    col = 0;

                    foreach (string entry in lineEntries)
                    {
                        tiles.Add(new Vector3(col, row, height), entry);
                        col++;
                    }

                    row++;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }

            return tiles;
        }
	}

	public void LoadLevelTiles(Dictionary<Vector3, string> tiles)
	{
		DestroyImmediate(tileContainer);
		tileContainer = GameObject.Instantiate(new GameObject("Tile Container"), Vector3.zero, Quaternion.identity) as GameObject;
		tileContainer.transform.parent = transform;

		Dictionary<string, Tile> tileMap = new Dictionary<string,Tile>();

		for(int i = 0; i < tileAlias.Count; i++)
		{
			tileMap.Add(tileAlias[i], tilePrefabs[i]);
		}

        Quaternion rotation = Quaternion.identity;
        if (offset == HexMath.OffsetType.EvenR || offset == HexMath.OffsetType.OddR)
        {
            rotation *= Quaternion.Euler(0f, 30f, 0f);
        }

		foreach (var item in tiles)
		{
            if (tileMap.ContainsKey(item.Value))
            {
                Vector2 position = HexMath.OffsetToWorld(new Vector2(item.Key.x, item.Key.y), tilePointRadius, offset);
                float height = item.Key.z * tileHeight;
                Tile newTile = GameObject.Instantiate(tileMap[item.Value], new Vector3(position.x, height, position.y), rotation) as Tile;
                newTile.transform.parent = tileContainer.transform;
            }
		}
	}

	#endregion

	#region Mono Methods

	private void Awake()
	{
		instance = this;
	}

	#endregion
}
