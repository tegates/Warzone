using UnityEngine;
using System.Collections;

public class CharacterEventHandler : MonoBehaviour 
{
	
	public delegate void AnimationEventDelegate();
	public event AnimationEventDelegate OnThrowLeaveHand;
	public event AnimationEventDelegate OnThrowFinish;
	public event AnimationEventDelegate OnPistolPullOutFinish;
	public event AnimationEventDelegate OnPistolPutAwayFinish;
	public event AnimationEventDelegate OnLongGunPullOutFinish;
	public event AnimationEventDelegate OnLongGunPutAwayFinish;
	public event AnimationEventDelegate OnReloadFinish;
	public event AnimationEventDelegate OnBulletInjury;
	public event AnimationEventDelegate OnDeath;

	public delegate void AIEventDelegate();
	public event AIEventDelegate OnNewEnemyTargetFound;
	public event AIEventDelegate OnOneSecondTimer;
	public event AIEventDelegate OnCurrentActionComplete;

	//Animation events
	public void TriggerOnThrowLeaveHand()
	{
		if(OnThrowLeaveHand != null)
		{
			OnThrowLeaveHand();
		}
	}

	public void TriggerOnThrowFinish()
	{
		if(OnThrowFinish != null)
		{
			OnThrowFinish();
		}
	}

	public void TriggerOnLongGunPullOutFinish()
	{
		if(OnLongGunPullOutFinish != null)
		{
			OnLongGunPullOutFinish();
		}
	}

	public void TriggerOnLongGunPutAwayFinish()
	{
		if(OnLongGunPutAwayFinish != null)
		{
			OnLongGunPutAwayFinish();
		}
	}

	public void TriggerOnPistolPullOutFinish()
	{
		if(OnPistolPullOutFinish != null)
		{
			OnPistolPullOutFinish();
		}
	}

	public void TriggerOnPistolPutAwayFinish()
	{
		if(OnPistolPutAwayFinish != null)
		{
			OnPistolPutAwayFinish();
		}
	}

	public void TriggerOnReloadFinish()
	{
		if(OnReloadFinish != null)
		{
			OnReloadFinish();
		}
	}

	public void TriggerOnBulletInjury()
	{
		if(OnBulletInjury != null)
		{
			OnBulletInjury();
		}
	}

	public void TriggerOnDeath()
	{
		if(OnDeath != null)
		{
			OnDeath();
		}
	}

	//AI events
	public void TriggerOnNewEnemyTargetFound()
	{
		if(OnNewEnemyTargetFound != null)
		{
			OnNewEnemyTargetFound();
		}
	}

	public void TriggerOnOneSecondTimer()
	{
		if(OnOneSecondTimer != null)
		{
			OnOneSecondTimer();
		}
	}

	public void TriggerOnActionCompletion()
	{
		if(OnCurrentActionComplete != null)
		{
			OnCurrentActionComplete();
		}
	}

}
