using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Region
{
	public int num; // my number, helps with reports and foreach loops
	
	public int color = Map.UNKNOWN;

	public Point2D midPoint = null;

	public int[] vertexList = null; // this is a list of pointers to the vertex list in map.

	public HashSet<int> markerList = new HashSet<int>();

	public int redCount = 0;
	public int blueCount = 0;
	public int greyCount = 0;

	public Region(int mynum)
	{
		num = mynum;
	}
}
