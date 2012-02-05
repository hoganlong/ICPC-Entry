using System;
using System.Collections.Generic;


public class Point2D
{
	public int x, y;

	public Point2D(int vx, int vy)
	{
		x = vx;
		y = vy;
	}

	public Point2D(Vector2D v)
		: this(v, d => (int)Math.Floor(d))
	{
		//x = (int)Math.Floor(v.x);
		//y = (int)Math.Floor(v.y);
	}

	public Point2D(Vector2D v, Func<double, int> converter)
	{
		x = converter(v.x);
		y = converter(v.y);
	}

	public Vector2D GetAsVector()
	{
		return new Vector2D(x, y);
	}

	internal static double Distance(Vector2D a, Point2D b)
	{
		double x = Math.Pow(a.x - b.x,2.0);
		double y = Math.Pow(a.y - b.y,2.0);
		return Math.Sqrt(x + y);
	}

	internal static double Distance(Point2D a, Point2D b)
	{
		int x = Math.Abs(a.x - b.x);
		int y = Math.Abs(a.y - b.y);
		return Math.Sqrt(x * x + y * y);
	}

	public double Distance(Vector2D b)
	{
		return Distance(b, this);
	}

	public double Distance(Point2D b)
	{
		return Point2D.Distance(this, b);
	}

}

// Simple representation for a vertex of the map.
public class Vertex3D
{
	public int x, y, z;
	public Point2D pos = null;

	public HashSet<int> adjacentRegions = null;

   public bool target = false;

	// make a 3D vertex with the given coordinates
	public Vertex3D(int vx, int vy, int vz)
	{
		x = vx;
		y = vy;
		z = vz;

		pos = new Point2D(vx, vy);

		adjacentRegions = new HashSet<int>();
	}
};
