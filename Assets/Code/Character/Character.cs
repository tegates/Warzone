using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public abstract class Character : MonoBehaviour
{
	public int ID;
	public bool IsHuman;
	public Vector3? Destination;
	public Vector3 AimPoint;

	public Animator MyAnimator;

	public Faction Faction;

	public CharacterReference MyReference;
	public CharacterEventHandler MyEventHandler;

	public CharacterStatus MyStatus;
	public bool IsBodyLocked;


	public AI MyAI;

	public NavMeshAgent MyNavAgent;


	public Transform AimTarget;
	public Transform AimTargetRoot;
	public Transform AimTransform;
	public Transform LookTarget;

	public abstract void SendCommand(HumanCharCommands command);

	/*
	public void HandleNavAgentMovement()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.SetDestination(Destination.Value);
		
		if(IsSneaking)
		{
			agent.speed = MovingSpeed * 0.3f;
		}
		else
		{
			
			agent.speed = MovingSpeed * MovingSpeedMultiplier;
		}
		
		if(IsSneaking)
		{
			Noise.Volume = 0.2f;
		}
		else
		{
			Noise.Volume = 0.4f;
		}
		Noise.Location = transform.position;
		SoundEventHandler.Instance.TriggerNoiseEvent(Noise);
		
		if(!IsWeaponFiring && !IsWeaponAiming)
		{
			GetComponent<NavMeshAgent>().updateRotation = true;
		}
	}
	*/

}
