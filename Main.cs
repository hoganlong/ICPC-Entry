using System;
using System.Collections.Generic;

public class Migrate
{
	// Width and height of the world in game units.
	const int FIELD_SIZE = 100;

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

	// Width and height of the home region.
	const int HOME_SIZE = 20;
 
	internal string debug = "";
 
	public static void Main()
	{
		// Make an instance of the player, and let it play the gaqme.
		Migrate migrate = new Migrate();
		migrate.Run();
	}
	
	private void Run()
	{
		// Read the static parts of the map.
		Map map = new Map();

		map.ReadStatic();
   
		int turnNum = int.Parse(Console.ReadLine());

		map.pList[0].myPlan = new SwitchingPlan(map.pList[0]);
		map.pList[1].myPlan = new SwitchingPlan(map.pList[1]);
		map.pList[2].myPlan = new SwitchingPlan(map.pList[2]);
  
		while (turnNum >= 0)
		{
			map.ReadTurn();

			map.StartTurnWork();
 
			for (int pdex = 0; pdex < Map.PCOUNT; pdex++)
			{
				Pusher p = map.pList[pdex];
				p.MoveThisTurn = "";

				if (p.myGoal == null) p.myGoal = p.myPlan.GetNextGoal(map, turnNum);
				
				if (p.myGoal.Done(map, turnNum))
				{
					p.myGoal.CleanUp();
					
					p.myGoal = p.myPlan.GetNextGoal(map,turnNum);
				}
				else
				{
					p.myGoal.Action(map);
				}
  
				if (p.MoveThisTurn == "")
					Console.Write("0.0 0.0");
				else
					Console.Write(p.MoveThisTurn);

				// Print a space or a newline depending on whether we're at the last pusher.
				if (pdex + 1 < Map.PCOUNT)
				{
					Console.Write(" ");
					//Console.Error.Write(" ");
				}
				else
				{
					Console.WriteLine();
					//Console.Error.WriteLine();
				}
			}

			if (debug.Length > 0)
			{
				Console.Error.Write("Turn " + turnNum.ToString() + " - ");
				Console.Error.WriteLine(debug);
				debug = "";
			}
			turnNum = int.Parse(Console.ReadLine());
			if (turnNum == -1) return;
		}
	}
}
