using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkingMemory 
{
	private Character _parentCharacter;
	public HashSet<WorkingMemoryFact> Facts;

	public WorkingMemory()
	{
		

	}

	public void Initialize(Character parent)
	{
		_parentCharacter = parent;
		Facts = new HashSet<WorkingMemoryFact>();

		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateFact;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdateFact;
	}

	public void UpdateFact()
	{
		//every second we will reduce the confidence value of each fact by reduce rate until
		//zero, and then remove (forget) it
		HashSet<WorkingMemoryFact> copy = new HashSet<WorkingMemoryFact>(Facts);
		foreach(WorkingMemoryFact fact in copy)
		{
			fact.Confidence -= fact.ConfidenceDropRate;
			if(fact.Confidence <= 0)
			{
				Debug.Log("Working memory removing fact of " + fact.FactType + " confidence " + fact.Confidence);
				RemoveFact(fact.FactType, fact.Target);
			}

			if(fact.FactType == FactType.KnownEnemy || fact.FactType == FactType.KnownNeutral || fact.FactType == FactType.KnownBeast)
			{
				fact.IsHittable = _parentCharacter.MyAI.Sensor.GetTargetHittability((Character)fact.Target);
			}

		}
	}

	public WorkingMemoryFact AddFact(FactType type, object target, Vector3 lastKnownPos, float confidence, float dropRate)
	{
		WorkingMemoryFact fact = new WorkingMemoryFact();
		fact.Confidence = confidence;
		fact.ConfidenceDropRate = dropRate;
		fact.FactType = type;
		fact.Target = target;
		Facts.Add(fact);

		return fact;
	}


	public void RemoveFact(FactType type, object target)
	{
		WorkingMemoryFact fact = null;
		foreach(WorkingMemoryFact f in Facts)
		{
			if(f.FactType == type && f.Target == target)
			{
				fact = f;
			}
		}
		if(fact != null)
		{
			Facts.Remove(fact);
		}
	}

	public bool CompareFact(WorkingMemoryFact fact1, WorkingMemoryFact fact2)
	{
		//returns true if the facts are the same 
		// check if types and handle are same
		if(fact1.FactType == fact2.FactType && fact1.Target == fact2.Target)
		{
			return true;
		}

		return false;
	}

	public WorkingMemoryFact FindExistingFact(FactType type, object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(type == f.FactType && target == f.Target)
			{
				return f;
			}
		}

		return null;
	}

	public WorkingMemoryFact FindExistingFact(object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(target == f.Target)
			{
				return f;
			}
		}

		return null;
	}

	public List<WorkingMemoryFact> FindExistingFactOfType(FactType type)
	{
		List<WorkingMemoryFact> facts = new List<WorkingMemoryFact>();
		foreach(WorkingMemoryFact f in Facts)
		{
			if(type == f.FactType)
			{
				facts.Add(f);
			}
		}

		return facts;
	}

	public WorkingMemoryFact FindFailedActionFact(GoapAction action, object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(f.FactType == FactType.FailedAction && target == f.Target && f.PastAction == action.Name)
			{
				return f;
			}
		}

		return null;
	}

}

public class WorkingMemoryFact
{
	public FactType FactType;
	public object Target;
	public Vector3 LastKnownPos;//use this when confidence is low enough
	public string PastAction;//only for fact type failedAction
	public float Confidence;
	public float ConfidenceDropRate;
	public float ThreatLevel;
	public float ThreatDropRate;
	public bool IsHittable;
}

public enum FactType
{
	KnownEnemy,
	KnownFriend,
	KnownNeutral,
	KnownBeast,
	NearbyExplosive,
	NearbyContainer,
	NearbyPickupObject,
	NearbyNavNode,
	NearbySmartObject,
	PersonalThreat,
	Event,
	FailedAction,
}