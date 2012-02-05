using System;


// Simple representation of a 2D point/vector.  C# may already
// provide something like this.  If so, I should have used it
// instead.

public class Vector2D
{
	public double x, y;

	// Make a 2D point/vector with the given coordinates.
	//  public Vector2D( double vx = 0, double vy = 0 ) {
	public Vector2D(double vx, double vy)
	{
		x = vx;
		y = vy;
	}

	// Return the squared magnitude of this vector.
	public double SquaredMag()
	{
		return x * x + y * y;
	}

	// Return the magnitude of this vector
	public double Mag()
	{
		return Math.Sqrt(x * x + y * y);
	}

	// Return a unit vector pointing in the same direction as this.
	public Vector2D Norm()
	{
		double m = Mag();
		return new Vector2D(x / m, y / m);
	}

	// Return a CCW perpendicular to this vector.
	public Vector2D Perp()
	{
		return new Vector2D(-y, x);
	}

	// Return a new vector thats thisvector rotated by r CCW.
	public Vector2D Rotate(double r)
	{
		double s = Math.Sin(r);
		double c = Math.Cos(r);
		return new Vector2D(x * c - y * s,	 x * s + y * c);
	}

	// Return a cross product of this and b.
	public double Cross(Vector2D b)
	{
		return x * b.y - y * b.x;
	}

	public static double Distance(Vector2D a, Vector2D b)
	{
		double x = Math.Abs(a.x - b.x);
		double y = Math.Abs(a.y - b.y);
		return (Math.Sqrt(x * x + y * y));
	}

	public double Distance(Vector2D b)
	{
		return Vector2D.Distance(this, b);
	}

	public double Distance(Point2D b)
	{
		return Point2D.Distance(this, b);
	}

	
	// Return a vector pointing in the same direction as this, but with
	// magnitude no greater than d.
	public Vector2D Limit(double d)
	{
		double m = Mag();
		if (m > d)
			return new Vector2D(d * x / m, d * y / m);
		else
			return new Vector2D(x, y);
	}

	// Vector addition.
	public static Vector2D operator +(Vector2D a, Vector2D b)
	{
		return new Vector2D(a.x + b.x, a.y + b.y);
	}

	// Vector subtraction.
	public static Vector2D operator -(Vector2D a, Vector2D b)
	{
		return new Vector2D(a.x - b.x, a.y - b.y);
	}

	// Return a copy of vector a, scaled by b
	public static Vector2D operator *(Vector2D a, double b)
	{
		return new Vector2D(a.x * b, a.y * b);
	}

	// Return a copy of vector a, scaled by b
	public static Vector2D operator *(double b, Vector2D a)
	{
		return new Vector2D(b * a.x, b * a.y);
	}

	// Return the dot product of a and b
	public static double operator *(Vector2D a, Vector2D b)
	{
		return a.x * b.x + a.y * b.y;
	}

	
};
