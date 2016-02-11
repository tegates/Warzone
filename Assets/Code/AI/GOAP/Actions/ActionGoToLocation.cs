using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionGoToLocation : GoapAction 
{
	public ActionGoToLocation(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();

	}

	public override bool ExecuteAction()
	{
		Vector3 target = ParentCharacter.MyAI.BlackBoard.NavTarget;
		ParentCharacter.MyAI.BlackBoard.NavTarget = target;
		ParentCharacter.Destination = target;
		ParentCharacter.SendCommand(HumanCharCommands.GoToPosition);

		return true;
	}

	public override void StopAction()
	{

	}

	public override bool CheckActionCompletion()
	{
		return true;
	}






	private bool CheckAvailability()
	{
		return true;
	}

}
