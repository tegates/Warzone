using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionPatrolArea : GoapAction 
{
	private bool _isPatrolling;

	public ActionPatrolArea(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if( !ParentCharacter.MyAI.BlackBoard.HasPatrolInfo)
		{
			return false;
		}
		Debug.Log("Starting to execute Patrol Area");
		_isPatrolling = false;

		UpdateAction();
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;


		return true;
	}

	public override void StopAction()
	{
		//CsDebug.Inst.Log("Action patrol area is completed!", CsDLevel.Info, CsDComponent.AI);
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
		if(ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(HumanCharCommands.StopAim);
		}
		ParentCharacter.SendCommand(HumanCharCommands.Idle);


	}

	public override bool CheckActionCompletion()
	{
		foreach(GoapWorldState state in Effects)
		{
			
			object result = ParentCharacter.MyAI.EvaluateWorldState(state);
			//Debug.Log("Checking if state " + state.Name + " value is " + state.Value + " result: " + result);
			if(!result.Equals(state.Value))
			{
				//Debug.Log("result is not equal to effect");
				return false;
			}
		}

		return true;
	}

	public void UpdateAction()
	{
		if(!CheckAvailability())
		{
			return;
		}

		if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
		{
			//Debug.Log("Patrol area update action nav target is set. is patrolling " + _isPatrolling);
			//check if is near patrol destination; if so set isNavTargetSet to false
			if(Vector3.Distance(ParentCharacter.transform.position, ParentCharacter.MyAI.BlackBoard.NavTarget) <= 2)
			{
				ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
			}
			else if(!_isPatrolling)
			{
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(HumanCharCommands.GoToPosition);
				_isPatrolling = true;
			}
		}
		else
		{
			Debug.Log("Patrol area update action nav target is not set");
			Vector3 result;
			ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = SelectPatrolDestination(out result);
			if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
			{
				ParentCharacter.MyAI.BlackBoard.NavTarget = result;
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(HumanCharCommands.GoToPosition);
				_isPatrolling = true;
			}
		}

		if(ParentCharacter.MyReference.CurrentWeapon != null)
		{
			ParentCharacter.SendCommand(HumanCharCommands.Unarm);
		}

		//check if patrol is complete
		if(CheckActionCompletion())
		{
			StopAction();
		}
	}




	private bool CheckAvailability()
	{
		if(ParentCharacter.IsBodyLocked || !ParentCharacter.MyAI.BlackBoard.HasPatrolInfo)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private bool SelectPatrolDestination(out Vector3 result)
	{
		//first find a random position within range
		if(AI.RandomPoint(ParentCharacter.MyAI.BlackBoard.PatrolLoc, ParentCharacter.MyAI.BlackBoard.PatrolRange, out result))
		{
			return true;
		}

		result = Vector3.zero;
		return false;
	}
}
