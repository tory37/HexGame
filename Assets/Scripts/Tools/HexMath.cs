using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		public int x, y, z;

		public Cube ()
		{
			x = y = z = 0;
		}

		public Cube (int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return "Cube: (" + x + ", " + y + ", " + z + ")";
		}

        public override bool Equals(object obj)
        {
            if (obj is Cube)
            {
                Cube oth = (Cube)obj;
                return x == oth.x && y == oth.y && z == oth.z;
            }
            return false;
        }

		public static bool operator ==(Cube left, Cube right)
		{
			try
			{
				return left.x == right.x && left.y == right.y && left.z == right.z;
			}
			catch
			{
				return false;
			}
		}

		public static bool operator != (Cube left, Cube right)
		{
			try
			{
				return !(left.x == right.x && left.y == right.y && left.z == right.z);
			}
			catch
			{
				return true;
			}
		}
	}

	public class CubeFraction
	{
		public float x, y, z;

		public CubeFraction ()
		{
			x = y = z = 0;
		}

		public CubeFraction (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return "Cube: (" + x + ", " + y + ", " + z + ")";
		}
	}

	public class Axial
	{
		public int q, r;

		public Axial ()
		{
			q = r = 0;
		}

		public Axial (int q, int r)
		{
			this.q = q;
			this.r = r;
		}

		public override string ToString()
		{
			return "Axial: (" + q + ", " + r + ")";
		}
	}

	public class AxialFraction
	{
		public float q, r;

		public AxialFraction ()
		{
			q = r = 0;
		}

		public AxialFraction (float q, float r)
		{
			this.q = q;
			this.r = r;
		}

		public override string ToString()
		{
			return "Axial: (" + q + ", " + r + ")";
		}
	}

	public class Offset
	{
		public int x, y;

		public Offset ()
		{
			x = y = 0;
		}

		public Offset(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return "Offset: (" + x + ", " + y + ")";  
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

	public static Offset CubeToEvenQ(Cube cube)
	{
		Offset evenQ = new Offset();

		evenQ.x = cube.x;
		evenQ.y = cube.z + (cube.x + (cube.x % 2)) / 2;

		return evenQ;
	}

	public static Cube EvenQToCube(Offset evenQ)
	{
		Cube cube = new Cube();

		cube.x = evenQ.x;
		cube.z = evenQ.y - (evenQ.x + (evenQ.x % 2)) / 2;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Offset CubeToOddQ(Cube cube)
	{
		Offset oddQ = new Offset();

		oddQ.x = cube.x;
		oddQ.y = cube.z + (cube.x - (cube.x % 2)) / 2;

		return oddQ;
	}

	public static Cube OddQToCube(Offset oddQ)
	{
		Cube cube = new Cube();

		cube.x = oddQ.x;
		cube.z = oddQ.y - (oddQ.x - (oddQ.x % 2)) / 2;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Offset CubeToEvenR(Cube cube)
	{
		Offset evenR = new Offset();

		evenR.x = cube.x + (cube.z + (cube.z % 2)) / 2;
		evenR.y = cube.z;

		return evenR;
	}

	public static Cube EvenRToCube(Offset evenR)
	{
		Cube cube = new Cube();

		cube.x = evenR.x - (evenR.y + (evenR.y % 2)) / 2;
		cube.z = evenR.y;
		cube.y = -cube.x - cube.z;

		return cube;
	}

	public static Offset CubeToOddR(Cube cube)
	{
		Offset oddR = new Offset();

		oddR.x = cube.x + (cube.z - (cube.z % 2)) / 2;
		oddR.y = cube.z;

		return oddR;
	}

	public static Cube OddRToCube(Offset oddR)
	{
		Cube cube = new Cube();

		cube.x = oddR.x - (oddR.y - (oddR.y % 2)) / 2;
		cube.z = oddR.y;
		cube.y = - cube.x - cube.z;

		return cube;
	}

	public static Cube OffsetToCube( Offset offset, OffsetType type )
	{
		Cube c;

		if (type == OffsetType.EvenQ)
		{
			c = EvenQToCube(offset);
		}
		else if (type == OffsetType.EvenR)
		{
			c = EvenRToCube(offset);
		}
		else if (type == OffsetType.OddQ)
		{
			c = OddQToCube(offset);
		}
		else
		{
			c = OddRToCube(offset);
		}

		return c;
	}

	public static Offset CubeToOffset( Cube cube, OffsetType type )
	{
		Offset o;

		if (type == OffsetType.EvenQ)
		{
			o = CubeToEvenQ(cube);
		}
		else if (type == OffsetType.EvenR)
		{
			o = CubeToEvenR(cube);
		}
		else if (type == OffsetType.OddQ)
		{
			o = CubeToOddQ(cube);
		}
		else
		{
			o = CubeToOddR(cube);
		}

		return o;
	}

	public static Axial CubeToAxial(Cube cube)
	{
		return new Axial(cube.x, cube.z);
	}

	public static CubeFraction AxialToCube(AxialFraction axial)
	{
		return new CubeFraction(axial.q, -axial.q - axial.r, axial.r);
	}

	public static Vector2 AxialToWorld(Axial axial, float pointRadius, OffsetType type)
	{
		float x;
		float y;

		if (type == OffsetType.EvenR || type == OffsetType.OddR)
		{
			x = pointRadius * Mathf.Sqrt( 3 ) * (axial.q + axial.r / 2);
			y = pointRadius * 3 / 2 * axial.r;
		}
		else
		{
			x = pointRadius * 3 / 2 * axial.q;
			y = pointRadius * Mathf.Sqrt( 3 ) * (axial.r + axial.q / 2);
		}

		return new Vector2( x, y );
	}

	public static AxialFraction WorldToAxial(Vector2 point, float pointRadius, OffsetType type)
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

		return new AxialFraction(q, r);
	}

	public static Offset OffsetRound(Vector2 point, float pointRadius, OffsetType type)
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

	public static Cube CubeRound(CubeFraction cubePoint)
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

		return new Cube((int)rx, (int)ry, (int)rz);
	}

	public static Vector2 OffsetToWorld(Offset offset, float pointRadius, OffsetType type)
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

	public static Cube RoundWorldToCube(Vector2 point, float pointRadius, OffsetType type)
	{
		return CubeRound( AxialToCube( WorldToAxial( point, pointRadius, type ) ) );
	}

	public static Vector2 CubeToWorld(Cube cube, float pointRadius, OffsetType type)
	{
		return AxialToWorld( CubeToAxial( cube ), pointRadius, type );
	}

	#endregion

	#region Others

	public static int OffsetDistance(Offset a, Offset b, OffsetType type)
	{
		Cube ac = OffsetToCube(a, type);
		Cube bc = OffsetToCube(b, type);

		return CubeDistance(ac, bc);
	}

	/// <summary>
	/// This function assumes your cube is a regular
	/// cube, with integer values and not fractional values
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns>The number of hexes to get from a to b</returns>
	public static int CubeDistance(Cube a, Cube b)
	{
		return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
	}

	public static CubeFraction CubeLerp( Cube a, Cube b, float t )
	{
		CubeFraction af = new CubeFraction((float)a.x, (float)a.y, (float)a.z);
		CubeFraction bf = new CubeFraction((float)b.x, (float)b.y, (float)b.z);

		return new CubeFraction(af.x + (bf.x - af.x) * t,
							 af.y + (bf.y - af.y) * t,
							 af.z + (bf.z - af.z) * t);
	}

	public static List<Cube> GetHexInLine(Cube from, Cube to)
	{
		int distance = CubeDistance(from, to);

		List<Cube> results = new List<Cube>();

		for (int i = 0; i <= distance; i++)
		{
			results.Add(CubeRound(CubeLerp(from, to, 1.0f / distance * (float)i)));
		}

		return results;
	}

	public static List<Offset> GetHexInLine(Offset a, Offset b, OffsetType type)
	{
		Cube ac = OffsetToCube(a, type);
		Cube bc = OffsetToCube(b, type);

		List<Cube> cubeList = GetHexInLine(ac, bc);

		List<Offset> offsetList = new List<Offset>();

		foreach ( Cube cube in cubeList )
			offsetList.Add(CubeToOffset(cube, type));

		return offsetList;
	}

	public static List<Cube> GetHexInRange(Cube center, int range)
	{
		List<Cube> results = new List<Cube>();

		for (int dx = -range; dx <= range; dx++ )
		{
			for (int dy = Mathf.Max(-range, -dx- range); dy <= Mathf.Min(range, -dx+ range); dy++ )
			{
				var dz = -dx-dy;
				results.Add(new Cube(center.x + dx, center.y + dy, center.z + dz));
			}
		}

		return results;
	}

	#endregion

}