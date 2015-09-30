using UnityEngine;
using System.Collections;

public class HexMath {

	#region Enums

	public enum EdgeDimension
	{
		EdgeWidth,
		EdgeRadius
	}

	public enum PointDimension
	{
		PointWidth,
		PointRadius,
	}

	public enum GridLayout
	{
		PointyTop,
		FlatTop
	}

	#endregion

	#region Static Functions

	/// <summary>
	/// Returns the distance from a corner to the center of a hexagon.
	/// </summary>
	/// <param name="dimension">The width from flat edge to flat edge or 
	///		the radius from center to flat edge of the hexagon </param>
	/// <param name="dimensionType">The enum to tell the function whether
	///		/dimension/ is the flat edge to flat edge width or radius </param>
	/// <returns></returns>
	public static float PointToCenterRadius(float dimension, EdgeDimension dimensionType)
	{
		float edgeRadius = dimension;

		if ( dimensionType == EdgeDimension.EdgeWidth )
			edgeRadius = dimension * .5f;
		else
			edgeRadius = dimension;

		return (2 * edgeRadius) / Mathf.Sqrt(3);
	}

	/// <summary>
	/// Returns the distance between opposite corners of a hexagon.
	/// </summary>
	/// <param name="dimension">The width from flat edge to flat edge or 
	///		the radius from center to flat edge of the hexagon</param>
	/// <param name="dimensionType">The enum to tell the function whether
	///		/dimension/ is the flat edge to flat edge width or radius </param>
	/// <returns></returns>
	public static float PointToPointWidth(float dimension, EdgeDimension dimensionType)
	{
		float edgeRadius = dimension;

		if ( dimensionType == EdgeDimension.EdgeWidth)
				edgeRadius  = dimension * .5f;
		else
				edgeRadius = dimension;

		return (4 * edgeRadius) / Mathf.Sqrt(3);
	}

	/// <summary>
	/// Returns the distance from a flat edge to the center of a hexagon.
	/// </summary>
	/// <param name="dimension">The width from point to opposite point or 
	///		the radius from center to point of the hexagon</param>
	/// <param name="dimensionType">The enum to tell the function whether
	///		/dimension/ is the point to opposite point width or radius </param>
	/// <returns></returns>
	public static float EdgeToCenterRadius(float dimension, PointDimension dimensionType)
	{
		float pointRadius = dimension;

		if ( dimensionType == PointDimension.PointWidth)
				pointRadius  = dimension * .5f;
		else
				pointRadius = dimension;

		return (pointRadius * Mathf.Sqrt( 3 )) / 2;
	}

	/// <summary>
	/// Returns the distance from a flat edge to the opposite flat edge of a hexagon.
	/// </summary>
	/// <param name="dimension">The width from point to opposite point or 
	///		the radius from center to point of the hexagon</param>
	/// <param name="dimensionType">The enum to tell the function whether
	///		/dimension/ is the point to opposite point width or radius </param>
	/// <returns></returns>
	public static float EdgeToEdgeWidth(float dimension, PointDimension dimensionType)
	{
		float pointRadius = dimension;

		if ( dimensionType == PointDimension.PointWidth)
				pointRadius  = dimension * .5f;
		else
				pointRadius = dimension;

		return 2 * ((pointRadius * Mathf.Sqrt(3)) / 2);
	}

	public static float CenterToInternalEdge(float pointWidth)
	{
		return pointWidth * .25f;
	}

	public static Vector2 GetCoordinates (Vector2 point, Vector2 origin, GridLayout layoutType, float edgeToEdgeWidth)
	{
		Vector2 coordinates = Vector2.zero;

		if ( layoutType == GridLayout.PointyTop )
		{
			int column = Mathf.FloorToInt((point.x - origin.x) / edgeToEdgeWidth);
		}
	}


	#endregion
}
