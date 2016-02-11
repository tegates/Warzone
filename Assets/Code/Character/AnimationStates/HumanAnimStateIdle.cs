using UnityEngine;
using System.Collections;

public class HumanAnimStateIdle : HumanAnimStateBase
{

	private float _vSpeed;
	private bool _isRotatingBody;
	private float _aimFreelookAngle;
	private float _noAimFreelookAngle;


	// This constructor will create new state taking values from old state
	public HumanAnimStateIdle(HumanAnimStateBase state)     
		:this(state.ParentCharacter)
	{
		
	}
	
	// this constructor will be used by the other one
	public HumanAnimStateIdle(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;
		
		Initialize();
	}

	
	public override void SendCommand (HumanCharCommands command)
	{
		switch(command)
		{
		case HumanCharCommands.GoToPosition:
			UpdateState(HumanBodyStates.WalkForward);
			break;
		case HumanCharCommands.Crouch:
			UpdateState(HumanBodyStates.CrouchIdle);
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
		//Debug.Log("Initializing Stand Idle");
		_vSpeed = this.ParentCharacter.MyAnimator.GetFloat("VSpeed");
		this.ParentCharacter.MyAnimator.SetBool("IsSneaking", false);
		this.ParentCharacter.Destination = this.ParentCharacter.transform.position;
		this.ParentCharacter.MyNavAgent.Stop();
		this.ParentCharacter.MyNavAgent.ResetPath();
		this.ParentCharacter.MyNavAgent.updateRotation = false;
		this.ParentCharacter.MyHeadIK.solver.bodyWeight = 0.5f;

		_aimFreelookAngle = 45;
		_noAimFreelookAngle = 60;
	}

	private void UpdateState(HumanBodyStates state)
	{
		switch(state)
		{
		case HumanBodyStates.WalkForward:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateGoForward(this);
			break;
		case HumanBodyStates.CrouchIdle:
			this.ParentCharacter.CurrentAnimState = new HumanAnimStateSneakIdle(this);
			break;
		}
	}
}
