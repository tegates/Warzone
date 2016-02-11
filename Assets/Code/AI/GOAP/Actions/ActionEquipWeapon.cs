using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionEquipWeapon : GoapAction
{
	private bool _isCompleted;


	public ActionEquipWeapon(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}


	public override bool CheckActionCompletion ()
	{

		return _isCompleted;
	}

	public override bool ExecuteAction ()
	{
		//if there's no focused weapon, then it means character has no weapon and action fails
		/*
		if(ParentCharacter.MyAI.BlackBoard.FocusedWeapon == null)
		{
			WorkingMemoryFact fact = ParentCharacter.MyAI.WorkingMemory.AddFact(FactType.FailedAction, null, Vector3.zero, 1, 0.1f);
			fact.PastAction = this.Name;
			return false;
		}
		*/
		Debug.Log("Start equip weapon action");
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;

		UpdateAction();
		return true;
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
		CsDebug.Inst.Log("Executing action " + this.Name + " sending command now", CsDLevel.Info, CsDComponent.AI);
		ParentCharacter.SendCommand(HumanCharCommands.SwitchWeapon2);

		_isCompleted = true;

		StopAction();

		ParentCharacter.MyEventHandler.TriggerOnActionCompletion();

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
