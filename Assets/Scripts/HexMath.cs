using UnityEngine;
using System.Collections;

public class HexMath {

	/* 
		This class is for use with hexagonal playfields.
		Terms:
		-- Column is Q
		-- Row is R
	*/

	#region Helper Classes

	public class Cube
	{
		public float x, y, z;

		public Cube ()
		{
			x = y = z = 0;
		}

		public Cube (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ", " + z + ")";
		}
	}

	public class Axial
	{
		public float q, r;

		public Axial ()
		{
			q = r = 0;
		}

		public Axial (float q, float r)
		{
			this.q = q;
			this.r = r;
		}

		public override string ToString()
		{
			return "(" + q + ", " + r + ")";
		}
	}

	#endregion

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

	public enum OffsetType
	{
		OddR,
		EvenR,
		OddQ,
		EvenQ
	}

	#endregion

	#region Dimensions

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

	public static float CenterToInternalEdge(float pointToPointWidth)
	{
		return pointToPointWidth * .25f;
	}

	#endregion

	#region Conversions

	public static Vector2 CubeToEvenQ(Cube cube)
	{
		Vector2 evenQ;

		evenQ.x = cube.x;
		evenQ.y = cube.z + (cube.x + (cube.x % 2)) / 2;

		return evenQ;
	}

	public static Cube EvenQToCube(Vector2 evenQ)
	{
		Cube cube = new Cube();

		cube.x = evenQ.x;
		cube.z = evenQ.y - (evenQ.x + (evenQ.x % 2)) / 2;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Vector2 CubeToOddQ(Cube cube)
	{
		Vector2 oddQ;

		oddQ.x = cube.x;
		oddQ.y = cube.z + (cube.x - (cube.x % 2)) / 2;

		return oddQ;
	}

	public static Cube OddQToCube(Vector2 oddQ)
	{
		Cube cube = new Cube();

		cube.x = oddQ.x;
		cube.z = oddQ.y - (oddQ.x - (oddQ.x % 2)) / 2;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Vector2 CubeToEvenR(Cube cube)
	{
		Vector2 evenR;

		evenR.x = cube.x + (cube.z + (cube.z % 2)) / 2;
		evenR.y = cube.z;

		return evenR;
	}

	public static Cube EvenRToCube(Vector2 evenR)
	{
		Cube cube = new Cube();

		cube.x = evenR.x - (evenR.y + (evenR.y % 2)) / 2;
		cube.z = evenR.y;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Vector2 CubeToOddR(Cube cube)
	{
		Vector2 oddR;

		oddR.x = cube.x + (cube.z - (cube.z % 2)) / 2;
		oddR.y = cube.z;

		return oddR;
	}

	public static Cube OddRToCube(Vector2 oddR)
	{
		Cube cube = new Cube();

		cube.x = oddR.x - (oddR.y - (oddR.y % 2)) / 2;
		cube.z = oddR.y;
		cube.y = - cube.x - cube.z;

		return cube;
	}

	public static Axial CubeToAxial(Cube cube)
	{
		return new Axial(cube.x, cube.z);
	}

	public static Cube AxialToCube(Axial axial)
	{
		return new Cube(axial.q, -axial.q - axial.r, axial.r);
	}

	public static Axial WorldToAxial(Vector2 point, float pointRadius, OffsetType type)
	{
		float q;
		float r;

		// Pointy top
		if ( type == OffsetType.EvenR || type == OffsetType.OddR )
		{
			q = ((point.x * (Mathf.Sqrt(3.0f) / 3.0f)) - (point.y / 3.0f)) / pointRadius;
			r = point.y * 2.0f/3.0f / pointRadius;
		}
		// Flat top
		else
		{
			q = point.x * 2.0f/3.0f / pointRadius;
			r = (-point.x / 3.0f + Mathf.Sqrt(3.0f) / 3.0f * point.y) / pointRadius;
		}

		return new Axial(q, r);
	}

	public static Vector2 OffsetRound(Vector2 point, float pointRadius, OffsetType type)
	{
		if (type == OffsetType.EvenQ)
			return HexMath.CubeToEvenQ(HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(point.x, point.y), .5f, type))));
		else if (type == OffsetType.EvenR)
			return HexMath.CubeToEvenR(HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(point.x, point.y), .5f, type))));
		else if (type == OffsetType.OddQ)
			return HexMath.CubeToOddQ(HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(point.x, point.y), .5f, type))));
		else
			return HexMath.CubeToOddR(HexMath.CubeRound(HexMath.AxialToCube(HexMath.WorldToAxial(new Vector2(point.x, point.y), .5f, type))));
	}

	public static Cube CubeRound(Cube cubePoint)
	{
		float rx = Mathf.Round(cubePoint.x);
		float ry = Mathf.Round(cubePoint.y);
		float rz = Mathf.Round(cubePoint.z);

		float x_diff = Mathf.Abs(rx - cubePoint.x);
		float y_diff = Mathf.Abs(ry - cubePoint.y);
		float z_diff = Mathf.Abs(rz - cubePoint.z);

		if ( x_diff > y_diff && x_diff > z_diff )
			rx = -ry - rz;
		else if ( y_diff > z_diff )
			ry = -rx - rz;
		else
			rz = -rx - ry;

		return new Cube(rx, ry, rz);
	}

	public static Vector2 OffsetToWorld(Vector2 offset, float pointRadius, OffsetType type)
	{
		float x;
		float y;

		if (type == OffsetType.EvenQ)
		{
			x = pointRadius * 3.0f / 2.0f * offset.x;
			y = pointRadius * Mathf.Sqrt(3) * (offset.y - .5f * (offset.x % 2));
		}
		else if (type == OffsetType.EvenR)
		{
			x = pointRadius * Mathf.Sqrt(3) * (offset.x - .5f * (offset.y % 2));
			y = pointRadius * 3.0f / 2.0f * offset.y;
		}
		else if (type == OffsetType.OddQ)
		{
			x = pointRadius * 3.0f / 2.0f * offset.x;
			y = pointRadius * Mathf.Sqrt(3) * (offset.y + .5f * (offset.x % 2));
		}
		//OddR
		else
		{
			x = pointRadius * Mathf.Sqrt(3) * (offset.x + .5f * (offset.y % 2));
			y = pointRadius * 3.0f / 2.0f * offset.y;
		}

		return new Vector2(x, y);
	}

	#endregion

	#region Others

	public static float CubeDistance(Cube a, Cube b)
	{
		return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
	}

	#endregion


}
