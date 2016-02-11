﻿using UnityEngine;
using System.Collections;

public class HumanAnimStateSneakIdle : HumanAnimStateBase
{

	private float _vSpeed;
	private bool _isRotatingBody;
	private float _aimFreelookAngle;
	private float _noAimFreelookAngle;
	
	// This constructor will create new state taking values from old state
	public HumanAnimStateSneakIdle(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateSneakIdle(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}
	
	
	public override void SendCommand (HumanCharCommands command)
	{
		switch(command)
		{
		case HumanCharCommands.GoToPosition:
			UpdateState(HumanBodyStates.CrouchWalk);
			break;
		case HumanCharCommands.StopCrouch:
			UpdateState(HumanBodyStates.StandIdle);
			break;
		case HumanCharCommands.ThrowGrenade:
			_aimFreelookAngle = 0;
			_noAimFreelookAngle = 0;
			break;
		case HumanCharCommands.FinishThrow:
			_aimFreelookAngle = 45;
			_noAimFreelookAngle = 60;
			break;
		}
	}
	
	public override void Update ()
	{
		
		if(_vSpeed > 0.3f)
		{
			_vSpeed -= 8 * Time.deltaTime;
		}
		else
		{
			_vSpeed = 0;
		}
		
		this.ParentCharacter.MyAnimator.SetFloat("VSpeed", _vSpeed);
		
		//update body rotation
		

		
		if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Aim)
		{
			Vector3 lookDir = this.ParentCharacter.AimTarget.position - this.ParentCharacter.transform.position;
			if(this.ParentCharacter.IsBodyLocked)
			{
				lookDir = this.ParentCharacter.GetLockedAimTarget() - this.ParentCharacter.transform.position;
			}
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			float lookBodyAngle = Vector3.Angle(lookDir, this.ParentCharacter.transform.forward);
			
			if(_isRotatingBody)
			{
				
				Quaternion rotation = Quaternion.LookRotation(lookDir);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				if(lookBodyAngle < 5)
				{
					_isRotatingBody = false;
				}
			}
			else
			{
				if(lookBodyAngle > _aimFreelookAngle)
				{
					_isRotatingBody = true;
				}
			}
		}
		else if(this.ParentCharacter.UpperBodyState == HumanUpperBodyStates.Idle)
		{
			Vector3 lookDir = this.ParentCharacter.LookTarget.position - this.ParentCharacter.transform.position;
			if(this.ParentCharacter.IsBodyLocked)
			{
				lookDir = this.ParentCharacter.GetLockedAimTarget() - this.ParentCharacter.transform.position;
			}
			lookDir = new Vector3(lookDir.x, 0, lookDir.z);
			float lookBodyAngle = Vector3.Angle(lookDir, this.ParentCharacter.transform.forward);


			if(_isRotatingBody)
			{
				
				Quaternion rotation = Quaternion.LookRotation(lookDir);
				this.ParentCharacter.transform.rotation = Quaternion.Lerp(this.ParentCharacter.transform.rotation, rotation, Time.deltaTime * 5);
				if(lookBodyAngle < 5)
				{
					_isRotatingBody = false;
				}
			}
			else
			{
				if(lookBodyAngle > _noAimFreelookAngle)
				{
					_isRotatingBody = true;
				}
			}
		}


	}
	
	
	
	
	private void Initialize()
	{
		Debug.Log("Initializing sneak idle");
		this.ParentCharacter.CurrentStance = HumanStances.Crouch;
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", true);
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		this.ParentCharacter.MyNavAgent.Stop();
		this.ParentCharacter.MyNavAgent.ResetPath();
		this.ParentCharacter.MyHeadIK.solver.bodyWeight = 0.2f;
		_aimFreelookAngle = 45;
		_noAimFreelookAngle = 60;
	}
	
	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.CrouchWalk:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakForward(this);
			break;
		case HumanBodyStates.StandIdle:
			this.ParentCharacter.CurrentStance = HumanStances.Run;
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateIdle(this);
			break;
		}
	}
}
