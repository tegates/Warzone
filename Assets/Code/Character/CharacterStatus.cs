using UnityEngine;
using System.Collections;

public class CharacterStatus
{
	public float WalkSpeed;
	public float StrafeSpeed;
	public float RunSpeed;
	public float SprintSpeed;

	public float RunSpeedModifier; //0.9 to 1.2
	public float SprintSpeedModifier; //0.9 to 1.1
	public float StrafeSpeedModifier; //0.8 to 1.2

	public float MaxArmFatigue;
	public float ArmFatigue;

	public void Initialize()
	{
		WalkSpeed = 1.5f;
		StrafeSpeed = 2f;
		RunSpeed = 3.8f;
		SprintSpeed = 5f;

		ArmFatigue = 0;
		MaxArmFatigue = 5;

		ResetSpeedModifier();
	}

	public void ResetSpeedModifier()
	{
		RunSpeedModifier = 1.0f;
		SprintSpeedModifier = 1.1f;
		StrafeSpeedModifier = 1.2f;
	}

}
