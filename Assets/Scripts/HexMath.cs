using UnityEngine;
using System.Collections;

public class HexMath {

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

	public enum LayoutType
	{
		PointyTop,
		FlatTop
	}

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

		switch ( dimensionType )
		{
			case EdgeDimension.EdgeWidth:
				edgeRadius  = dimension * .5f;
				break;
			case EdgeDimension.EdgeRadius:
				edgeRadius = dimension;
				break;
		}

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

		switch ( dimensionType )
		{
			case EdgeDimension.EdgeWidth:
				edgeRadius  = dimension * .5f;
				break;
			case EdgeDimension.EdgeRadius:
				edgeRadius = dimension;
				break;
		}

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

		switch ( dimensionType )
		{
			case PointDimension.PointWidth:
				pointRadius  = dimension * .5f;
				break;
			case PointDimension.PointRadius:
				pointRadius = dimension;
				break;
		}

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

		switch ( dimensionType )
		{
			case PointDimension.PointWidth:
				pointRadius  = dimension * .5f;
				break;
			case PointDimension.PointRadius:
				pointRadius = dimension;
				break;
		}

		return 2 * ((pointRadius * Mathf.Sqrt( 3 )) / 2);
	}

	public static int GetRow(float xPoint, Vector3 originPosition, LayoutType layoutType, float edgeToEdgeWidth )
	{
		
	}

	public static int GetColumn()
	{

	}
}
