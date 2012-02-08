using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BasePlan
{
	internal Pusher myP = null;
	internal string state = "";
	

	public BasePlan(Pusher p)
	{
		myP = p;
		state = "Initial";
	}

	abstract public BaseGoal GetNextGoal(int turnNumber);
}

// Switch between getting vertex and converting grey.
// Same plan from the start -- the most basic strategy.
public class SwitchingPlan : BasePlan
{
	public SwitchingPlan(Pusher me) : base(me)
	{
	}

	public override BaseGoal GetNextGoal(int turnNum)
	{
		BaseGoal nextGoal = null;

		if (state == "MoveToVertex")
		{
			nextGoal = new TurnGreyMarkerRedGoal(myP, turnNum);
			state = "MoveToRed";
			if (nextGoal.Done(turnNum))
			{
				nextGoal.CleanUp();
				nextGoal = new MoveMarkerToVertexGoal(myP, turnNum);
				state = "MoveToVertex";
			}
		}
		else 
		{
			nextGoal = new MoveMarkerToVertexGoal(myP, turnNum);
			state = "MoveToVertex";
		}

		return nextGoal;
	}
}

// Switch between getting vertex and converting grey.
// Same plan from the start -- the most basic strategy.
// now we don't move home for turn grey red.
public class SwitchingPlan2 : BasePlan
{
	BaseGoal lastGoal = null;

	public SwitchingPlan2(Pusher me) : base(me)
	{
	}

	public override BaseGoal GetNextGoal(int turnNum)
	{
		BaseGoal nextGoal = null;


		if (state == "MoveToVertex")
		{
			nextGoal = new TurnGreyMarkerRedGoal2(myP, turnNum);
			state = "MoveToRed";
			if (nextGoal.Done(turnNum))
			{
				nextGoal.CleanUp();
				nextGoal = new MoveMarkerToVertexGoal(myP, turnNum);
				state = "MoveToVertex";
			}
		}
		else // oldName == "TurnToRed"
		{
			nextGoal = new MoveMarkerToVertexGoal(myP, turnNum);
			state = "MoveToVertex";
		}


		return lastGoal;
	}
}
