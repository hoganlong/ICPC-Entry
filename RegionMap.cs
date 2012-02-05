using System;
using System.Collections.Generic;
  
class RegionMapPoint
{
	internal HashSet<int> myRegions;

	public RegionMapPoint()
	{
		myRegions = new HashSet<int>();
	}

}

static public class RegionMap
{
	static RegionMapPoint[,] loc = new RegionMapPoint[101, 101];

	static RegionMap()
	{
		for (int x = 0; x <= 100; x++)
			for (int y = 0; y <= 100; y++)
				loc[x, y] = new RegionMapPoint();
	}

	static public HashSet<int> GetRegions(Vector2D pos)
	{
		return GetRegions((int)Math.Floor(pos.x), (int)Math.Floor(pos.y));
	}

	static public HashSet<int> GetRegions(int x, int y)
	{
		HashSet<int> result = new HashSet<int>();
		
		result.UnionWith(loc[x, y].myRegions);

		return result;
	}

	// Does the work of setting up the map... for each region do a line scan and add to points.
	static public void Setup()
	{
		// loop for each region
		for (int regionNumber = 0; regionNumber < Map.regionList.Length; regionNumber++)
		{
			List<Vertex3D> vertexList= new List<Vertex3D>();
			
			// Make a list of points
			for (int index = 0; index < Map.regionList[regionNumber].vertexList.Length; index++)
				vertexList.Add(Map.vertexList[Map.regionList[regionNumber].vertexList[index]]);

			ScanARegion(regionNumber, vertexList);
		}
	}

	static private void ScanARegion(int regionNumber, List<Vertex3D> vertexList)
	{
		int minY = 101;
		int maxY = -1;

		// find min and max x
		foreach (Vertex3D v in vertexList)
		{
			if (v.y < minY) minY = v.y;
			if (v.y > maxY) maxY = v.y;
		}

		List<double> crossLoc = new List<double>();
		// loop over each x
		for (int y = minY; y <= maxY; y++)
		{
			crossLoc.Clear();
			
			// if not then find the (two) x locations it crosses (loop lines index -> index+1 and then index[max]+index[0]
			//List<Vertex3D> crossLoc = new List<Vertex3D>();
			for (int vertexIndex = 0; vertexIndex < vertexList.Count; vertexIndex++)
			{
				Vertex3D a = vertexList[vertexIndex];
				Vertex3D b = vertexList[(vertexIndex == vertexList.Count-1)? 0:vertexIndex+1]; // tricky, also check first and last.

				if ((a.y == b.y) && (a.y == y))
				{
					crossLoc.Clear();
					crossLoc.Add((double)a.x);
					crossLoc.Add((double)b.x);
					break;   // convex, so we are done.
				}
				else
				{
					if ((a.y < y) && (b.y >= y) || (b.y < y) && (a.y >= y))
					{
						//int changeX = b.x - a.x;
						//int changeY = b.y - a.y;
						//double ratio = changeX/changeY;
						//int distance = y - a.y;
						//double xLoc = a.x + (distance*ratio);

						double xLoc = a.x + ((y - a.y) * ((b.x - a.x) / (b.y - a.y)));
						//double xLoc = a.x + ((y - a.y) / (b.y - a.y)) * (b.x - a.x);
						crossLoc.Add(xLoc);
					}
					else
					{
						if (a.y == y) crossLoc.Add(a.x);
						if (b.y == y) crossLoc.Add(b.x); 
					}
				}
			}

			int minX;
			int maxX;

			if (crossLoc.Count == 1) // Single point
			{
				minX = maxX = (int)Math.Floor(crossLoc[0]);
			}
			else
			{
				// if statement would probably be faster (shrug)
				minX = (int)Math.Floor(Math.Min(crossLoc[0], crossLoc[1]));
				maxX = (int)Math.Floor(Math.Max(crossLoc[0], crossLoc[1]));
			}
			// add a "line" of points from one to the other.
			for (int x = minX; x <= maxX; x++)
				loc[x, y].myRegions.Add(regionNumber);
		}
	}
}
