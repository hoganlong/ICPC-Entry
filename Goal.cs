using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GoalUtility
{
	// Radius of the pusher.
	const double PUSHER_RADIUS = 1;

	// Mass of the pusher.
	const double PUSHER_MASS = 1;

	// Maximum velocity for a pusher
	const double PUSHER_SPEED_LIMIT = 6.0;

	// Maximum acceleration for a pusher
	const double PUSHER_ACCEL_LIMIT = 2.0;


	// Radius of the marker
	const double MARKER_RADIUS = 2;

	// Mass of the marker.
	const double MARKER_MASS = 3;

	// Marker velocity lost per turn
	const double MARKER_FRICTION = 0.35;

	static public int PickAVertex(Map map, Vector2D near2)
	{
		// assume we have at least one element in candidates
		int choice = 0;
		double distance;
		double choiceDistance = map.vertexPoints[map.candidates[choice]].Distance(near2);

		for (int index = 1; index < map.candidates.Count; index++)
		{
			if (map.vertexList[map.candidates[index]].target == false)
			{
				distance = map.vertexPoints[map.candidates[index]].Distance(near2);
				if (distance < choiceDistance)
				{
					choice = index;
					choiceDistance = distance;
				}
			}
		}

		int vertex = map.candidates[choice];

		map.candidates.RemoveAt(choice);
		return vertex;
	}

	static public int FindNearest(Map map, Vector2D near2, int color)
	{
		bool first = true;
		int choice = -1;
		double distance = 0.0;
		double choiceDistance = 0.0;

		for (int index = 0; index < map.mList.Length; index++)
		{
			if ((map.mList[index].beingUsedBy == null) && (map.mList[index].color == color))
			{
				distance = map.mList[index].pos.Distance(near2);

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
 
	// Return the value of a, clamped to the [ b, c ] range
	static private double Clamp(double a, double b, double c)
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
	static private double MoveTo(double pos, double vel, double target,	double alim)
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
	static public void MoveTo(Pusher p, Vector2D target)
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
			f1 = GoalUtility.MoveTo(-dist, v1, 0.0, raccel);
		}

		// Convert force 
		Vector2D force = a1 * f1 + a2 * f2;
		p.MoveThisTurn = force.x + " " + force.y;
	}

	/* Print out a force vector that will move the given pusher around
	   to the side of marker m that's opposite from target.  Return true
	   if we're alreay behind the marker.  */
	static public bool MoveAround(Pusher p, Marker m, Vector2D target)
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

}

public abstract class BaseGoal
{
	internal int startTime = 0;
	internal Vector2D startLoc = null;

	public BaseGoal(Map map, Pusher me,int turn)
	{
		startTime = turn;
		startLoc = me.pos;
	}

	abstract public void Action(Map map);

	abstract public bool Done(Map map, int turn);

	abstract public void CleanUp(); // should make disposable

	abstract public string Name();
}

public class MoveMarkerToVertexGoal : BaseGoal
{
	Marker myMarker = null;
	int targetVertextSeq = -1;
	Vertex3D targetVertex = null;
	Vector2D dest = null;
	Pusher me = null;

	public MoveMarkerToVertexGoal(Map map, Pusher inMe, int turn) : base(map,inMe,turn)
	{
		me = inMe;

		// Find a marker to move
		myMarker = map.mList[GoalUtility.FindNearest(map, me.pos, Map.RED)];
		myMarker.beingUsedBy = this;

		// Find a place to push it.
		targetVertextSeq = GoalUtility.PickAVertex(map, myMarker.pos);
		targetVertex = map.vertexList[targetVertextSeq];
		targetVertex.target = true;	 // we should fix this make me the user.
		dest = new Vector2D(targetVertex.x, targetVertex.y);
	}

	public override  void Action(Map map)
	{
		if (GoalUtility.MoveAround(me, myMarker, dest))
		{
			Vector2D mToD = (dest - myMarker.pos).Norm();
			GoalUtility.MoveTo(me, myMarker.pos - mToD);
		}
	}

	public override bool Done(Map map, int turn)
	{
		// we should check to see if we have not gotten anywhere

		if (turn > startTime + 60)
			return true;

		if (map.vertexColors[targetVertextSeq] == 1) 
			return true; // it converted while I was moving

	    if ((myMarker.pos.Distance(dest) < 2.0) && (myMarker.pos.x > 2 && myMarker.pos.y > 2)) 
			return true; // I'm there. (>2 so we don't jam the edges)

	    if (dest.x == 0) // on the left edge
		{
			if ((me.pos.x < 6.0) && (me.pos.y > dest.y - 2) && (me.pos.y < dest.y + 2))
				return true;
		}
		
		if (dest.y == 0) // on the bottom
		{
			if ((me.pos.y < 6.0) && (me.pos.x > dest.x - 2) && (me.pos.x < dest.x + 2))
				return true;
		}

		if (dest.y == 100) // on the top
		{
			if ((me.pos.y > 94.0) && (me.pos.x > dest.x - 2) && (me.pos.x < dest.x + 2))
				return true;
		}

		return false;
	}

	public override void CleanUp()
	{
		if (myMarker != null)
		  myMarker.beingUsedBy = null;
		if (targetVertex != null)
		  targetVertex.target = false;
	}

	public override string Name()
	{
		return "MoveToVertex";
	}
}

public class TurnGreyMarkerRedGoal : BaseGoal
{
	Pusher me = null;
	Marker myMarker = null;
	Vector2D dest = new Vector2D(0, 0);
	bool donefor = false;

	public TurnGreyMarkerRedGoal(Map map, Pusher inMe, int turn) : base(map, inMe, turn)
	{
		me = inMe;

		// Find a marker to move
		int tmp = GoalUtility.FindNearest(map, me.pos, Map.GREY);
		if (tmp == -1)
     		tmp = GoalUtility.FindNearest(map, me.pos, Map.BLUE);
		if (tmp == -1)
			donefor = true;
		else
		{
			myMarker = map.mList[tmp];

			myMarker.beingUsedBy = this;
		}
	}

	public override void Action(Map map)
	{
		if (donefor) return;

		if (GoalUtility.MoveAround(me, myMarker, dest))
		{
			Vector2D mToD = (dest - myMarker.pos).Norm();
			GoalUtility.MoveTo(me, myMarker.pos - mToD);
		}
	}

	public override bool Done(Map map, int turn)
	{
		// we should check to see if we have not gotten anywhere

		// we can't find something to move!
		if (donefor)
			return true;

		if (turn > startTime + 55)
			return true;

		if (myMarker.color == Map.RED)
			return true;

		if (myMarker.pos.Distance(dest) < 20)
			return true;

		return false;
	}

	public override void CleanUp()
	{
		if (myMarker != null)
			myMarker.beingUsedBy = null;
	}

	public override string Name()
	{
		return "TurnToRed";
	}
}

