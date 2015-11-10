using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(MonoFSM), true)]
public class MonoFSMEditor : Editor {

	public override void OnInspectorGUI()
	{
		MonoFSM fsm = (MonoFSM)target;

		fsm.StateEnumName = EditorGUILayout.TextField( "State Enum Name", fsm.StateEnumName );

		Type enumType = Type.GetType( fsm.StateEnumName + ",Assembly-CSharp" );

		if ( enumType != null )
		{
			fsm.IsStatesExpanded = EditorGUILayout.Foldout( fsm.IsStatesExpanded, "States" );

			string[] enumNames = Enum.GetNames( enumType );

			if ( fsm.IsStatesExpanded )
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.BeginHorizontal();
				{
					if ( GUILayout.Button( "Add State" ) )
					{
						fsm.StateKeys.Add( 0 );
						fsm.StateValues.Add( null );
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField( "Enum Key" );
					EditorGUILayout.LabelField( "State" );
				}
				EditorGUILayout.EndHorizontal();

				for ( int i = 0; i < fsm.StateKeys.Count; i++ )
				{
					EditorGUILayout.BeginHorizontal();
					{
						if ( GUILayout.Button( "X" ) )
						{
							fsm.StateKeys.RemoveAt( i );
							fsm.StateValues.RemoveAt( i );
							continue;
						}

						fsm.StateKeys[i] = EditorGUILayout.Popup( fsm.StateKeys[i], enumNames );
						State state = (State)EditorGUILayout.ObjectField( fsm.StateValues[i], typeof( State ), true );
						if ( state != null )
						{
							Type stateType = state.GetType();
							bool contains = false;
							for ( int j = 0; j < fsm.StateValues.Count; j++ )
							{
								if ( i != j && fsm.StateValues[j] != null )
									if ( fsm.StateValues[j].GetType() == stateType )
										contains = true;
							}
							if ( contains == false )
								fsm.StateValues[i] = state;
						}
						else
							fsm.StateValues[i] = null;
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();

			fsm.IsTransitionsExpanded = EditorGUILayout.Foldout( fsm.IsTransitionsExpanded, "Transitions" );

			if ( fsm.IsTransitionsExpanded )
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.BeginHorizontal();
				{
					if ( GUILayout.Button( "Add Transition" ) )
					{
						fsm.ValidTransitions.Add( new FSMTransition( 0, 0 ) );
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField( "From" );
					EditorGUILayout.LabelField( "To" );
				}
				EditorGUILayout.EndHorizontal();

				if ( enumType != null )
				{
					for ( int i = 0; i < fsm.ValidTransitions.Count; i++ )
					{
						EditorGUILayout.BeginHorizontal();
						{
							if ( GUILayout.Button( "X" ) )
							{
								fsm.ValidTransitions.RemoveAt( i );
								continue;
							}

							fsm.ValidTransitions[i].From = EditorGUILayout.Popup( fsm.ValidTransitions[i].From, enumNames );
							fsm.ValidTransitions[i].To = EditorGUILayout.Popup( fsm.ValidTransitions[i].To, enumNames );
						}
						EditorGUILayout.EndHorizontal();
					}
				}

				EditorGUI.indentLevel--;
			}
		}
		else
		{
			EditorGUILayout.LabelField( "There is no found Enum of type '" + fsm.StateEnumName + "'." );
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		base.OnInspectorGUI();
	}
}
