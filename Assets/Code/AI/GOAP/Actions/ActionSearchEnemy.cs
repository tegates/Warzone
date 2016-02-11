using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionSearchEnemy : GoapAction
{
	private bool _isSearchDestSet;
	private Vector3 _searchDest;


	public ActionSearchEnemy(string name, string description, float cost)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_preconditions = new List<GoapWorldState>();
		_effects = new List<GoapWorldState>();
	}

	public override bool ExecuteAction()
	{
		if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			return false;
		}
			
		Debug.Log("Start executing Search Enemy");

		ParentCharacter.MyAI.BlackBoard.NavTarget = ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
		ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = true;

		WorkingMemoryFact fact = ParentCharacter.MyAI.WorkingMemory.FindExistingFact (ParentCharacter.MyAI.BlackBoard.InvisibleEnemy);
		float threat = fact.ThreatLevel;

		//if threat is low, just run towards last known location
		//if threat is high, keep aiming and go carefully towards a distance away from the last known location

		if (threat >= 0.66f)
		{
			//if enemy didn't move or didn't move much, stay and wait (go to camp action)
			//if enemy moved away, select a location near the opposite direction of the enemy's actual position
			//near the last known pos and go there
			Vector3 dist = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy.transform.position - ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
			if(dist.magnitude < 0.1f)
			{

			}
			else
			{
				_isSearchDestSet = SelectSearchDestination(ParentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition + dist.normalized * -4, new Vector3(3, 3, 3), out _searchDest);

			}

			ParentCharacter.SendCommand(HumanCharCommands.Aim);
			((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
		}
		else
		{

			_searchDest = ParentCharacter.MyAI.BlackBoard.NavTarget;
			_isSearchDestSet = true;
			ParentCharacter.SendCommand(HumanCharCommands.StopAim);
			((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Run;
		}

		UpdateAction();

		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;
		ParentCharacter.MyEventHandler.OnOneSecondTimer += UpdateAction;

		return true;
	}

	public override void StopAction()
	{
		CsDebug.Inst.Log("Search enemy is completed!", CsDLevel.Info, CsDComponent.AI);
		ParentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateAction;

		ParentCharacter.MyAI.TargetingSystem.Mode = AITargetingModes.LookAhead;
		((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
		ParentCharacter.SendCommand(HumanCharCommands.StopAim);
		ParentCharacter.SendCommand(HumanCharCommands.Idle);

	}

	public override bool CheckActionCompletion()
	{
		//if both enemy target and invisible targets are null, complete
		if(ParentCharacter.MyAI.BlackBoard.TargetEnemy == null && ParentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			Debug.Log("Both target enemy and invisible enemy are null");
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

	public void UpdateAction()
	{
		if(!CheckAvailability())
		{
			return;
		}

		if(_isSearchDestSet)
		{
			if(Vector3.Distance(ParentCharacter.transform.position, _searchDest) > 1)
			{
				ParentCharacter.Destination = _searchDest;
				ParentCharacter.SendCommand(HumanCharCommands.GoToPosition);
			}
			else
			{
				_isSearchDestSet = false;
				ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = false;
			}
		}
		else
		{
			//search random locations
			Vector3 searchCenter = ParentCharacter.transform.position;
			if(ParentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
			{
				searchCenter = ParentCharacter.MyAI.BlackBoard.InvisibleEnemy.transform.position;
			}
			ParentCharacter.MyAI.BlackBoard.IsNavTargetSet = SelectSearchDestination(searchCenter, new Vector3(5, 5, 5), out _searchDest);
			_isSearchDestSet = ParentCharacter.MyAI.BlackBoard.IsNavTargetSet;
			if(ParentCharacter.MyAI.BlackBoard.IsNavTargetSet)
			{
				ParentCharacter.MyAI.BlackBoard.NavTarget = _searchDest;
				ParentCharacter.Destination = ParentCharacter.MyAI.BlackBoard.NavTarget;
				((HumanCharacter)ParentCharacter).CurrentStance = HumanStances.Walk;
				ParentCharacter.SendCommand(HumanCharCommands.Aim);
				ParentCharacter.SendCommand(HumanCharCommands.GoToPosition);
				ParentCharacter.MyAI.TargetingSystem.Mode = AITargetingModes.LookAround;
			}
		}
			



		//check if patrol is complete
		if(CheckActionCompletion())
		{
			StopAction();
			ParentCharacter.MyEventHandler.TriggerOnActionCompletion();
		}
	}




	private bool CheckAvailability()
	{
		if(ParentCharacter.IsBodyLocked)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	private bool SelectSearchDestination(Vector3 center, Vector3 range, out Vector3 result)
	{
		//first find a random position within range
		if(AI.RandomPoint(center, range, out result))
		{
			return true;
		}

		result = Vector3.zero;
		return false;
	}
}
