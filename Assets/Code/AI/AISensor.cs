using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISensor 
{

	private Character _parentCharacter;
	private WorkingMemory _workingMemory;




	public void Initialize(Character parent)
	{
		_parentCharacter = parent;
		_workingMemory = _parentCharacter.MyAI.WorkingMemory;
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdatePerSecond;
	}

	public void UpdatePerFrame()
	{

	}

	public void UpdatePerSecond()
	{
		if(_parentCharacter.MyAI.ControlType != AIControlType.Player)
		{
			UpdateWorkingMemoryCharacters();
		}
	}

	public bool GetTargetHittability(Character target)
	{
		GameObject myEyes = _parentCharacter.MyReference.Eyes;
		RaycastHit hit;
		float colliderHeight = target.GetComponent<CapsuleCollider>().height;
		Vector3 rayTarget = target.transform.position + Vector3.up * colliderHeight * 0.66f;
		Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
		if(Physics.Raycast(ray, out hit))
		{
			//Debug.Log("raycast hit in sensor: " + hit.collider.name);
			Character hitCharacter = hit.collider.GetComponent<Character>();
			if(hitCharacter == target)
			{
				return true;
			}
		}
		else
		{
			return false;
		}

		return false;
	}








	private void UpdateWorkingMemoryCharacters()
	{
		List<Character> characters = GetCharactersInSight();

		foreach(Character c in characters)
		{
			if(c.Faction != _parentCharacter.Faction)
			{
				//add/update enemy fact
				WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.KnownEnemy, c);
				if(fact == null)
				{
					//didn't find it in working memory, create a new fact
					fact = _workingMemory.AddFact(FactType.KnownEnemy, c, c.transform.position, 1, 0.01f);
					fact.ThreatLevel = 0.8f;
					fact.ThreatDropRate = 0.005f;
				}
				else
				{
					//found it in working memory, refresh confidence level
					fact.Confidence = 1;
				}


				CheckPersonalThreat(c);
			}
			else
			{
				//add/update friend fact
			}
		}
	}



	private List<Character> GetCharactersInSight()
	{
		//set the field of view and view range to a number for now; these will be part of char attributes
		float fov = 170;
		float range = 60;
		GameObject myEyes = _parentCharacter.MyReference.Eyes;
		List<Character> characters = new List<Character>();

		foreach(HumanCharacter c in GameManager.Inst.NPCManager.HumansInScene)
		{
			if(c == _parentCharacter)
			{
				continue;
			}

			//check if within range and fov
			if(Vector3.Distance(c.transform.position, _parentCharacter.transform.position) <= range
				&& Vector3.Angle(myEyes.transform.forward, (c.transform.position - _parentCharacter.transform.position)) <= fov / 2)
			{

				//now do a raycast check if this character is behind walls. ray direction would be towards 
				//1.5 meters above the feet of the character
				RaycastHit hit;
				float colliderHeight = c.GetComponent<CapsuleCollider>().height;
				Vector3 rayTarget = c.transform.position + Vector3.up * colliderHeight * 0.9f;
				Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
				Debug.DrawRay(myEyes.transform.position, rayTarget - myEyes.transform.position);
				if(Physics.Raycast(ray, out hit))
				{
					//Debug.Log("raycast hit in sensor: " + hit.collider.name);
					HumanCharacter hitCharacter = hit.collider.GetComponent<HumanCharacter>();
					if(hitCharacter != null && hitCharacter == c)
					{
						characters.Add(hitCharacter);
					}

				}
			}
		}

		return characters;
	}

	private void CheckPersonalThreat(Character c)
	{
		//check if enemy is aiming at me. Add personal threat memory.
		if(c.MyReference.CurrentWeapon != null)
		{
			if(c.MyReference.CurrentWeapon.GetComponent<Weapon>().IsRanged)
			{
				float aimAngle = Vector3.Angle(_parentCharacter.transform.position - c.transform.position, c.MyReference.CurrentWeapon.transform.forward);
				if(aimAngle < 15)
				{
					_workingMemory.AddFact(FactType.PersonalThreat, c, c.transform.position, 0.6f, 0.1f);
				}
			}

		}
	}
}
