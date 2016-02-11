using UnityEngine;
using System.Collections;

public class HumanAnimStateGoForward : HumanAnimStateBase
{
	private float _vSpeed;
	private bool _isWalkingBack;
	private bool _isStrafing;
	private bool _isFirstUpdateDone;
	private float _aimFreelookAngle;
	private float _noAimFreelookAngle;

	// This constructor will create new state taking values from old state
	public HumanAnimStateGoForward(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateGoForward(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}
	
	
	public override void SendCommand(HumanCharCommands command)
	{
		switch(command)
		{
		case HumanCharCommands.Crouch:
			UpdateState(HumanBodyStates.CrouchWalk);
			break;
		case HumanCharCommands.ThrowGrenade:
			break;
		case HumanCharCommands.FinishThrow:
			break;
		case HumanCharCommands.Idle:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		}
	}
	
	public override void Update()
	{
		float targetVSpeed = 0;
		if(this.ParentCharacter.CurrentStance == HumanStances.Run)
		{
			if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
			{
				targetVSpeed = 1f  * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
			}
			else
			{
				targetVSpeed = 1.5f * this.ParentCharacter.MyStatus.RunSpeedModifier;
			}

		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Walk)
		{
			targetVSpeed = 1;
		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint)
		{
			targetVSpeed = 2f * this.ParentCharacter.MyStatus.SprintSpeedModifier;
		}

		_vSpeed = Mathf.Lerp(_vSpeed, targetVSpeed, 5 * Time.deltaTime);
		//Debug.Log("VSpeed " + _vSpeed);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);

		HandleNavAgentMovement();


	}
	






	private void Initialize()
	{
		//Debug.Log("initializing walk forward " + "Dest " + this.ParentCharacter.Destination);
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", 0);
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", false);
		this.ParentCharacter.MyHeadIK.solver.bodyWeight = 0.0f;
		this.ParentCharacter.MyHeadIK.solver.headWeight = 0.75f;
		//this.ParentCharacter.MyHeadIK.solver.SmoothDisable();
		_vSpeed = 0;

	}

	private void HandleNavAgentMovement()
	{
		NavMeshAgent agent = this.ParentCharacter.GetComponent<NavMeshAgent>();

		//set the speed and acceleration
		if(this.ParentCharacter.CurrentStance == HumanStances.Run || this.ParentCharacter.CurrentStance == HumanStances.Walk || _isWalkingBack)
		{
			if(_isWalkingBack && !_isStrafing)
			{
				if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
				{
					//walking backward while aiming
					agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				}
				else
				{
					agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
				}

				agent.acceleration = 20;
			}
			else if(_isStrafing)
			{
				agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed * this.ParentCharacter.MyStatus.StrafeSpeedModifier;
				agent.acceleration = 20;
			}
			else
			{
				if(this.ParentCharacter.CurrentStance == HumanStances.Run)
				{
					if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
					{
						agent.speed = this.ParentCharacter.MyStatus.StrafeSpeed;
					}
					else
					{
						agent.speed = this.ParentCharacter.MyStatus.RunSpeed * this.ParentCharacter.MyStatus.RunSpeedModifier;
					}
					agent.acceleration = 20;
				}
				else
				{
					agent.speed = this.ParentCharacter.MyStatus.WalkSpeed;
					agent.acceleration = 6;
				}
			}


		}
		else if(this.ParentCharacter.CurrentStance == HumanStances.Sprint && !_isWalkingBack)
		{
			agent.speed = this.ParentCharacter.MyStatus.SprintSpeed * this.ParentCharacter.MyStatus.SprintSpeedModifier;
			agent.acceleration = 20;
		}

		agent.SetDestination(this.ParentCharacter.Destination.Value);

		if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Idle)
		{
			
			agent.updateRotation = false;

			_isStrafing = false;

			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			
			Vector3 destDir = this.ParentCharacter.MyNavAgent.velocity.normalized; 
			destDir = new Vector3(destDir.x, 0, destDir.z);
			float lookDestAngle = Vector3.Angle(lookDir, destDir);

			float destRightBodyAngle = Vector3.Angle(destDir, this.ParentCharacter.transform.right);

			this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", 0);
			this.ParentCharacter.MyAnimator.SetFloat("DestRightBodyAngle", destRightBodyAngle);
			/*
			if(lookDestAngle > 90)
			{
				_isWalkingBack = true;
				_isStrafing = false;
				
				Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}
			else */
			{
				_isWalkingBack = false;
				_isStrafing = false;
				
				Vector3 direction = destDir + lookDir.normalized * 0.05f;
				Quaternion rotation = Quaternion.LookRotation(direction);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
			}

		}
		else if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
		{
			agent.updateRotation = false;
			//check the destination and look angle
			Vector3 lookDir = this.ParentCharacter.AimTarget.position - this.ParentCharacter.transform.position;
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);

			Vector3 destDir = this.ParentCharacter.MyNavAgent.velocity.normalized; //this.ParentCharacter.Destination.Value - this.ParentCharacter.transform.position;
			destDir = new Vector3(destDir.x, 0, destDir.z);

			float lookDestAngle = Vector3.Angle(lookDir, destDir);
			float destRightBodyAngle = Vector3.Angle(destDir, this.ParentCharacter.transform.right);
			this.ParentCharacter.MyAnimator.SetFloat("LookDestAngle", lookDestAngle);
			this.ParentCharacter.MyAnimator.SetFloat("DestRightBodyAngle", destRightBodyAngle);
			//Debug.Log("look dest angle " + lookDestAngle);
			//if destination and look dir angle greater than 90 it means we are walking backwards. when
			//walking backwards disable agent update rotation and manually align rotation to opposite of destDir
			//when holding weapon and aiming, then it's 45 degrees so we will go into strafe mode
			WeaponAnimType weaponType = (WeaponAnimType)this.ParentCharacter.MyAnimator.GetInteger("WeaponType");

			if(weaponType == WeaponAnimType.Pistol || weaponType == WeaponAnimType.Longgun)
			{
				if(lookDestAngle > 45 && lookDestAngle <= 135 && this.ParentCharacter.CurrentStance != HumanStances.Sprint)
				{
					//strafe
					_isStrafing = true;
					_isWalkingBack = false;

					Vector3 direction = Vector3.zero;
					//check if body is turning left or right by checking the angle between lookdir and cross(up, destdir)
					Vector3 crossUpDestDir = Vector3.Cross(Vector3.up, destDir);
					float lookCrossDirAngle = Vector3.Angle(lookDir, crossUpDestDir);

					if(lookCrossDirAngle > 90)
					{
						direction = crossUpDestDir * -1;
					}
					else
					{
						direction = crossUpDestDir;
					}

					if(direction == Vector3.zero)
					{
						direction = this.ParentCharacter.transform.forward;
					}
					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
				else if(lookDestAngle > 135)
				{
					//walk back
					_isWalkingBack = true;
					_isStrafing = false;
					
					Vector3 direction = destDir * -1 + lookDir.normalized * 0.05f;
					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
				else
				{
					//walk forward
					_isWalkingBack = false;
					_isStrafing = false;
					
					Vector3 direction = destDir + lookDir.normalized * 0.05f;
					Quaternion rotation = Quaternion.LookRotation(direction);
					this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				}
			}

				


		}

	

		//go to idle state if very close to destination

		if(this.ParentCharacter.MyNavAgent.remainingDistance <= 0.1f && _isFirstUpdateDone)
		{
			UpdateState(HumanBodyStates.StandIdle);
		}

		if (!_isFirstUpdateDone) 
		{
			_isFirstUpdateDone = true;
		}

	}

	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		case HumanBodyStates.CrouchWalk:
			this.ParentCharacter.CurrentStance = HumanStances.Crouch;
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakForward(this);
			break;
		}
	}
	
	

}
