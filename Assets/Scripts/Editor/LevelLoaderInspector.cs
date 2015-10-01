using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(LevelLoader), true)]
public class LevelLoaderInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		LevelLoader ll = (LevelLoader)target;

		ll.TileContainer = EditorGUILayout.ObjectField(ll.TileContainer, typeof(GameObject)) as GameObject;

		string fileName = EditorGUILayout.TextField(ll.FileName);

		ll.TilePointRadius = EditorGUILayout.FloatField(ll.TilePointRadius);

		ll.TileHeight = EditorGUILayout.FloatField(ll.TileHeight);

		ll.Offset = (HexMath.OffsetType)EditorGUILayout.EnumPopup(ll.Offset);

		if (GUILayout.Button("Load Level"))
		{
			LevelLoader.LoadLevelTiles(LevelLoader.GetLevelData(fileName));
		}

		if (GUILayout.Button("Add Tile Alias"))
		{
			ll.TileAlias.Add("");
			ll.TilePrefabs.Add(null);
		}

		EditorGUI.indentLevel++;

		for (int i = 0; i < ll.TileAlias.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("X"))
			{
				ll.TileAlias.RemoveAt(i);
				ll.TilePrefabs.RemoveAt(i);
			}

			ll.TileAlias[i] = EditorGUILayout.TextField(ll.TileAlias[i]);
			ll.TilePrefabs[i] = EditorGUILayout.ObjectField(ll.TilePrefabs[i], typeof(Tile)) as Tile;

			EditorGUILayout.EndHorizontal();
		}

		EditorGUI.indentLevel--;
	}
}
