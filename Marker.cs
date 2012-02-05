using System;
using System.Collections.Generic;


// Simple representation for a marker.
public class Marker
{
	// Position of the marker.
	public Vector2D pos = new Vector2D(0, 0);

	// Marker velocity
	public Vector2D vel = new Vector2D(0, 0);
 
	public BaseGoal beingUsedBy = null;

	public HashSet<int> myRegions = null;

	// Marker color
	public int color;
};
