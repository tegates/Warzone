using UnityEngine;
using System.Collections;

public class Gun : Weapon 
{

	public Transform MuzzleFlash;
	public Vector3 MuzzleFlashPosition;
	public GunReceiver Receiver;
	public GunBarrel Barrel;
	public GunStock Stock;

	public GunFireModes CurrentFireMode;


	private float _coolDownTimer;
	private bool _isCooledDown;
	private bool _isTriggerDown;

	private WeaponCallBack OnSuccessfulShot;

	void Update()
	{
		//this is a workaround for the bug when particle only follows root object and not child object in a rig
		MuzzleFlash.parent = this.transform;
		MuzzleFlash.localPosition = MuzzleFlashPosition;
		MuzzleFlash.parent = null;
		MuzzleFlash.forward = this.transform.forward * -1;

		//update cool down timer
		if(!_isCooledDown)
		{
			_coolDownTimer += Time.deltaTime;
			if(CurrentFireMode == GunFireModes.Burst && _coolDownTimer >= 1 / Receiver.BurstFireRate)
			{
				_isCooledDown = true;
				_coolDownTimer = 0;
			}
			else if(CurrentFireMode == GunFireModes.Semi && _coolDownTimer >= 1 / Receiver.SemiFireRate)
			{
				_isCooledDown = true;
				_coolDownTimer = 0;
			}
			else if(CurrentFireMode == GunFireModes.Full && _coolDownTimer >=  1 / Receiver.AutoFireRate)
			{
				_isCooledDown = true;
				_coolDownTimer = 0;
			}
		}

		//fire automatic
		if(CurrentFireMode == GunFireModes.Full && _isTriggerDown && _isCooledDown)
		{
			TriggerPull();
		}
	}


	public override void Rebuild(WeaponCallBack callBack)
	{
		Receiver = GetComponent<GunReceiver>();
		Barrel = GetComponent<GunBarrel>();
		Stock = GetComponent<GunStock>();

		_isCooledDown = true;

		OnSuccessfulShot = callBack;
	}

	public override float GetTotalLessMoveSpeed()
	{
		float value = 0;
		if(Receiver != null)
		{
			value += Receiver.LessMoveSpeed;
		}
		if(Barrel != null)
		{
			value += Barrel.LessMoveSpeed;
		}
		if(Stock != null)
		{
			value += Stock.LessMoveSpeed;
		}

		return value;
	}



	public float GetRecoil()
	{
		float value = 0;
		if(Receiver != null)
		{
			value += Receiver.Recoil;
		}
		if(Stock != null)
		{
			value -= Stock.LessRecoil;
		}

		return value;
	}



	public void TriggerPull()
	{
		if(_isCooledDown)
		{

			FlashMuzzle();
			_isCooledDown = false;
			_isTriggerDown = true;

			OnSuccessfulShot();
		}

	}

	public void TriggerRelease()
	{
		_isTriggerDown = false;
	}





	private void FlashMuzzle()
	{
		ParticleSystem sparks = MuzzleFlash.FindChild("Sparks").GetComponent<ParticleSystem>();
		ParticleSystem flame = MuzzleFlash.GetComponent<ParticleSystem>();
		ParticleSystem bulletTrail = MuzzleFlash.FindChild("BulletTrail").GetComponent<ParticleSystem>();
		Light light = MuzzleFlash.FindChild("Light").GetComponent<Light>();


		light.enabled = true;
		flame.Emit(1);
		sparks.Emit(1);
		bulletTrail.Emit(1);

		StartCoroutine(LightsOut(light));
	}

	IEnumerator LightsOut(Light light)
	{
		yield return new WaitForSeconds(0.05f);
		light.enabled = false;
	}
}
