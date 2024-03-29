﻿using System;
using System.Collections.Generic;

static public class Map
{
	// Number of pushers per side.
	public const int PCOUNT = 3;

	// Total number of markers on the field
	public const int MCOUNT = 22;

	// Current game score, for red and blue
	static public int[] score = new int[2];

	// Color value for the red player.
	public const int RED = 0;

	// Color value for the blue player.
	public const int BLUE = 1;

	// Color value for unclaimed pucks.
	public const int GREY = 2;

	// Color unknown
	public const int UNKNOWN = -1;


	static internal HashSet<int> candidates = new HashSet<int>();
	static internal Vertex3D[] vertexList=null;
	static internal Marker[] mList=null;
	static internal Pusher[] pList = null;
//	static internal int[] vertexColors = null;

	static internal Region[] regionList = null;

	// Internal change events (we could use an event model, but I'm lazy.
	static internal void RegionColorChanged(int regionNumber, int oldColor, int newColor)
	{
		// fix up vertex colors for all vertex which we touch
		for (int regionVertextNumber = 0; regionVertextNumber < regionList[regionNumber].vertexList.Length; regionVertextNumber++)
		{
			// get this vertex
			int vertexNum = regionList[regionNumber].vertexList[regionVertextNumber];

			// clear the color
			vertexList[vertexNum].adjacentColorMap = 0;

			// loop over the touching regions and set vertexColors
			foreach (int region in vertexList[vertexNum].adjacentRegions)
			{
				if (regionList[region].color == 0)
					vertexList[vertexNum].adjacentColorMap |= 1;
				else
					vertexList[vertexNum].adjacentColorMap |= (1 << regionList[region].color);
			}

			// Candidate vertices for putting a marker on, vertices that have some red but are not all red.
			if (((vertexList[vertexNum].adjacentColorMap & 0x1) == 1) && (vertexList[vertexNum].adjacentColorMap != 1))
			{
				candidates.Add(vertexNum);
			}
			else
			{
				if (candidates.Contains(vertexNum))
				{
					candidates.Remove(vertexNum);
				}
			}

		}

		//IO.ErrorWrite("Candiates: ");
		//foreach (int x in candidates)
		//{
		//    IO.ErrorWrite(x.ToString() + " ");
		//}
		//string tmp = "";
		//foreach (int x in candidates)
		//{
		//    if (Map.vertexList[x].AllAdjacentRed()) tmp += " "+x.ToString();
		//}
		//if (tmp.Length > 0)
		//    IO.ErrorWrite("All red -" + tmp);
		//IO.ErrorWriteLine("");


	}

	static internal void MarkerColorChanged(int markerNumber, int oldColor, int newColor)
	{
		foreach (int regionIndex in mList[markerNumber].myRegions)
		{
			switch (oldColor)
			{
				case Map.RED:
					regionList[regionIndex].redCount--;
					break;
				case Map.BLUE:
					regionList[regionIndex].blueCount--;
					break;
				case Map.GREY:
					regionList[regionIndex].greyCount--;
					break;
			}

			switch (newColor)
			{
				case Map.RED:
					regionList[regionIndex].redCount++;
					break;
				case Map.BLUE:
					regionList[regionIndex].blueCount++;
					break;
				case Map.GREY:
					regionList[regionIndex].greyCount++;
					break;
			}
 		}
	}

	
	static internal void RegionListChanged(int markerNumber, HashSet<int> newRegionList)
	{
		HashSet<int> oldRegionList = mList[markerNumber].myRegions;

	    HashSet<int> noLongerIn = new HashSet<int>();
		noLongerIn.UnionWith(oldRegionList);
		noLongerIn.ExceptWith(newRegionList);

		HashSet<int> newRegions = new HashSet<int>();
		newRegions.UnionWith(newRegionList);
		newRegions.ExceptWith(oldRegionList);

		foreach (int regionIndex in noLongerIn)
		{
			regionList[regionIndex].markerList.Remove(markerNumber);

			switch (mList[markerNumber].color)
			{
				case Map.RED:
					regionList[regionIndex].redCount--;
					break;
				case Map.BLUE:
					regionList[regionIndex].blueCount--;
					break;
				case Map.GREY:
					regionList[regionIndex].greyCount--;
					break;
			}
		}

		foreach (int regionIndex in newRegions)
		{
			regionList[regionIndex].markerList.Add(markerNumber);

			switch (mList[markerNumber].color)
			{
				case Map.RED:
					regionList[regionIndex].redCount++;
					break;
				case Map.BLUE:
					regionList[regionIndex].blueCount++;
					break;
				case Map.GREY:
					regionList[regionIndex].greyCount++;
					break;
			}
		}

	}

