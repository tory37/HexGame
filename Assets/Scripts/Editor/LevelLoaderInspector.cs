using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(LevelLoader), true)]
public class LevelLoaderInspector : Editor
{
	public override void OnInspectorGUI()
	{
		LevelLoader ll = (LevelLoader)target;

		ll.TileContainer = EditorGUILayout.ObjectField("Tile Container", ll.TileContainer, typeof(GameObject), true) as GameObject;

		ll.FileName = EditorGUILayout.TextField("File Name", ll.FileName);

		ll.TilePointRadius = EditorGUILayout.FloatField("Tile Point Radius", ll.TilePointRadius);

        ll.NumberRows = EditorGUILayout.IntField("Number of Rows", ll.NumberRows);

		ll.TileHeight = EditorGUILayout.FloatField("Tile Height", ll.TileHeight);

		ll.Offset = (HexMath.OffsetType)EditorGUILayout.EnumPopup("Offset Type", ll.Offset);

        EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Load Level"))
		{
			ll.LoadLevelTiles(ll.GetLevelData(ll.FileName));
		}

        ll.GoFromBottom = EditorGUILayout.Toggle("Go From Bottom", ll.GoFromBottom);

        EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Add Tile Alias"))
		{
			ll.TileAlias.Add("");
			ll.TilePrefabs.Add(null);
		}

		EditorGUI.indentLevel++;

		ll.isTileMapExpanded = EditorGUILayout.Foldout(ll.isTileMapExpanded, "Tile Alias");

		if (ll.isTileMapExpanded)
		{
            ll.EmptyAlias = EditorGUILayout.TextField(ll.EmptyAlias, "Empty Alias");

			for (int i = 0; i < ll.TileAlias.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("X"))
				{
					ll.TileAlias.RemoveAt(i);
					ll.TilePrefabs.RemoveAt(i);
				}

				ll.TileAlias[i] = EditorGUILayout.TextField(ll.TileAlias[i]);
				ll.TilePrefabs[i] = EditorGUILayout.ObjectField(ll.TilePrefabs[i], typeof(Tile), false) as Tile;

				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUI.indentLevel--;
	}
}
