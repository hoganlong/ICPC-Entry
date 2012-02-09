using System;
using System.Collections.Generic;


// Simple representation for a marker.
public class Marker
{
	public int num; // my number, helps with reports and foreach loops
	
	// Position of the marker.
	public Vector2D pos = new Vector2D(0, 0);

	// Marker velocity
	public Vector2D vel = new Vector2D(0, 0);
 
	public BaseGoal beingUsedBy = null;

	public HashSet<int> myRegions = null;

	// Marker color
	public int color;

	public Marker(int mynum)
	{
		num = mynum;
	}

	public string Short()
	{
		return " Marker#" + num.ToString() + " @(" + pos.x.ToString("#,0.0") + "," + pos.y.ToString("#,0.0") + ")";
	}
};
