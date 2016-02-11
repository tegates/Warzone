using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionRangedAttack : GoapAction
{
	public ActionRangedAttack(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			return false;
		}
		Debug.Log("Start executing Ranged Attack action");

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;

		UpdateAction();

		return true;
	}

	public override void StopAction()
	{
		Debug.Log("Stop executing Ranged Attack");
		ParentCharacter.MyAI.WeaponSystem.StopFiringRangedWeapon();
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
	}

	public override bool CheckActionCompletion()
	{
		if(!ParentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable)
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

	public override bool CheckContextPrecondition ()
	{
		//here we are going to check if the target in blackboard is worthy of attack.
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemyThreat < 0.66f)
		{
			Debug.Log("Target enemy threat is too low for ranged attack");
			return false;
		}

		//check if the target center mass is exposed. 
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			if(ParentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable)
			{
				return true;
			}
			else
			{
				Debug.Log("target enemy is not null and target is not hittable");
				return false;
			}
		}

		return true;
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
		else
		{
			if(ParentCharacter.MyAI.WeaponSystem.AIWeaponState != AIWeaponStates.FiringRangedWeapon)
			{
				ParentCharacter.MyAI.WeaponSystem.StartFiringRangedWeapon();
			}
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
