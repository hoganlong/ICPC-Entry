using System;



// Simple representation for a pusher.
public class Pusher
{
	public int num; // my number, helps with reports and foreach loops

	// Position of the pusher.
	public Vector2D pos = new Vector2D(0, 0);

	// Pusher velocity
	public Vector2D vel = new Vector2D(0, 0);

	public string MoveThisTurn = "";

	public BaseGoal myGoal = null;
	public BasePlan myPlan = null;

	public Pusher(int mynum)
	{
		num = mynum;		
	}
};