	// Runs once before first turn
	static internal void ReadStatic()
	{
		// Read the list of vertex locations.
		int n = int.Parse(IO.ReadLine());

		// List of points in the map.
		vertexList = new Vertex3D[n];
	//	vertexColors = new int[n];

		for (int i = 0; i < n; i++)
		{
			string[] tokens = IO.ReadLine().Split();
			vertexList[i] = new Vertex3D(i,int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
//			vertexList[i].adjacentColorMap  = 0;
		}
		//Console.Error.Write("VertexColors: ");
		//foreach (int x in vertexColors) Console.Error.Write(" " + x.ToString());
		//Console.Error.WriteLine();
	
		// Read the list of region outlines.
		n = int.Parse(IO.ReadLine());
		// List of regions in the map
		regionList = new Region[n];
		for (int regionNumber = 0; regionNumber < n; regionNumber++)
		{
			string[] tokens = IO.ReadLine().Split();
			int vertexCount = int.Parse(tokens[0]);
			regionList[regionNumber] = new Region(regionNumber);
      		regionList[regionNumber].vertexList = new int[vertexCount];
			for (int rVertextNumber = 0; rVertextNumber < vertexCount; rVertextNumber++)
			{
				int vertexNumber = int.Parse(tokens[rVertextNumber + 1]);
				regionList[regionNumber].vertexList[rVertextNumber] = vertexNumber;
				vertexList[vertexNumber].adjacentRegions.Add(regionNumber);
			}
		}

		// List of current region colors, pusher and marker locations.
		// These are updated on every turn snapshot from the game.
		//regionColors = new int[regionList.Length];
		pList = new Pusher[2 * PCOUNT];
		for (int i = 0; i < pList.Length; i++)
			pList[i] = new Pusher(i);
		mList = new Marker[MCOUNT];
		for (int i = 0; i < mList.Length; i++)
			mList[i] = new Marker(i);

	
	}

	// Runs every turn
	static internal void ReadTurn(int turnNum)
	{
		string[] tokens = IO.ReadLine().Split();
		Map.score[RED] = int.Parse(tokens[0]);
		Map.score[BLUE] = int.Parse(tokens[1]);

		// Read all the region colors.
		tokens = IO.ReadLine().Split();
		int n = int.Parse(tokens[0]);
		for (int i = 0; i < regionList.Length; i++)
		{
			int c = int.Parse(tokens[i + 1]);
			if (regionList[i].color != c)
			{
				regionList[i].color = c;
				if (turnNum != 0) RegionColorChanged(i, regionList[i].color, c);
			}
		}

		// Read all the pusher locations.
		n = int.Parse(IO.ReadLine());
		for (int i = 0; i < pList.Length; i++)
		{
			tokens = IO.ReadLine().Split();
			pList[i].pos.x = double.Parse(tokens[0]);
			pList[i].pos.y = double.Parse(tokens[1]);
			pList[i].vel.x = double.Parse(tokens[2]);
			pList[i].vel.y = double.Parse(tokens[3]);
		}

		// Read all the marker locations.
		n = int.Parse(IO.ReadLine());
		for (int i = 0; i < n; i++)
		{
			tokens = IO.ReadLine().Split();
			mList[i].pos.x = double.Parse(tokens[0]);
			mList[i].pos.y = double.Parse(tokens[1]);
			mList[i].vel.x = double.Parse(tokens[2]);
			mList[i].vel.y = double.Parse(tokens[3]);
			int c = int.Parse(tokens[4]);
			if (mList[i].color != c)
			{
				if (turnNum != 0) MarkerColorChanged(i, mList[i].color, c);
				mList[i].color = c;
			}

			if (turnNum != 0)
			{
				HashSet<int> rList = RegionMap.GetRegions(mList[i].pos);
				if (!rList.SetEquals(mList[i].myRegions))
				{
					RegionListChanged(i, rList);
					mList[i].myRegions = rList;
				}
			}
		}

	}
 
	static internal void StartTurnWork(int turnNum)
	{
		if (turnNum == 0)
		{
			// Compute a bit vector for the region colors incident on each vertex.
			for (int regionNumber = 0; regionNumber < regionList.Length; regionNumber++)
				for (int rVertextNumber = 0; rVertextNumber < regionList[regionNumber].vertexList.Length; rVertextNumber++)
				{
					if (regionList[regionNumber].color == 0)
						vertexList[regionList[regionNumber].vertexList[rVertextNumber]].adjacentColorMap |= 1;
					else
						vertexList[regionList[regionNumber].vertexList[rVertextNumber]].adjacentColorMap |= (1 << regionList[regionNumber].color);
				}

			//Console.Error.Write("VertexColors-1: ");
			//foreach (int x in vertexColors) Console.Error.Write(" " + x.ToString());
			//Console.Error.WriteLine();

			// Candidate vertices for putting a marker on, vertices that have
			// some red but are not all red.
			candidates.Clear();
			for (int i = 0; i < vertexList.Length; i++)
				if (((vertexList[i].adjacentColorMap & 0x1) == 1) && (vertexList[i].adjacentColorMap != 1)) //&& (vertex[i].pos.x != 0) && (vertex[i].pos.y != 0))
				{
					candidates.Add(i);
				}

			//IO.ErrorWrite("Candiates: ");
			//foreach (int x in candidates)
			//{
			//    IO.ErrorWrite(x.ToString() + " ");
			//}
			//IO.ErrorWriteLine("");

			for (int mIndex = 0; mIndex < mList.Length; mIndex++)
			{
				HashSet<int> rList = RegionMap.GetRegions(mList[mIndex].pos);

				foreach (int regionIndex in rList)
				{
					regionList[regionIndex].markerList.Add(mIndex);

					switch (mList[mIndex].color)
					{
						case Map.RED:
							regionList[regionIndex].redCount++;
							break;
						case Map.BLUE:
							regionList[regionIndex].blueCount++;
							break;
						case Map.GREY:
							regionList[regionIndex].greyCount++;
							break;
					}
				}

				mList[mIndex].myRegions = rList;
			}

			regionList[0].redCount = 10000;  // some serious redness

			//Console.Error.Write("Candidates: ");
			//foreach (int x in candidates) Console.Error.Write(" " + x.ToString());
			//Console.Error.WriteLine();


		}

	

	}
}

