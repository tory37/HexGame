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

				for ( int i = 0; i < fsm.StateKeys.Count; i++ )
				{
					string stateLabel = "EMPTY STATE / NULL";

					if ( fsm.StateValues[i] != null )
						stateLabel = fsm.StateValues[i].Identifier;

					bool isStateExpanded = true;
					EditorGUILayout.BeginHorizontal();
					{
						if ( fsm.StateValues[i] == null )
							isStateExpanded = true;
						else
						{
							isStateExpanded = fsm.StateValues[i].IsStateExpanded;
							fsm.StateValues[i].IsStateExpanded = EditorGUILayout.Foldout( fsm.StateValues[i].IsStateExpanded, stateLabel );
						}
						if ( GUILayout.Button( "X", GUILayout.Width(20f), GUILayout.Height(20f) ) )
						{
							fsm.StateKeys.RemoveAt( i );
							fsm.StateValues.RemoveAt( i );
							i--;
							continue;
						}
					}
					EditorGUILayout.EndHorizontal();

					if ( isStateExpanded )
					{
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
				}

				EditorGUI.indentLevel--;
			}

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
							fsm.ValidTransitions[i].From = EditorGUILayout.Popup( fsm.ValidTransitions[i].From, enumNames );
							fsm.ValidTransitions[i].To = EditorGUILayout.Popup( fsm.ValidTransitions[i].To, enumNames );
						
							if ( GUILayout.Button( "X", GUILayout.Width( 20f ), GUILayout.Height( 20f ) ) )
							{
								fsm.ValidTransitions.RemoveAt( i );
								i--;
								continue;
							}
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

		fsm.IsChildValuesExpanded = EditorGUILayout.Foldout( fsm.IsChildValuesExpanded, "Child Values" );

		if ( fsm.IsChildValuesExpanded )
		{
			EditorGUI.indentLevel++;

			base.OnInspectorGUI();

			EditorGUI.indentLevel--;
		}
	}
}
