using System;



// Simple representation for a pusher.
public class Pusher
{
	// Position of the pusher.
	public Vector2D pos = new Vector2D(0, 0);

	// Pusher velocity
	public Vector2D vel = new Vector2D(0, 0);

	public string MoveThisTurn = "";

	public BaseGoal myGoal = null;
	public BasePlan myPlan = null;

	public Pusher()
	{
		
	}
};