using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Region
{
	public int color = Map.UNKNOWN;

	public int[] vertexList = null; // this is a list of pointers to the vertex list in map.

	public HashSet<int> markerList = new HashSet<int>();

	public int redCount = 0;
	public int blueCount = 0;
	public int greyCount = 0;
}
