using UnityEngine;
using System.Collections;

public class HexLocation {

	public HexMath.Offset Offset { get; private set; }

	public int Height { get; private set; }

	public HexLocation (int row, int column, int height)
	{
		this.Offset = new HexMath.Offset(row, column);

		this.Height = height;
	}

}
