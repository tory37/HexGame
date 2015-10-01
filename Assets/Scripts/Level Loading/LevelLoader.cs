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

	public List<string> TileAlias
	{
		get { return tileAlias; }
		#if UNITY_EDITOR
		set { tileAlias = value; }
		#endif
	}
	[SerializeField] private List<string> tileAlias;

	public List<Tile> TilePrefabs
	{
		get { return tilePrefabs; }
		#if UNITY_EDITOR
		set { tilePrefabs = value; }
		#endif
	}
	[SerializeField] private List<Tile> tilePrefabs;

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
	public static Dictionary<Vector3, string> GetLevelData(string fileName)
	{
		Dictionary<Vector3, string> tiles = new Dictionary<Vector3,string>();
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
					col = 0;
					continue;
				}

				foreach (string entry in lineEntries)
				{
					tiles.Add(new Vector3(row, col, height), entry);
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

	public static void LoadLevelTiles(Dictionary<Vector3, string> tiles)
	{
		Destroy(Instance.TileContainer);
		Instance.TileContainer = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity) as GameObject;

		Dictionary<string, Tile> tileMap = new Dictionary<string,Tile>();

		for(int i = 0; i < Instance.tileAlias.Count; i++)
		{
			tileMap.Add(Instance.tileAlias[i], Instance.TilePrefabs[i]);
		}

		foreach (var item in tiles)
		{
			Vector2 position = HexMath.OffsetToWorld(new Vector2(item.Key.x, item.Key.y), Instance.TilePointRadius, Instance.Offset);
			float height = item.Key.z * Instance.TileHeight;
			Tile newTile = GameObject.Instantiate(tileMap[item.Value], new Vector3(position.x, height, position.y), Quaternion.identity) as Tile;
			newTile.transform.parent = Instance.TileContainer.transform;
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
