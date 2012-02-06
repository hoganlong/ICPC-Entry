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
		//Map Map = new Map();

		Map.ReadStatic();

		RegionMap.Setup();
   
		int turnNum = int.Parse(IO.ReadLine());

		Map.pList[0].myPlan = new SwitchingPlan(Map.pList[0]);
		Map.pList[1].myPlan = new SwitchingPlan(Map.pList[1]);
		Map.pList[2].myPlan = new SwitchingPlan2(Map.pList[2]);
  
		while (turnNum >= 0)
		{
			Map.ReadTurn(turnNum);

			Map.StartTurnWork(turnNum);
 
			for (int pdex = 0; pdex < Map.PCOUNT; pdex++)
			{
				Pusher p = Map.pList[pdex];
				p.MoveThisTurn = "";

				if (p.myGoal == null) p.myGoal = p.myPlan.GetNextGoal(turnNum);
				
				if (p.myGoal.Done(turnNum))
				{
					p.myGoal.CleanUp();
					
					p.myGoal = p.myPlan.GetNextGoal(turnNum);
				}
				else
				{
					p.myGoal.Action();
				}
  
				if (p.MoveThisTurn == "")
					IO.Write("0.0 0.0");
				else
					IO.Write(p.MoveThisTurn);

				// Print a space or a newline depending on whether we're at the last pusher.
				if (pdex + 1 < Map.PCOUNT)
				{
					IO.Write(" ");
					//Console.Error.Write(" ");
				}
				else
				{
					IO.WriteLine();
					//Console.Error.WriteLine();
				}
			}

			if (debug.Length > 0)
			{
				IO.ErrorWrite("Turn " + turnNum.ToString() + " - ");
				IO.ErrorWriteLine(debug);
				debug = "";
			}
			turnNum = int.Parse(IO.ReadLine());
			if (turnNum == -1) return;
		}
	}
}
