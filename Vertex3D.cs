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
	public int num; // my number, helps with reports and foreach loops

	public int x, y, z;
	public int adjacentColorMap = 0;

	public Point2D pos = null;

	public HashSet<int> adjacentRegions = null;

   public bool target = false;

	// make a 3D vertex with the given coordinates
	public Vertex3D(int mynum,int vx, int vy, int vz)
	{
		num = mynum;

		x = vx;
		y = vy;
		z = vz;

		pos = new Point2D(vx, vy);

		adjacentRegions = new HashSet<int>();
	}

	public bool AllAdjacentRed()
	{

		foreach (int rNum in adjacentRegions)
		{
			if (Map.regionList[rNum].color != Map.RED)
				return false;
		}

		return true;
	}

	public string Short2D()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();

		result.Append(" V"+num.ToString()+"@(" + pos.x.ToString("#,0.0") + "," + pos.y.ToString("#,0.0") + ")" + (target == true ? " Target" : ""));


        if (AllAdjacentRed()) result.Append(" ALL RED ");

		//bool hasNonRed = false;

		//foreach (int rNum in adjacentRegions)
		//{
		//    result.Append(" r" + rNum.ToString() + "-" + Map.regionList[rNum].color.ToString());
		//    if (Map.regionList[rNum].color != Map.RED)
		//        hasNonRed = true;
		//}
		//if (hasNonRed == false) result.Append(" ALL RED ");

		return result.ToString();
	}
};
