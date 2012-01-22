// A sample player implemented in C#, based on the Java Migrate
// player.  In this player, each pusher takes one of the red markers
// and tries to gradually move it to vertices that are on the
// boundary between red and non-red.
//
// This is probably only the 5th or 6th C# program I've written, so feel free to
// let me know if there are better ways to use the langauge.
//
// ICPC Challenge
// Sturgill, NC State University

using System;
using System.Collections.Generic;


public class Migrate
{
	// Width and height of the world in game units.
	const int FIELD_SIZE = 100;

	// Number of pushers per side.
	const int PCOUNT = 3;

	// Radius of the pusher.
	const double PUSHER_RADIUS = 1;

	// Mass of the pusher.
	const double PUSHER_MASS = 1;

	// Maximum velocity for a pusher
	const double PUSHER_SPEED_LIMIT = 6.0;

	// Maximum acceleration for a pusher
	const double PUSHER_ACCEL_LIMIT = 2.0;

	// Total number of markers on the field
	const int MCOUNT = 22;

	// Radius of the marker
	const double MARKER_RADIUS = 2;

	// Mass of the marker.
	const double MARKER_MASS = 3;

	// Marker velocity lost per turn
	const double MARKER_FRICTION = 0.35;

	// Width and height of the home region.
	const int HOME_SIZE = 20;

	// Color value for the red player.
	const int RED = 0;

	// Color value for the blue player.
	const int BLUE = 1;

	// Color value for unclaimed pucks.
	const int GREY = 2;

	// Source of Randomness
	// private Random rnd = new Random(5);

	// Current game score, for red and blue
	private int[] score = new int[2];



	// Simple representation for a marker.
	class Marker
	{
		// Position of the marker.
		public Vector2D pos = new Vector2D(0, 0);

		// Marker velocity
		public Vector2D vel = new Vector2D(0, 0);

		public int beingPushedBy = -1;

		// Marker color
		public int color;
	};

	// Return the value of a, clamped to the [ b, c ] range
	private double Clamp(double a, double b, double c)
	{
		if (a < b)
			return b;
		if (a > c)
			return c;
		return a;
	}

	/* One dimensional function to help compute acceleration
	   vectors. Return an acceleration that can be applied to a pusher
	   at pos and moving with velocity vel to get it to target.  The
	   alim parameter puts a limit on the acceleration available.  This
	   function is used by the two-dimensional MoveTo function to
	   compute an acceleration vector toward the target after movement
	   perp to the target direction has been cancelled out. */
	private double MoveTo(double pos, double vel, double target,
						   double alim)
	{
		// Compute how far pos has to go to hit target.
		double dist = target - pos;

		// Kill velocity if we are close enough.
		if (Math.Abs(dist) < 0.01)
			return Clamp(-vel, -alim, alim);

		// How many steps, at minimum, would cover the remaining distance
		// and then stop.
		double steps = Math.Ceiling((-1 + Math.Sqrt(1 + 8.0 * Math.Abs(dist) / alim))
								 / 2.0);
		if (steps < 1)
			steps = 1;

		// How much acceleration would we need to apply at each step to
		// cover dist.
		double accel = 2 * dist / ((steps + 1) * steps);

		// Ideally, how fast would we be going now
		double ivel = accel * steps;

		// Return the best change in velocity to get vel to ivel.
		return Clamp(ivel - vel, -alim, alim);
	}

