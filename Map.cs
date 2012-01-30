using System;
using System.Collections.Generic;

public class Map
{
	// Number of pushers per side.
	public const int PCOUNT = 3;

	// Total number of markers on the field
	public const int MCOUNT = 22;

	// Current game score, for red and blue
	public int[] score = new int[2];

	// Color value for the red player.
	public const int RED = 0;

	// Color value for the blue player.
	public const int BLUE = 1;

	// Color value for unclaimed pucks.
	public const int GREY = 2;

	// Color unknown
	public const int UNKNOWN = -1;


	internal List<int> candidates = new List<int>();
	internal Vertex3D[] vertexList=null;
	internal Vector2D[] vertexPoints=null;
	internal Marker[] mList=null;
	internal Pusher[] pList = null;
	internal int[] vertexColors = null;

	internal Region[] regionListNew = null;

	// Internal change events (we could use an event model, but I'm lazy.
	internal void RegionColorChanged(int regionNumber, int oldColor, int newColor)
	{
	}

	internal void MarkerColorChanged(int markerNumber, int oldColor, int newColor)
	{
	}

	// Runs once before first turn
	internal void ReadStatic()
	{
		// Read the list of vertex locations.
		int n = int.Parse(Console.ReadLine());

		// List of points in the map.
		vertexList = new Vertex3D[n];
		vertexPoints = new Vector2D[n];

		for (int i = 0; i < n; i++)
		{
			string[] tokens = Console.ReadLine().Split();
			vertexList[i] = new Vertex3D(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
			vertexPoints[i] = new Vector2D(int.Parse(tokens[0]), int.Parse(tokens[1]));
		}

		// Read the list of region outlines.
		n = int.Parse(Console.ReadLine());
		// List of regions in the map
		regionListNew = new Region[n];
		for (int i = 0; i < n; i++)
		{
			string[] tokens = Console.ReadLine().Split();
			int m = int.Parse(tokens[0]);
			regionListNew[i] = new Region();
      		regionListNew[i].vertexList = new int[m];
			for (int j = 0; j < m; j++)
				regionListNew[i].vertexList[j] = int.Parse(tokens[j+1]);
		}

		// List of current region colors, pusher and marker locations.
		// These are updated on every turn snapshot from the game.
		//regionColors = new int[regionList.Length];
		pList = new Pusher[2 * PCOUNT];
		for (int i = 0; i < pList.Length; i++)
			pList[i] = new Pusher();
		mList = new Marker[MCOUNT];
		for (int i = 0; i < mList.Length; i++)
			mList[i] = new Marker();
	}

	// Runs every turn
	internal void ReadTurn()
	{

		string[] tokens = Console.ReadLine().Split();
		score[RED] = int.Parse(tokens[0]);
		score[BLUE] = int.Parse(tokens[1]);

		// Read all the region colors.
		tokens = Console.ReadLine().Split();
		int n = int.Parse(tokens[0]);
		for (int i = 0; i < regionListNew.Length; i++)
		{
			int c = int.Parse(tokens[i + 1]);
			if (regionListNew[i].color != c)
			{
				RegionColorChanged(i, regionListNew[i].color, c);
				regionListNew[i].color = c;
			}
		}

		// Read all the pusher locations.
		n = int.Parse(Console.ReadLine());
		for (int i = 0; i < pList.Length; i++)
		{
			tokens = Console.ReadLine().Split();
			pList[i].pos.x = double.Parse(tokens[0]);
			pList[i].pos.y = double.Parse(tokens[1]);
			pList[i].vel.x = double.Parse(tokens[2]);
			pList[i].vel.y = double.Parse(tokens[3]);
		}

		// Read all the marker locations.
		n = int.Parse(Console.ReadLine());
		for (int i = 0; i < n; i++)
		{
			tokens = Console.ReadLine().Split();
			mList[i].pos.x = double.Parse(tokens[0]);
			mList[i].pos.y = double.Parse(tokens[1]);
			mList[i].vel.x = double.Parse(tokens[2]);
			mList[i].vel.y = double.Parse(tokens[3]);
			int c = int.Parse(tokens[4]);
			if (mList[i].color != c)
			{
				MarkerColorChanged(i, mList[i].color, c);
				mList[i].color = c;
			}
		}
	}

	internal void StartTurnWork()
	{
		// Compute a bit vector for the region colors incident on each vertex.
		vertexColors = new int[vertexList.Length];
		for (int regionNumber = 0; regionNumber < regionListNew.Length; regionNumber++)
			for (int rVertextNumber = 0; rVertextNumber < regionListNew[regionNumber].vertexList.Length; rVertextNumber++)
			{
				if (regionListNew[regionNumber].color == 0)
					vertexColors[regionListNew[regionNumber].vertexList[rVertextNumber]] |= 1;
				else
					vertexColors[regionListNew[regionNumber].vertexList[rVertextNumber]] |= (1 << regionListNew[regionNumber].color);
			}
		// Candidate vertices for putting a marker on, vertices that have
		// some red but are not all red.
		candidates.Clear();
		for (int i = 0; i < vertexList.Length; i++)
			if (((vertexColors[i] & 0x1) == 1) && (vertexColors[i] != 1)) //&& (vertex[i].pos.x != 0) && (vertex[i].pos.y != 0))
				candidates.Add(i);
	}
}

