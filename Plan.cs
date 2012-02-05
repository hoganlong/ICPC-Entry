using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BasePlan
{
	internal Pusher myP = null;

	public BasePlan(Pusher p)
	{
		myP = p;
	}

	abstract public BaseGoal GetNextGoal(int turnNumber);
}

// Switch between getting vertex and converting grey.
// Same plan from the start -- the most basic strategy.
public class SwitchingPlan : BasePlan
{
	BaseGoal lastGoal = null;

	public SwitchingPlan(Pusher me) : base(me)
	{
	}

	public override BaseGoal GetNextGoal(int turnNum)
	{
		if (lastGoal == null)
		{
			lastGoal = new MoveMarkerToVertexGoal(myP, turnNum);
		}
		else
		{
			if (lastGoal.Name() == "MoveToVertex")
			{
				lastGoal = new TurnGreyMarkerRedGoal(myP, turnNum);
				if (lastGoal.Done(turnNum))
				{
					lastGoal.CleanUp();
					lastGoal = new MoveMarkerToVertexGoal(myP, turnNum);
				}
			}
			else // oldName == "TurnToRed"
			{
				lastGoal = new MoveMarkerToVertexGoal(myP, turnNum);
			}
		}

		return lastGoal;
	}
}

// Switch between getting vertex and converting grey.
// Same plan from the start -- the most basic strategy.
// now we don't move home for turn grey red.
public class SwitchingPlan2 : BasePlan
{
	BaseGoal lastGoal = null;

	public SwitchingPlan2(Pusher me)
		: base(me)
	{
	}

	public override BaseGoal GetNextGoal(int turnNum)
	{
		if (lastGoal == null)
		{
			lastGoal = new MoveMarkerToVertexGoal( myP, turnNum);
		}
		else
		{
			if (lastGoal.Name() == "MoveToVertex")
			{
				lastGoal = new TurnGreyMarkerRedGoal2(myP, turnNum);
				if (lastGoal.Done(turnNum))
				{
					lastGoal.CleanUp();
					lastGoal = new MoveMarkerToVertexGoal(myP, turnNum);
				}
			}
			else // oldName == "TurnToRed"
			{
				lastGoal = new MoveMarkerToVertexGoal(myP, turnNum);
			}
		}

		return lastGoal;
	}
}
