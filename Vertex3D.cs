using System;

// Simple representation for a vertex of the map.
public class Vertex3D
{
	public int x, y, z;

   public bool target = false;

	// make a 3D vertex with the given coordinates
	//  public Vertex3D( int vx = 0, int vy = 0, int vz = 0 ) {
	public Vertex3D(int vx, int vy, int vz)
	{
		x = vx;
		y = vy;
		z = vz;
	}
};