	/* Print out a force vector that will move the given pusher to
	   the given target location. */
	private void MoveTo(Pusher p, Vector2D target)
	{
		// Compute a frame with axis a1 pointing at the target.
		Vector2D a1, a2;

		// Build a frame (a trivial one if we're already too close).
		double dist = target.Distance(p.pos);

		if (dist < 0.0001)
		{
			a1 = new Vector2D(1.0, 0.0);
			a2 = new Vector2D(0.0, 1.0);
		}
		else
		{
			a1 = (target - p.pos) * (1.0 / dist);
			a2 = a1.Perp();
		}

		// Represent the pusher velocity WRT that frame.
		double v1 = a1 * p.vel;
		double v2 = a2 * p.vel;

		// Compute a force vector in this frame, first cancel out velocity
		// perp to the target.
		double f1 = 0;
		double f2 = -v2;

		// If we have remaining force to spend, use it to move toward the target.
		if (Math.Abs(f2) < PUSHER_ACCEL_LIMIT)
		{
			double raccel = Math.Sqrt(PUSHER_ACCEL_LIMIT * PUSHER_ACCEL_LIMIT -
									   v2 * v2);
			f1 = MoveTo(-dist, v1, 0.0, raccel);
		}

		// Convert force 
		Vector2D force = a1 * f1 + a2 * f2;
		p.MoveThisTurn = force.x + " " + force.y;
	}

	/* Print out a force vector that will move the given pusher around
	   to the side of marker m that's opposite from target.  Return true
	   if we're alreay behind the marker.  */
	private bool MoveAround(Pusher p, Marker m, Vector2D target)
	{
		// Compute vectors pointing from marker-to-target and Marker-to-pusher
		Vector2D mToT = (target - m.pos).Norm();
		Vector2D mToP = (p.pos - m.pos).Norm();

		// See if we're already close to behind the marker.
		if (mToT * mToP < -0.8)
			return true;

		// Figure out how far around the target we need to go, we're
		// going to move around a little bit at a time so we don't hit
		// the target.
		double moveAngle = Math.Acos(mToT * mToP);
		if (moveAngle > Math.PI * 0.25)
			moveAngle = Math.PI * 0.25;

		// We're not, decide which way to go around.
		if (mToT.Cross(mToP) > 0)
		{
			// Try to go around to the right.
			MoveTo(p, m.pos + mToP.Rotate(moveAngle) * 4.0);
		}
		else
		{
			// Try to go around to the left.
			MoveTo(p, m.pos + mToP.Rotate(-moveAngle) * 4.0);
		}

		return false;
	}

	public static void Main()
	{
		// Make an instance of the player, and let it play the gaqme.
		Migrate migrate = new Migrate();
		migrate.Run();
	}


	// globals are a bad idea ;)
	List<int> candidates = new List<int>();
	string debug = "";
	Vector2D[] vertexPoints;
	Marker[] mList;

	public int PickAVertex(Vector2D near2)
	{
		// assume we have at least one element in candidates
		int choice = 0;
		double distance;
		double choiceDistance=vertexPoints[candidates[choice]].Distance(near2);

		for(int index=1;index < candidates.Count ; index++)
		{
			distance = vertexPoints[candidates[choice]].Distance(near2);
			if (distance < choiceDistance)
			{
				choice = index;
				choiceDistance = distance;
			}
		}

		int vertex = candidates[choice];
		candidates.RemoveAt(choice);
		return vertex;
	}

	public int FindNearest(Vector2D near2,int color)
	{
		bool first = true;
		int choice = 0;
		double distance = 0.0;
		double choiceDistance = 0.0;

		for (int index = 0; index < mList.Length; index++)
		{
			if ((mList[index].beingPushedBy == -1) && (mList[index].color == color))
			{
				distance = mList[index].pos.Distance(near2);

				if (first || (distance < choiceDistance))
				{
					choice = index;
					choiceDistance = distance;
					first = false;
				}
			}
		}
		return choice;
	}

