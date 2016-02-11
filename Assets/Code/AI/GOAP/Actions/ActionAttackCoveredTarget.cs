using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionAttackCoveredTarget : GoapAction
{

	public ActionAttackCoveredTarget(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}


	public override bool CheckActionCompletion ()
	{
		if(ParentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable)
		{
			return true;
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return true;
		}

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

	public override bool ExecuteAction ()
	{
		//this action makes agent continue to aim at target enemy. if there's grenade available throw grenade
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return false;
		}

		Debug.Log("Start executing Ranged Attack Covered Target");



		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;

		UpdateAction();
		return true;
	}

	public override bool CheckContextPrecondition ()
	{
		//here we are going to check if the target in blackboard is worthy of attack.
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat < 0.66f)
		{
			return false;
		}
		//check if the target center mass is exposed. 
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null && !ParentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void StopAction ()
	{
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

	}

	public void UpdateAction()
	{
		//continue to check if body is locked, i.e. wait till it's not locked
		if(!CheckAvailability())
		{
			return;
		}

		if(((HumanCharacter)ParentCharacter).UpperBodyState != HumanUpperBodyStates.Aim)
		{
			ParentCharacter.SendCommand(HumanCharCommands.Aim);
		}

		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			ParentCharacter.AimPoint = ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position 
				+ (ParentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position - ParentCharacter.transform.position).normalized * 2;
			ParentCharacter.SendCommand(HumanCharCommands.ThrowGrenade);
		}


		if(CheckActionCompletion())
		{
			StopAction();

			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}

	}


	private bool CheckAvailability()
	{
		//check if body is locked
		if(ParentCharacter.IsBodyLocked)
		{
			return false;
		}
		else
		{
			return true;
		}

	}
}
