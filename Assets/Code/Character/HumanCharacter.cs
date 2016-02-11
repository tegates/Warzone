using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class HumanCharacter : Character
{
	public LeftHandIKControl MyLeftHandIK;
	public AimIK MyAimIK;
	public LookAtIK MyHeadIK;





	public HumanAnimStateBase CurrentAnimState;

	public HumanStances CurrentStance;
	public HumanUpperBodyStates UpperBodyState;
	public HumanActionStates ActionState;




	private bool _isThrowing;
	private bool _isSwitchingWeapon;
	private Vector3 _throwTarget;
	private Vector3 _throwDir;
	private ThrownObject _thrownObjectInHand;



	private bool _layerDState;//false = decreasing; true = increasing


	void Update()
	{
		CurrentAnimState.Update();

		UpdateLookDirection();

		UpdateDestBodyAngle();

		UpdateFatigue();

		MyAI.AlwaysPerFrameUpdate();
	}

	

	
	void LateUpdate() 
	{
		//adjust aim direction for recoil
		if(this.MyReference.CurrentWeapon != null && UpperBodyState == HumanUpperBodyStates.Aim)
		{

			Vector3 originalDir = AimTransform.transform.forward;
			Vector3 dir = new Vector3(originalDir.x, 0, originalDir.z);
			MyAimIK.solver.axis = Vector3.Lerp(MyAimIK.solver.transform.InverseTransformDirection(originalDir), MyAimIK.solver.transform.InverseTransformDirection(dir), Time.deltaTime * 0.1f);

			//recover from recoil
			float recoverRate = 8 * (1 - (this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue));
			AimTarget.localPosition = Vector3.Lerp(AimTarget.localPosition, new Vector3(0, 0, 2), Time.deltaTime * recoverRate);
		}



	}


	public void Initialize()
	{
		this.MyAnimator = GetComponentInChildren<Animator>();
		this.MyReference = GetComponentInChildren<CharacterReference>();
		this.MyEventHandler = GetComponentInChildren<CharacterEventHandler>();

		this.MyReference.ParentCharacter = this;

		//setup animator parameters initial values
		this.MyAnimator.SetBool("IsAiming", false);
		this.MyAnimator.SetBool("IsSneaking", false);

		this.Destination = transform.position;
		MyNavAgent = GetComponent<NavMeshAgent>();
		UpperBodyState = HumanUpperBodyStates.Idle;
		ActionState = HumanActionStates.None;

		//load aim target and look target
		GameObject aimTarget = (GameObject)GameObject.Instantiate(Resources.Load("IKAimTargetRoot"));
		GameObject lookTarget = (GameObject)GameObject.Instantiate(Resources.Load("IKLookTarget"));
		AimTargetRoot = aimTarget.transform;
		AimTarget = AimTargetRoot.Find("IKAimTarget").transform;
		LookTarget = lookTarget.transform;

		this.MyAimIK = GetComponentInChildren<AimIK>();
		this.MyAimIK.solver.target = AimTarget;
		this.MyAimIK.solver.IKPositionWeight = 0;
		this.MyAimIK.solver.SmoothDisable();

		this.MyLeftHandIK = GetComponentInChildren<LeftHandIKControl>();
		this.MyLeftHandIK.Initialize();

		this.MyHeadIK = GetComponentInChildren<LookAtIK>();
		this.MyHeadIK.solver.target = LookTarget;

		this.MyStatus = new CharacterStatus();
		this.MyStatus.Initialize();



		//each time a human char is initialized it's added to NPC manager's list of human characters to keep track of
		GameManager.Inst.NPCManager.AddHumanCharacter(this);

		CurrentAnimState = new HumanAnimStateIdle(this);
		//SendCommand(HumanCharCommands.Unarm);

		MyAI = GetComponent<AI>();
		MyAI.Initialize(this);

		//subscribe events
		this.MyEventHandler.OnLongGunPullOutFinish += OnLongGunPullOutFinish;
		this.MyEventHandler.OnLongGunPutAwayFinish += OnLongGunPutAwayFinish;
		this.MyEventHandler.OnPistolPullOutFinish += OnPistolPullOutFinish;
		this.MyEventHandler.OnPistolPutAwayFinish += OnPistolPutAwayFinish;
		this.MyEventHandler.OnReloadFinish += OnReloadFinish;
		this.MyEventHandler.OnThrowFinish += OnThrowFinish;
		this.MyEventHandler.OnThrowLeaveHand += OnThrowLeaveHand;
		this.MyEventHandler.OnBulletInjury += OnBulletInjury;
		this.MyEventHandler.OnDeath += OnDeath;
	}

	public override void SendCommand(HumanCharCommands command)
	{
		//following commands are not given by AI or user. All commands that will unlock the body go here



		if(IsBodyLocked)
		{
			return;
		}

		//following commands are given by AI or user, and can be locked
		CurrentAnimState.SendCommand(command);

		if(command == HumanCharCommands.Crouch)
		{
			CapsuleCollider collider = GetComponent<CapsuleCollider>();
			collider.height = 1.3f;
			collider.center = new Vector3(0, 0.6f, 0);

		}

		if(command == HumanCharCommands.StopCrouch)
		{
			CapsuleCollider collider = GetComponent<CapsuleCollider>();
			collider.height = 1.7f;
			collider.center = new Vector3(0, 1, 0);
		}

		if(command == HumanCharCommands.Aim && GetCurrentAnimWeapon() != WeaponAnimType.Unarmed && CurrentStance != HumanStances.Sprint)
		{
			if(ActionState == HumanActionStates.None)
			{
				UpperBodyState = HumanUpperBodyStates.Aim;
				MyAimIK.solver.SmoothEnable(2.5f);
				MyLeftHandIK.SmoothEnable();
				MyHeadIK.solver.SmoothDisable();
				MyAnimator.SetBool("IsAiming", true);
				//StartCoroutine(WaitAndEnableAimIK(0.2f));
			}
			else if(ActionState == HumanActionStates.SwitchWeapon)
			{
				UpperBodyState = HumanUpperBodyStates.Aim;
			}
		}

		if(command == HumanCharCommands.StopAim && ActionState == HumanActionStates.None)
		{
			UpperBodyState = HumanUpperBodyStates.Idle;
			MyAimIK.solver.SmoothDisable(9);
			MyHeadIK.solver.SmoothEnable();
			MyAnimator.SetBool("IsAiming", false);

			if(GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
			{
				MyLeftHandIK.SmoothDisable(9);
			}
			//Debug.Log("stopping aim");
		}

		if(command == HumanCharCommands.Sprint)
		{


			if(CurrentStance == HumanStances.Crouch || CurrentStance == HumanStances.CrouchRun)
			{
				CurrentStance = HumanStances.CrouchRun;
			}
			else
			{
				CurrentStance = HumanStances.Sprint;
				MyAimIK.solver.SmoothDisable();
				MyHeadIK.solver.SmoothDisable();
			}
		}

		if(command == HumanCharCommands.StopSprint)
		{
			if(UpperBodyState == HumanUpperBodyStates.Aim)
			{
				MyAimIK.solver.SmoothEnable();
			}

			if(CurrentStance == HumanStances.CrouchRun || CurrentStance == HumanStances.Crouch)
			{
				CurrentStance = HumanStances.Crouch;
			}
			else
			{
				CurrentStance = HumanStances.Run;
			}
			MyHeadIK.solver.SmoothEnable();
		}

		if(command == HumanCharCommands.SwitchWeapon2)
		{
			Debug.Log("current human action state " + ActionState);
			if(ActionState == HumanActionStates.None)
			{
				MyLeftHandIK.SmoothDisable(15);
				MyAimIK.solver.SmoothDisable(9);
				MyAnimator.SetInteger("WeaponType", 2);
				SwitchWeapon("AK47");

				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == HumanCharCommands.SwitchWeapon1)
		{
			if(ActionState == HumanActionStates.None)
			{
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					//MyLeftHandIK.SmoothEnable();
				}
				else
				{
					
				}
				MyLeftHandIK.SmoothDisable(15);
				MyAimIK.solver.SmoothDisable(9);
				MyAnimator.SetInteger("WeaponType", 1);
				SwitchWeapon("44MagnumRevolver");

				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == HumanCharCommands.Unarm)
		{
			if(ActionState == HumanActionStates.None)
			{
				MyLeftHandIK.SmoothDisable();
				UpperBodyState = HumanUpperBodyStates.Idle;
				MyAimIK.solver.SmoothDisable();
				MyHeadIK.solver.SmoothEnable();
				MyAnimator.SetBool("IsAiming", false);
				MyAnimator.SetInteger("WeaponType", 0);
				SwitchWeapon("");

				ActionState = HumanActionStates.SwitchWeapon;
			}
		}

		if(command == HumanCharCommands.PullTrigger)
		{
			if(ActionState != HumanActionStates.None || UpperBodyState != HumanUpperBodyStates.Aim)
			{
				return;
			}

			if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
			{
				//
				this.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerPull();


			}
		}

		if(command == HumanCharCommands.ReleaseTrigger)
		{
			if(ActionState != HumanActionStates.None || UpperBodyState != HumanUpperBodyStates.Aim)
			{
				return;
			}

			if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
			{
				//
				this.MyReference.CurrentWeapon.GetComponent<Gun>().TriggerRelease();


			}
		}


		if(command == HumanCharCommands.Reload)
		{
			if(ActionState == HumanActionStates.None && this.MyReference.CurrentWeapon != null)
			{
				if(GetCurrentAnimWeapon() == WeaponAnimType.Longgun || GetCurrentAnimWeapon() == WeaponAnimType.Pistol)
				{
					MyAimIK.solver.SmoothDisable();
					MyAnimator.SetTrigger("Reload");
					
					MyLeftHandIK.SmoothDisable();

				}

				MyHeadIK.solver.SmoothDisable();
					
				ActionState = HumanActionStates.Reload;

			}
		}



		if(command == HumanCharCommands.CancelReload)
		{
			if(ActionState == HumanActionStates.Reload && this.MyReference.CurrentWeapon != null)
			{
				Debug.Log("cancel reload");
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					MyAimIK.solver.SmoothEnable();
					MyAnimator.SetTrigger("CancelReload");
				}
				else
				{
					MyAnimator.SetTrigger("CancelReload");
				}

				if(MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
				{
					MyLeftHandIK.SmoothEnable();
				}
				else
				{
					Debug.Log("done reloading pistol " + UpperBodyState);
					if(UpperBodyState == HumanUpperBodyStates.Aim)
					{
						MyLeftHandIK.SmoothEnable();
					}
					else
					{
						MyLeftHandIK.SmoothDisable();
					}
				}

				MyHeadIK.solver.SmoothEnable();
				ActionState = HumanActionStates.None;
			}
		}

		if(command == HumanCharCommands.ThrowGrenade)
		{
			if(ActionState != HumanActionStates.None)
			{
				return;
			}

			if(UpperBodyState == HumanUpperBodyStates.Aim)
			{
				//MyAimIK.solver.SmoothDisable();
			}


			if(this.MyReference.CurrentWeapon != null && MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
			{
				MyLeftHandIK.SmoothEnable();
			}

			//MyHeadIK.solver.SmoothDisable(1);

			//move weapon to torso mount so that right hand is free
			if(this.MyReference.CurrentWeapon != null)
			{
				this.MyReference.CurrentWeapon.transform.parent = this.MyReference.TorsoWeaponMount.transform;
			}
			MyAnimator.SetTrigger("ThrowGrenade");

			_throwTarget = this.AimPoint;
			_throwDir = this.AimPoint - transform.position;
			IsBodyLocked = true;

			ActionState = HumanActionStates.Throw;

			_thrownObjectInHand = ((GameObject)GameObject.Instantiate(Resources.Load("TestGrenade"))).GetComponent<ThrownObject>();
			_thrownObjectInHand.GetComponent<Rigidbody>().isKinematic = true;

			_thrownObjectInHand.transform.parent = this.MyReference.RightHandWeaponMount.transform;
			_thrownObjectInHand.transform.localPosition = _thrownObjectInHand.InHandPosition;
			_thrownObjectInHand.transform.localEulerAngles = _thrownObjectInHand.InHandRotation;
		}


	}


	public WeaponAnimType GetCurrentAnimWeapon()
	{
		return (WeaponAnimType)MyAnimator.GetInteger("WeaponType");
	}






	public bool IsThrowing()
	{
		return _isThrowing;
	}

	public Vector3 GetLockedAimTarget()
	{
		return _throwTarget;
	}



	public void OnSuccessfulShot()
	{
		MyAnimator.SetTrigger("Shoot");
		StartCoroutine(WaitAndMuzzleClimb(0.05f));
		this.MyStatus.ArmFatigue += 1f;
		if(this.MyStatus.ArmFatigue > this.MyStatus.MaxArmFatigue)
		{
			this.MyStatus.ArmFatigue = this.MyStatus.MaxArmFatigue;
		}
	}

	public void OnThrowLeaveHand()
	{
		_thrownObjectInHand.transform.parent = null;

		Vector3 distance = this.AimPoint - transform.position;

		float magnitude = Mathf.Clamp(distance.magnitude, 10, 15);
		Vector3 direction = distance.normalized;

		if(MyAnimator.GetFloat("VSpeed") < 0.3f)
		{
			direction = _throwDir.normalized;
		}





		_thrownObjectInHand.transform.position = this.MyReference.RightHandWeaponMount.transform.position + direction * 1f;
		Vector3 throwForce = (direction * 2 + Vector3.up).normalized * (magnitude * 0.8f);

		_thrownObjectInHand.GetComponent<Rigidbody>().isKinematic = false;
		_thrownObjectInHand.GetComponent<Rigidbody>().AddForce(throwForce, ForceMode.Impulse);
		_thrownObjectInHand.GetComponent<Rigidbody>().AddTorque((transform.right + transform.up) * 6, ForceMode.Impulse);
	}

	public void OnThrowFinish()
	{
		if(UpperBodyState == HumanUpperBodyStates.Aim)
		{
			MyAimIK.solver.SmoothEnable();
			//MyAimIK.solver.transform = this.MyReference.CurrentWeapon.GetComponent<Weapon>().AimTransform;
		}
		else
		{
			MyAimIK.solver.SmoothDisable();
		}

		MyHeadIK.solver.SmoothEnable();

		//move weapon back to right hand mount
		if(this.MyReference.CurrentWeapon != null)
		{
			Weapon myWeapon = this.MyReference.CurrentWeapon.GetComponent<Weapon>();
			myWeapon.transform.parent = MyReference.RightHandWeaponMount.transform;
			myWeapon.transform.localPosition = myWeapon.InHandPosition;
			myWeapon.transform.localEulerAngles = myWeapon.InHandAngles;
		}

		IsBodyLocked = false;
		ActionState = HumanActionStates.None;

		SendCommand(HumanCharCommands.FinishThrow);
	}

	public void OnReloadFinish()
	{
		if(ActionState == HumanActionStates.Reload && this.MyReference.CurrentWeapon != null)
		{
			Debug.Log("finish reload");
			if(UpperBodyState == HumanUpperBodyStates.Aim)
			{
				MyAimIK.solver.SmoothEnable();
			}

			if(MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
			{
				MyLeftHandIK.SmoothEnable();
			}
			else
			{
				Debug.Log("done reloading pistol " + UpperBodyState);
				if(UpperBodyState == HumanUpperBodyStates.Aim)
				{
					MyLeftHandIK.SmoothEnable();
				}
				else
				{
					MyLeftHandIK.SmoothDisable();
				}
			}
			MyHeadIK.solver.SmoothEnable();

			ActionState = HumanActionStates.None;
		}
	}

	public void OnLongGunPullOutFinish()
	{
		ActionState = HumanActionStates.None;
		this.MyLeftHandIK.SmoothEnable();

		if(UpperBodyState == HumanUpperBodyStates.Aim)
		{
			SendCommand(HumanCharCommands.Aim);
		}
	}

	public void OnLongGunPutAwayFinish()
	{
		ActionState = HumanActionStates.None;
	}
		

	public void OnPistolPullOutFinish()
	{
		ActionState = HumanActionStates.None;
		if(UpperBodyState == HumanUpperBodyStates.Aim)
		{
			SendCommand(HumanCharCommands.Aim);
		}
		else
		{
			MyLeftHandIK.SmoothDisable(6);
		}
	}

	public void OnPistolPutAwayFinish()
	{
		ActionState = HumanActionStates.None;
	}

	public void OnBulletInjury()
	{
		this.MyAnimator.SetTrigger("Injure");
		if(UpperBodyState == HumanUpperBodyStates.Aim)
		{
			MyAimIK.solver.IKPositionWeight = 0;
			MyAimIK.solver.SmoothEnable(1);
		}

		if(MyLeftHandIK.IsEnabled() && MyAnimator.GetInteger("WeaponType") == (int)WeaponAnimType.Longgun)
		{
			MyLeftHandIK.InstantDisable();
			MyLeftHandIK.SmoothEnable(1);
		}

		MyHeadIK.solver.IKPositionWeight = 0;
		MyHeadIK.solver.SmoothEnable(1);
	}

	public void OnDeath()
	{
		float posture = UnityEngine.Random.Range(0, 200)/200f;

		this.MyAnimator.SetFloat("DeathPosture", posture);
		this.MyAnimator.SetTrigger("Death");

		CurrentAnimState = new HumanAnimStateDeath(this);
		IsBodyLocked = true;
		MyAimIK.solver.SmoothDisable(9);
		MyLeftHandIK.SmoothDisable(12);
		MyHeadIK.solver.SmoothDisable(9);


	}







	private void SwitchWeapon(string weaponName)
	{
		//first destroy the current weapon prefab
		GameObject.Destroy(this.MyReference.CurrentWeapon);

		this.MyStatus.ResetSpeedModifier();

		if(weaponName != "")
		{
			//load a new weapon and set position/rotation/parent
			GameObject obj = GameObject.Instantiate(Resources.Load(weaponName) as GameObject);
			Weapon newWeapon = obj.GetComponent<Weapon>();
			newWeapon.transform.parent = MyReference.RightHandWeaponMount.transform;
			newWeapon.transform.localPosition = newWeapon.InHandPosition;
			newWeapon.transform.localEulerAngles = newWeapon.InHandAngles;
			newWeapon.Rebuild(OnSuccessfulShot);

			this.MyAimIK.solver.transform = newWeapon.AimTransform;
			AimTransform = newWeapon.AimTransform;
			this.MyLeftHandIK.Target = newWeapon.ForeGrip;

			this.MyReference.CurrentWeapon = obj;

			float lessSpeed = newWeapon.GetTotalLessMoveSpeed();

			this.MyStatus.RunSpeedModifier = Mathf.Clamp(this.MyStatus.RunSpeedModifier - lessSpeed, 0.9f, 1.2f);
			this.MyStatus.SprintSpeedModifier = Mathf.Clamp(this.MyStatus.SprintSpeedModifier - lessSpeed, 0.9f, 1.1f);
			this.MyStatus.StrafeSpeedModifier = Mathf.Clamp(this.MyStatus.StrafeSpeedModifier - lessSpeed, 0.9f, 1.2f);

			this.MyAI.BlackBoard.EquippedWeapon = newWeapon;
		}
	}




	private void UpdateLayerWeights()
	{
		if(_layerDState)
		{
			this.MyAnimator.SetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D"), 
				Mathf.Lerp(this.MyAnimator.GetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D")), 1, Time.deltaTime * 1));
		}
		else
		{
			this.MyAnimator.SetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D"), 
				Mathf.Lerp(this.MyAnimator.GetLayerWeight(this.MyAnimator.GetLayerIndex("FullBodyOverride-D")), 0, Time.deltaTime * 1));
		}
	}

	private void UpdateLookDirection()
	{
		//get the direction of look on the xz plane
		Vector3 lookDir = LookTarget.position - transform.position;
		lookDir = new Vector3(lookDir.x, 0, lookDir.z);
		float lookBodyAngle = Vector3.Angle(lookDir, transform.right);

		//manipulate lookBodyAngle so it's not linear
		//float controlValue = lookBodyAngle * lookBodyAngle / 100;
		this.MyAnimator.SetFloat("LookBodyAngle", lookBodyAngle);



	}

	private void UpdateDestBodyAngle()
	{
		//get the direction of destination on the xz plane
		Vector3 destDir = this.Destination.Value - transform.position;
		destDir = new Vector3(destDir.x, 0, destDir.z);
		float destBodyAngle = Vector3.Angle(destDir, transform.right);
		
		//manipulate destBodyAngle so it's not linear
		float controlValue = destBodyAngle;
		this.MyAnimator.SetFloat("DestBodyAngle", controlValue);
		//Debug.Log(destBodyAngle.ToString() + " " + controlValue);
	}

	private void UpdateFatigue()
	{
		this.MyStatus.ArmFatigue -= Time.deltaTime * 3;
		if(this.MyStatus.ArmFatigue < 0)
		{
			this.MyStatus.ArmFatigue = 0;
		}
	}


	IEnumerator WaitAndEnableAimIK(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		MyAimIK.solver.SmoothEnable();
	}

	IEnumerator WaitAndMuzzleClimb(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		if(AimTarget.localPosition.y < 0.5f && this.MyReference.CurrentWeapon != null)
		{
			float climb = Mathf.Clamp(this.MyReference.CurrentWeapon.GetComponent<Gun>().GetRecoil() 
				* (this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue), 0, 1);
			AimTarget.localPosition += new Vector3(0, climb, 0);
		}

		float maxSpread = Mathf.Clamp(this.MyReference.CurrentWeapon.GetComponent<Gun>().GetRecoil() 
			* (this.MyStatus.ArmFatigue / this.MyStatus.MaxArmFatigue), 0, 0.5f) / 2;
		AimTarget.localPosition = new Vector3(0, AimTarget.localPosition.y, 2);
		AimTarget.localPosition += new Vector3(UnityEngine.Random.Range(-1 * maxSpread, maxSpread), 0, 0);

	}

	IEnumerator WaitAndCreateThrowObject(float waitTime, string objectName)
	{
		yield return new WaitForSeconds(waitTime);



	}
}