	private void Run()
	{

	

		// Read the static parts of the map.

		// Read the list of vertex locations.
		int n = int.Parse(Console.ReadLine());

		// List of points in the map.
		Vertex3D[] vertexList = new Vertex3D[n];
		vertexPoints = new Vector2D[n];

		for (int i = 0; i < n; i++)
		{
			string[] tokens = Console.ReadLine().Split();
			vertexList[i] = new Vertex3D(int.Parse(tokens[0]),int.Parse(tokens[1]),int.Parse(tokens[2]));
			vertexPoints[i] = new Vector2D(int.Parse(tokens[0]), int.Parse(tokens[1]));
										
		}

		// Read the list of region outlines.
		n = int.Parse(Console.ReadLine());
		// List of regions in the map
		int[][] regionList = new int[n][];
		for (int i = 0; i < n; i++)
		{
			string[] tokens = Console.ReadLine().Split();
			int m = int.Parse(tokens[0]);
			regionList[i] = new int[m];
			for (int j = 0; j < m; j++)
				regionList[i][j] = int.Parse(tokens[j + 1]);
		}

		// List of current region colors, pusher and marker locations.
		// These are updated on every turn snapshot from the game.
		int[] regionColors = new int[regionList.Length];
		Pusher[] pList = new Pusher[2 * PCOUNT];
		for (int i = 0; i < pList.Length; i++)
			pList[i] = new Pusher();
		mList = new Marker[MCOUNT];
		for (int i = 0; i < mList.Length; i++)
			mList[i] = new Marker();

		for (int i = 0; i <= 3; i++)
		{
			pList[i].myMarker = i;
			mList[i].beingPushedBy = i;
		}


		int turnNum = int.Parse(Console.ReadLine());
		while (turnNum >= 0)
		{

			string[] tokens = Console.ReadLine().Split();
			score[RED] = int.Parse(tokens[0]);
			score[BLUE] = int.Parse(tokens[1]);

			// Read all the region colors.
			tokens = Console.ReadLine().Split();
			n = int.Parse(tokens[0]);
			for (int i = 0; i < regionList.Length; i++)
				regionColors[i] = int.Parse(tokens[i + 1]);

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
				mList[i].color = int.Parse(tokens[4]);
			}

			// Compute a bit vector for the region colors incident on each vertex.
			int[] vertexColors = new int[vertexList.Length];
			for (int i = 0; i < regionList.Length; i++)
				for (int j = 0; j < regionList[i].Length; j++)
				{
					if (regionColors[i] == 0)
						vertexColors[regionList[i][j]] |= 1;
					else
						vertexColors[regionList[i][j]] |= (1 << regionColors[i]);
				}
			// Candidate vertices for putting a marker on, vertices that have
			// some red but are not all red.
			candidates.Clear();
			for (int i = 0; i < vertexList.Length; i++)
				if (((vertexColors[i] & 0x1) == 1) && (vertexColors[i] != 1)) //&& (vertex[i].pos.x != 0) && (vertex[i].pos.y != 0))
					candidates.Add(i);

			//Console.Error.Write("Candiates : ");
			//foreach (int i in candidates) Console.Error.Write(i.ToString() + " ");
			//Console.Error.WriteLine();

			// Choose a next action for each pusher, each pusher is responsible
			// for the marker with the same index.
			for (int pdex = 0; pdex < PCOUNT; pdex++)
			{
				Pusher p = pList[pdex];
				p.MoveThisTurn = "";

				//	Console.Error.Write(" P" + pdex.ToString() + ":");
				if (p.myTask != (int)Pusher.Tasks.sleep)
				{
					p.jobTime++;

					if (p.jobTime >= 60)
					{
						p.myTask = (int)Pusher.Tasks.sleep;
					}
				}

				// If we lose our marker, then just sit idle.
				if ((mList[p.myMarker].color != RED) && (p.myTask == (int)Pusher.Tasks.move2vertex))
				{
					p.myTask = (int)Pusher.Tasks.sleep;
				}

				// deal with sleep.
				if (p.myTask == (int)Pusher.Tasks.sleep)
				{
					if (mList[p.myMarker].color == RED)
					{
						if (candidates.Count > 0)
						{
							p.targetVertex = PickAVertex(p.pos);
                            p.myTask = (int)Pusher.Tasks.move2vertex;
							p.jobTime = 0;  // added HL
						}
						else
						{
							debug = "Panic: There are no candidates!";
						}

						// we need to do something here!!
					}
					else
					{
						p.myTask = (int)Pusher.Tasks.move2home;
						p.jobTime = 0;
				//		debug += " P" + pdex.ToString() + ":Sleeping grey, strange.";
					}
				}

				// Choose a move direction in support of our current goal.
				if (p.myTask == (int)Pusher.Tasks.move2vertex)
				{
					// Get behind our marker and push it toward its destination.
					Vertex3D v = vertexList[p.targetVertex];
					Vector2D dest = new Vector2D(v.x, v.y);

					if (((System.Math.Abs(mList[p.myMarker].pos.x - dest.x) < 2) && (System.Math.Abs(mList[p.myMarker].pos.y - dest.y) < 2)) || (vertexColors[p.targetVertex] == 1))
					{
						debug += " P" + pdex.ToString() + ":Hit It";

						int grey2move = FindNearest(p.pos,GREY);

						mList[p.myMarker].beingPushedBy = -1;
						p.myTask = (int)Pusher.Tasks.move2home;
						p.myMarker = grey2move;
						mList[p.myMarker].beingPushedBy = pdex;
						p.jobTime = 0;
						debug += " P" + pdex.ToString() + ":Moved to marker " + grey2move.ToString();
					}
					else
					{
						if (MoveAround(p, mList[p.myMarker], dest))
						{
							Vector2D mToD = (dest - mList[p.myMarker].pos).Norm();
							MoveTo(p, mList[p.myMarker].pos - mToD);
						}
					}
				}

				if (p.myTask == (int)Pusher.Tasks.move2home)
				{
					if (mList[p.myMarker].color == RED)
					{
						if (candidates.Count > 0)
						{
							p.targetVertex = PickAVertex(p.pos);
							p.myTask = (int)Pusher.Tasks.move2vertex;
							p.jobTime = 0;  

							debug = " P" + pdex.ToString() + ":aiming for vertex " + p.targetVertex.ToString();
						
						}
						else
						{
							debug = "Panic: There are no candidates!";
						}

					//	p.jobTime = 0;
					//	p.myTask = (int)Pusher.Tasks.sleep;
					}
					else
					{
						if ((mList[p.myMarker].pos.x < 18) && (mList[p.myMarker].pos.y < 18))
						{
							mList[p.myMarker].beingPushedBy = -1;
							p.myTask = (int)Pusher.Tasks.sleep;
							p.myMarker = FindNearest(p.pos,RED);  // we should make sure this is red.
							mList[p.myMarker].beingPushedBy = pdex;
							p.jobTime = 0;
							debug += " P" + pdex.ToString() + ":Moved to marker " + pdex.ToString();
						}
						else
						{
							Vector2D dest = new Vector2D(0, 0);

							if (MoveAround(p, mList[p.myMarker], dest))
							{
								Vector2D mToD = (dest - mList[p.myMarker].pos).Norm();
								MoveTo(p, mList[p.myMarker].pos - mToD);
							}
						}
					}
				}

				if (p.MoveThisTurn == "")
					Console.Write("0.0 0.0");
				else
					Console.Write(p.MoveThisTurn);

				// Print a space or a newline depending on whether we're at the last pusher.
				if (pdex + 1 < PCOUNT)
				{
					Console.Write(" ");
					//Console.Error.Write(" ");
				}
				else
				{
					Console.WriteLine();
					//Console.Error.WriteLine();
				}
			}

			if (debug.Length > 0)
			{
				Console.Error.Write("Turn " + turnNum.ToString() + " - ");
				Console.Error.WriteLine(debug);
				debug = "";
			}
			turnNum = int.Parse(Console.ReadLine());
			if (turnNum == -1) return;
		}
	}
}
