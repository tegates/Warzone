using UnityEngine;
using System.Collections;

public class AIWeapon
{
	public AIWeaponStates AIWeaponState;

	private Character _parentCharacter;
	private AIWeaponTriggerState _triggerState;


	public void Initialize(Character c)
	{
		_parentCharacter = c;
		
	}

	public void UpdatePerFrame()
	{
		if(AIWeaponState == AIWeaponStates.FiringRangedWeapon)
		{
			HandleFiringRangedWeapon();
		}

	}

	public void StartFiringRangedWeapon()
	{
		AIWeaponState = AIWeaponStates.FiringRangedWeapon;
	}

	public void StopFiringRangedWeapon()
	{
		if(_parentCharacter.MyReference.CurrentWeapon != null)
		{
			_parentCharacter.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerRelease();
		}
		AIWeaponState = AIWeaponStates.None;
		_triggerState = AIWeaponTriggerState.Released;
	}






	private void HandleFiringRangedWeapon()
	{
		/*
		if(_parentCharacter.MyAI.BlackBoard.TargetEnemy == null)
		{
			StopFiringRangedWeapon();
			return;
		}
		*/

		float aimAngleThreshold = 5;
		Vector3 aimPoint = _parentCharacter.MyAI.BlackBoard.AimPoint;
		float aimAngle = Vector3.Angle(aimPoint - _parentCharacter.MyReference.CurrentWeapon.transform.position, _parentCharacter.MyReference.CurrentWeapon.transform.forward);
		Debug.Log(aimAngle + " " + _triggerState);
		if(aimAngle > aimAngleThreshold / 4 && _triggerState == AIWeaponTriggerState.WaitForRecoil)
		{

		}
		else if(aimAngle < aimAngleThreshold && _triggerState != AIWeaponTriggerState.Pulled)
		{
			_parentCharacter.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerPull();
			_triggerState = AIWeaponTriggerState.Pulled;
		}
		else if(aimAngle >= aimAngleThreshold && _triggerState == AIWeaponTriggerState.Pulled)
		{
			_parentCharacter.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerRelease();
			_triggerState = AIWeaponTriggerState.WaitForRecoil;
		}
	}
}

public enum AIWeaponStates
{
	None,
	FiringRangedWeapon,

}

public enum AIWeaponTriggerState
{
	WaitForRecoil,
	Pulled,
	Released,
}