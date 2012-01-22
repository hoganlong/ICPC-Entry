using System;



// Simple representation for a pusher.
class Pusher
{
	public enum Tasks
	{
		sleep,
		move2vertex,
		move2home
	}
	
	// Position of the pusher.
	public Vector2D pos = new Vector2D(0, 0);

	// Pusher velocity
	public Vector2D vel = new Vector2D(0, 0);

	public string MoveThisTurn = "";

	public int myTask;

	public int myMarker;

	// How long we've been doing the current job.  If
	// this number gets to large, we'll pick a new job.

	public int jobTime;

	// Target vertex for this pusher.
	public int targetVertex;

	public Pusher()
	{
		myTask = (int)Tasks.sleep;
	}
};