using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//this class selects from working memory a enemy target and feed to the black board
//also controls the looking direction and aiming

public class AITargeting 
{
	public AITargetingModes Mode;

	private Character _parentCharacter;
	private float _lookAroundAngle;


	public void Initialize(Character c)
	{
		_parentCharacter = c;
		Mode = AITargetingModes.LookAhead;
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdatePerSecond;
	}

	public void UpdatePerFrame()
	{
		Character currentTarget = _parentCharacter.MyAI.BlackBoard.TargetEnemy;


		//update looking and aiming position
		if(_parentCharacter.IsHuman && currentTarget != null && ((HumanCharacter)_parentCharacter).UpperBodyState == HumanUpperBodyStates.Aim 
			&& _parentCharacter.MyReference.CurrentWeapon != null)
		{
			Vector3 lookPos = currentTarget.GetComponent<HumanCharacter>().MyReference.Eyes.transform.position;
			_parentCharacter.LookTarget.transform.position = Vector3.Lerp(_parentCharacter.LookTarget.transform.position, lookPos, 8 * Time.deltaTime);

			Vector3 gunPos = _parentCharacter.MyReference.CurrentWeapon.transform.position;
			_parentCharacter.AimTargetRoot.position = gunPos;
			_parentCharacter.MyAI.BlackBoard.AimPoint = GetAimPointOnTarget(currentTarget);
			Vector3 aimDir = _parentCharacter.MyAI.BlackBoard.AimPoint - _parentCharacter.MyReference.CurrentWeapon.transform.position;
			Quaternion rotation = Quaternion.LookRotation(aimDir);
			_parentCharacter.AimTargetRoot.transform.rotation = Quaternion.Lerp(_parentCharacter.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 9);


		}
		else
		{
			float aimHeight = 1.5f;

			_parentCharacter.AimTargetRoot.position = _parentCharacter.transform.position + Vector3.up * 1.5f;
			Vector3 velocity = _parentCharacter.MyNavAgent.velocity;


			Vector3 aimDir = velocity.normalized;
			if(_parentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
			{
				if(Mode == AITargetingModes.LookAround)
				{
					aimDir = Quaternion.Euler(0, _lookAroundAngle, 0) * (velocity.magnitude > 0 ? velocity : _parentCharacter.transform.forward);
				}
				else
				{
					aimDir = _parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition - _parentCharacter.transform.position;
				}
			}
			else if(velocity.magnitude < 0.1f)
			{
				aimDir = _parentCharacter.transform.forward;
			}



			Quaternion rotation = Quaternion.LookRotation(aimDir);
			_parentCharacter.AimTargetRoot.transform.rotation = Quaternion.Lerp(_parentCharacter.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 6);

			_parentCharacter.MyAI.BlackBoard.AimPoint = _parentCharacter.AimTarget.position;

			Vector3 lookPos = _parentCharacter.transform.position + aimDir * 2 + new Vector3(0, aimHeight, 0);
			_parentCharacter.LookTarget.transform.position = Vector3.Lerp(_parentCharacter.LookTarget.transform.position, lookPos, 8 * Time.deltaTime);
		}


	}

	public void UpdatePerSecond()
	{
		if(Mode == AITargetingModes.LookAround)
		{
			_lookAroundAngle = UnityEngine.Random.Range(-90, 90);
		}

		//check current target. If it's still in sight (confidence 1) increase threat level
		//if no longer in sight, move it to invisible enemy
		//if no longer in memory then remove
		if(_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			WorkingMemoryFact currentTargetFact = _parentCharacter.MyAI.WorkingMemory.FindExistingFact(_parentCharacter.MyAI.BlackBoard.TargetEnemy);
			if(currentTargetFact != null && currentTargetFact.Confidence >= 1)
			{
				currentTargetFact.ThreatLevel += 0.03f;
				if(currentTargetFact.ThreatLevel > 1)
				{
					currentTargetFact.ThreatLevel = 1;
				}
				_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = currentTargetFact.ThreatLevel;
				_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
				_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = currentTargetFact.IsHittable;

				return;
			}
			else if(currentTargetFact != null)
			{
				//when confidence is low, move enemy to invisible (I know there's this guy but I don't see him)
				_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = _parentCharacter.MyAI.BlackBoard.TargetEnemy;
				currentTargetFact.LastKnownPos = _parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
				_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
				_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = currentTargetFact.IsHittable;

				currentTargetFact.ThreatLevel -= currentTargetFact.ThreatDropRate;
				if(currentTargetFact.ThreatLevel < 0)
				{
					currentTargetFact.ThreatLevel = 0;
				}
			}
			else
			{
				_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
				_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
				_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = false;
			}
		}



		//from working memory look for biggest enemy threat
		//if no threat present, look for nearest uninvestigated item
		List<WorkingMemoryFact> enemyFacts = _parentCharacter.MyAI.WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
		if(enemyFacts.Count > 0)
		{
			//for now just get the closest enemy in sight
			WorkingMemoryFact selected = enemyFacts.OrderBy(p => (p.LastKnownPos - _parentCharacter.transform.position)).FirstOrDefault();
			if(selected != null)
			{ 
				
				bool isNewTarget = (_parentCharacter.MyAI.BlackBoard.TargetEnemy != (Character)selected.Target && selected.Confidence >= 1);


				if(isNewTarget)
				{
					_parentCharacter.MyAI.BlackBoard.TargetEnemy = (Character)selected.Target;
					_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
					_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = selected.IsHittable;
					selected.LastKnownPos = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
					_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
					//trigger when a new target is selected
					CsDebug.Inst.Log("AITargeting found a new target!", CsDLevel.Info, CsDComponent.AI);
					_parentCharacter.MyEventHandler.TriggerOnNewEnemyTargetFound();
				}
			}

		}
		else
		{
			CsDebug.Inst.Log("AITarget has cleared the target", CsDLevel.Debug, CsDComponent.AI);
			_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
			_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
			_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = false;

		}
	}

	public Vector3 GetAimPointOnTarget(Character target)
	{
		float colliderHeight = target.GetComponent<CapsuleCollider>().height;
		return target.transform.position + Vector3.up * colliderHeight * 0.66f;
	}
}

public enum AITargetingModes
{
	LookAhead,
	LookAround,
}
