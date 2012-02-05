using System;
using System.Collections.Generic;

public class Map
{
	// Current game score, for red and blue
	public int[] score = new int[2];

	//internal HashSet<int> candidates = new HashSet<int>();
	internal Vertex3D[] vertexList = null;

	internal Region[] regionList = null;

	// Runs once before first turn
	internal void ReadStatic()
	{
		// Read the list of vertex locations.
		int n = int.Parse(IO.ReadLine());

		// List of points in the map.
		vertexList = new Vertex3D[n];

		for (int i = 0; i < n; i++)
		{
			string[] tokens = IO.ReadLine().Split();
			vertexList[i] = new Vertex3D(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
		}


		// Read the list of region outlines.
		n = int.Parse(IO.ReadLine());
		// List of regions in the map
		regionList = new Region[n];
		for (int regionNumber = 0; regionNumber < n; regionNumber++)
		{
			string[] tokens = IO.ReadLine().Split();
			int vertexCount = int.Parse(tokens[0]);
			regionList[regionNumber] = new Region();
			regionList[regionNumber].vertexList = new int[vertexCount];
			for (int rVertextNumber = 0; rVertextNumber < vertexCount; rVertextNumber++)
			{
				int vertexNumber = int.Parse(tokens[rVertextNumber + 1]);
				regionList[regionNumber].vertexList[rVertextNumber] = vertexNumber;
				vertexList[vertexNumber].adjacentRegions.Add(regionNumber);
			}
		}


	}
}
 