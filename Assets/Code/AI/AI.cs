﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : MonoBehaviour 
{
	public GoapGoal CurrentGoal { get { return _currentGoal; } }

	public AIControlType ControlType;
	public AISensor Sensor;
	public GoapPlanner Planner;
	public WorkingMemory WorkingMemory;
	public BlackBoard BlackBoard;
	public AITargeting TargetingSystem;
	public AIWeapon WeaponSystem;

	public AIStateBase CurrentAIState;

	private Character _parentCharacter;
	private List<GoapGoal> _goals;
	private List<GoapAction> _actions;
	private GoapGoal _currentGoal;
	private GoapAction _currentAction;
	private List<GoapWorldState> _currentWorldStates;
	private Queue<GoapAction> _actionQueue;


	
	// Update is called from AI scheduler
	public void PerFrameUpdate () 
	{
		Sensor.UpdatePerFrame();
	}

	//this update is called directly without going through scheduler
	public void AlwaysPerFrameUpdate()
	{
		if(ControlType != AIControlType.Player)
		{
			TargetingSystem.UpdatePerFrame();

		}
		WeaponSystem.UpdatePerFrame();
	}

	public void Initialize(Character character)
	{
		_parentCharacter = character;

		WorkingMemory = new WorkingMemory();
		WorkingMemory.Initialize(_parentCharacter);

		BlackBoard = new BlackBoard();
		Sensor = new AISensor();
		Sensor.Initialize(_parentCharacter);
		TargetingSystem = new AITargeting();
		TargetingSystem.Initialize(_parentCharacter);
		WeaponSystem = new AIWeapon();
		WeaponSystem.Initialize(_parentCharacter);
		Planner = new GoapPlanner(this);


		_goals = GameManager.Inst.DBManager.DBHandlerAI.GetCharacterGoalSet(_parentCharacter.ID);
		_actions = GameManager.Inst.DBManager.DBHandlerAI.GetCharacterActionSet(_parentCharacter.ID);
		_currentWorldStates = new List<GoapWorldState>();

		_parentCharacter.MyEventHandler.OnNewEnemyTargetFound += OnImportantEvent;
		_parentCharacter.MyEventHandler.OnCurrentActionComplete += OnCurrentActionComplete;

		//update parent character for each action
		foreach(GoapAction action in _actions)
		{
			action.ParentCharacter = _parentCharacter;
		}

		BlackBoard.PatrolLoc = new Vector3(-15, 0, -15);
		BlackBoard.PatrolRange = new Vector3(20, 10, 20);
		BlackBoard.HasPatrolInfo = true;



		if(ControlType != AIControlType.Player)
		{
			_currentGoal = null;
			_currentAction = null;

			FindAndExecuteAction();
		}

	
	}









	public void SetCurrentWorldState(GoapWorldState state, object newValue)
	{
		GoapWorldState existing = (from s in _currentWorldStates where s.Name == state.Name select s).FirstOrDefault();
		if(existing != null)
		{
			existing.Value = newValue;
		}
		else
		{
			//if a world state with same name is not found in current list of world states, create a new one and add to current world states
			GoapWorldState newState = new GoapWorldState(state.ID, state.Name, state.Operator, newValue);
			_currentWorldStates.Add(newState);
		}
	}

	public List<GoapWorldState> GetCurrentWorldState()
	{
		return _currentWorldStates;
	}


	public bool CheckActionViable(GoapAction action)
	{
		//check if working memory's list of failed actions
		WorkingMemoryFact fact = WorkingMemory.FindExistingFact(FactType.FailedAction, action.Name);
		if(fact != null)
		{
			return false;
		}
		else
		{
			return true;
		}
	
		return true;
	}

	public object EvaluateWorldState(GoapWorldState state)
	{
		if(state.Name == "IsThreatInSightInArea")
		{
			//loop through all known enemies whose confidence is 1 (in sight)
			//return true if any is found

			List<WorkingMemoryFact> knownEnemies = WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
			foreach(WorkingMemoryFact fact in knownEnemies)
			{
				if(fact.Confidence >= 1)
				{
					//if enemy is in sight, return true even if not in area
					SetCurrentWorldState(state, true);
					return true;
				}

			}

			//now loop through all known neutrals whose confidence is 1
			//return true if any is within area
			List<WorkingMemoryFact> knownNeutrals = WorkingMemory.FindExistingFactOfType(FactType.KnownNeutral);
			foreach(WorkingMemoryFact fact in knownNeutrals)
			{
				if(fact.Confidence >= 1)
				{
					Vector3 position = ((Character)fact.Target).transform.position;

					//check if the neutral is within patrol range
					Vector3 targetLoc = BlackBoard.PatrolLoc;
					Vector3 targetRange = BlackBoard.PatrolRange;
					if(IsPosisionInArea(position, targetLoc, targetRange))
					{
						//Debug.Log("Evaluation IsThreatInSightInArea, true");
						SetCurrentWorldState(state, true);
						return true;
					}
				}
			}

			//Debug.Log("Evaluation IsThereThreatInArea, false");
			SetCurrentWorldState(state, false);
			return false;
		}

		else if(state.Name == "IsThereInvisibleEnemy")
		{
			bool result = (_parentCharacter.MyAI.BlackBoard.InvisibleEnemy != null);
			SetCurrentWorldState(state, result);

			return result;
		}

		else if(state.Name == "IsTargetAThreat")
		{
			//TO DO:
			//if target is not in sight, and if last known pos is inside area, return true;
			//if target is in sight, if target is enemy return true
			//if target is in sight, if target is neutral and still in area return true

			Character target = BlackBoard.TargetEnemy;
			if(target == null && BlackBoard.InvisibleEnemy != null)
			{
				target = BlackBoard.InvisibleEnemy;
			}

			WorkingMemoryFact fact = WorkingMemory.FindExistingFact(FactType.KnownEnemy, target);

			if(fact != null && fact.Confidence >= 1)
			{
				SetCurrentWorldState(state, true);
				return true;
			}
			else if(fact != null)
			{
				//check if last known pos is inside area
				Vector3 position = fact.LastKnownPos;

				//check if the neutral is within patrol range
				Vector3 targetLoc = BlackBoard.PatrolLoc;
				Vector3 targetRange = BlackBoard.PatrolRange;
				if(IsPosisionInArea(position, targetLoc, targetRange))
				{
					SetCurrentWorldState(state, true);
					return true;
				}
			}

			SetCurrentWorldState(state, false);
			return false;

		}

		else if(state.Name == "IsTherePersonalThreat")
		{
			bool result = BlackBoard.HighestPersonalThreat > 0;
			SetCurrentWorldState(state, result);
			return false;
		}

		else if(state.Name == "IsRangedWeaponEquipped")
		{
			if(BlackBoard.EquippedWeapon != null)
			{
				SetCurrentWorldState(state, true);
				return true;
			}
			else
			{
				SetCurrentWorldState(state, false);
				return false;
			}
		}

		else if(state.Name == "IsThreatInSight")
		{
			List<WorkingMemoryFact> knownEnemies = WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
			foreach(WorkingMemoryFact fact in knownEnemies)
			{
				if(fact.Confidence >= 1)
				{
					SetCurrentWorldState(state, true);
					return true;
				}
			}

			SetCurrentWorldState(state, false);
			return false;
		}

		else if(state.Name == "IsAmmoAvailable")
		{
			SetCurrentWorldState(state, true);
			return true;
		}

		return null;
	}



	#region events
	//AI Events
	public void OnImportantEvent()
	{
		//when an important event happens, stop current action and reevaluate all the goals and pick the highest priority goal
		if(_currentAction != null)
		{
			_currentAction.StopAction();
			_currentAction = null;
		}

		_currentGoal = null;
		FindAndExecuteAction();
	}

	public void OnCurrentActionComplete()
	{
		//Debug.Log("Action completed");
		if(_actionQueue.Count > 0)
		{
			_currentAction = _actionQueue.Dequeue();
			if(_currentAction.ExecuteAction())
			{
				//action executed successfully
				return;
			}
			else
			{
				_currentAction = null;
				FindAndExecuteAction();
			}
		}
		else
		{
			//Debug.Log("No more actions, evaluating goal");
			//now there are no more actions; evaluate the goal again. if goal is reached, then find new goal; if goal has not reached, rerun planner
			_currentAction = null;
			FindAndExecuteAction();
		}
	}


	#endregion


	private void AddGoal(GoapGoal goal)
	{
		if(_goals.Count < 5)
		{
			_goals.Add(goal);
		}
	}

	private GoapGoal GetNextGoal()
	{
		GoapGoal result = null;
		if(_currentGoal == null)
		{
			//get the highest priority goal
			GoapGoal goal = _goals.OrderBy(p => p.Priority).FirstOrDefault();
			result = goal;
		}
		else
		{
			//get the goal that's next highest than current goal
			var intermediate = (from g in _goals where g.Priority >= _currentGoal.Priority && g != _currentGoal orderby g.Priority select g);
			GoapGoal goal = intermediate.FirstOrDefault();

			result = goal;
		}

		return result;
	}

	private bool EvaluateGoal(GoapGoal goal)
	{
		//check which goal's conditions are met

		bool isGoalMet = true;
		foreach(GoapWorldState state in goal.GoalStates)
		{
			object value = EvaluateWorldState(state);
			if(value != state.Value)
			{
				//this goal isn't met!
				isGoalMet = false;
				return isGoalMet;
			}
		}

		return isGoalMet;
	}


	private void FindAndExecuteAction()
	{
		//check if there is current goal and if the current goal is met
		//if met, find next goal
		//if not met, check if there's current action. if there is action leave it alone. if no action then calculate planner and execute first action
		//if no current goal, get next goal

		//Debug.Log("Start finding and executing action");

		if(_currentGoal == null)
		{
			Debug.Log("no current goal, getting a new one");
			_currentGoal = GetNextGoal();

			Debug.Log("found new goal " + _currentGoal.Name);
			_currentAction = null;
		}

		int counter = _goals.Count;
		while(counter > 0 && _currentGoal != null)
		{
			//Debug.Log("Find&Execute: checking goal " + _currentGoal.Name);
			bool needNextGoal = false;
			if(_currentAction == null && _currentGoal != null)
			{
				if(!EvaluateGoal(_currentGoal))
				{
					//CsDebug.Inst.Log("current goal " + _currentGoal.Name + " is not met, running planner", CsDLevel.Info, CsDComponent.AI);
					_actionQueue = Planner.GetActionQueue(_currentGoal, _actions);
					if(_actionQueue != null && _actionQueue.Count > 0)
					{
						_currentAction = _actionQueue.Dequeue();
						//Debug.Log("Found current action " + _currentAction.Name + " for goal " + _currentGoal.Name); 
						if(_currentAction.ExecuteAction())
						{
							//action executed successfully
							return;
						}
					}
					//failed to find action for current goal, get next goal
					needNextGoal = true;
				}
				else
				{
					needNextGoal = true;
				}
			}
			else
			{
				//there's action; leave it alone
				return;
			}

			if(needNextGoal)
			{
				_currentGoal = GetNextGoal();
				//Debug.Log("getting next goal; result: " + _currentGoal.Name);
				_currentAction = null;
			}


			counter --;
		}
	}






	public static bool RandomPoint(Vector3 center, Vector3 range, out Vector3 result) 
	{
		for (int i = 0; i < 10; i++) 
		{
			Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(center.x - range.x, center.x + range.x),
												UnityEngine.Random.Range(center.y - range.y, center.y + range.y),
												UnityEngine.Random.Range(center.z - range.z, center.z + range.z));
			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) 
			{
				result = hit.position;
				return true;
			}
		}
		result = Vector3.zero;
		return false;
	}

	public static bool IsPosisionInArea(Vector3 position, Vector3 targetLoc, Vector3 targetRange)
	{
		if(position.x <= targetLoc.x + targetRange.x && position.x >= targetLoc.x - targetRange.x
			&& position.y <= targetLoc.y + targetRange.y && position.y >= targetLoc.y - targetRange.y
			&& position.z <= targetLoc.z + targetRange.z && position.z >= targetLoc.z - targetRange.z)
		{
			return true;
		}

		return false;
	}


}
