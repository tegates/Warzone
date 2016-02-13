using UnityEngine;
using System.Collections;

public class PlayerControl
{

	#region Public Fields
	public HumanCharacter SelectedPC;
	
	#endregion
	
	#region Private Fields
	private bool _isMoveKeyDown;
	private PlayerMoveDirEnum _moveDirection;
	private PlayerMoveDirEnum _moveDirection2;
	private int _numberOfRocks;
	private GameObject _aimedObject;
	#endregion
	
	
	#region Public Methods

	public void Initialize()
	{
		SelectedPC = GameObject.Find("HumanCharacter").GetComponent<HumanCharacter>();
		SelectedPC.Initialize();

		_numberOfRocks = 10;

		InputEventHandler.OnCameraSwitchMode += SwitchCameraMode;

		InputEventHandler.OnPlayerMove += this.OnPlayerMove;
		InputEventHandler.OnPlayerStopMove += this.OnPlayerStopMove;
		InputEventHandler.OnWeaponPullTrigger += this.OnWeaponPullTrigger;
		InputEventHandler.OnWeaponReleaseTrigger += this.OnWeaponReleaseTrigger;
		InputEventHandler.OnWeaponAim += this.OnWeaponAim;
		InputEventHandler.OnWeaponStopAim += this.OnWeaponStopAim;
		InputEventHandler.OnKick += this.OnKick;

		InputEventHandler.OnPlayerMoveLeft += this.OnPlayerMoveLeft;
		InputEventHandler.OnPlayerMoveRight += this.OnPlayerMoveRight;
		InputEventHandler.OnPlayerMoveUp += this.OnPlayerMoveUp;
		InputEventHandler.OnPlayerMoveDown += this.OnPlayerMoveDown;
		InputEventHandler.OnPlayerStopMoveLeft += this.OnPlayerStopMoveLeft;
		InputEventHandler.OnPlayerStopMoveRight += this.OnPlayerStopMoveRight;
		InputEventHandler.OnPlayerStopMoveUp += this.OnPlayerStopMoveUp;
		InputEventHandler.OnPlayerStopMoveDown += this.OnPlayerStopMoveDown;

		InputEventHandler.OnPlayerStartSprint += this.OnPlayerStartSprint;
		InputEventHandler.OnPlayerStopSprint += this.OnPlayerStopSprint;

		InputEventHandler.OnPlayerToggleSneak += OnToggleSneaking;
		InputEventHandler.OnPlayerThrow += OnThrow;

		InputEventHandler.OnPlayerSwitchWeapon2 += OnToggleWeapon2;
		InputEventHandler.OnPlayerSwitchWeapon1 += OnToggleWeapon1;

		InputEventHandler.OnPlayerReload += OnWeaponReload;

		_moveDirection = PlayerMoveDirEnum.Stop;
		_moveDirection2 = PlayerMoveDirEnum.Stop;

		SelectedPC.CurrentStance = HumanStances.Run;
	}
	


	public void PerFrameUpdate()
	{
		HumanCharacter c = SelectedPC.GetComponent<HumanCharacter>();


		//calculate aimpoint
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit))
		{
			_aimedObject = hit.collider.gameObject;
			c.AimPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);

		}




		if((_moveDirection == PlayerMoveDirEnum.Left && _moveDirection2 == PlayerMoveDirEnum.Stop) ||
		   (_moveDirection == PlayerMoveDirEnum.Stop && _moveDirection2 == PlayerMoveDirEnum.Left))
		{
			SelectedPC.Destination = SelectedPC.transform.position + Camera.main.transform.right * -1 * 1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Left && _moveDirection2 == PlayerMoveDirEnum.Up) ||
		        (_moveDirection == PlayerMoveDirEnum.Up && _moveDirection2 == PlayerMoveDirEnum.Left))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + (Camera.main.transform.right * -1 + cameraForwardFlat.normalized).normalized * 1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Left && _moveDirection2 == PlayerMoveDirEnum.Down) ||
		        (_moveDirection == PlayerMoveDirEnum.Down && _moveDirection2 == PlayerMoveDirEnum.Left))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + (Camera.main.transform.right * -1 + cameraForwardFlat.normalized * -1).normalized * 1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Right && _moveDirection2 == PlayerMoveDirEnum.Stop) ||
		        (_moveDirection == PlayerMoveDirEnum.Stop && _moveDirection2 == PlayerMoveDirEnum.Right))
		{
			SelectedPC.Destination = SelectedPC.transform.position + Camera.main.transform.right * 1f;
			//SelectedPC.Destination = SelectedPC.transform.position + SelectedPC.transform.right * 0.1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Right && _moveDirection2 == PlayerMoveDirEnum.Up) ||
		        (_moveDirection == PlayerMoveDirEnum.Up && _moveDirection2 == PlayerMoveDirEnum.Right))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + (Camera.main.transform.right + cameraForwardFlat.normalized).normalized * 1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Right && _moveDirection2 == PlayerMoveDirEnum.Down) ||
		        (_moveDirection == PlayerMoveDirEnum.Down && _moveDirection2 == PlayerMoveDirEnum.Right))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + (Camera.main.transform.right + cameraForwardFlat.normalized * -1).normalized * 1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Up && _moveDirection2 == PlayerMoveDirEnum.Stop) ||
		        (_moveDirection == PlayerMoveDirEnum.Stop && _moveDirection2 == PlayerMoveDirEnum.Up))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + cameraForwardFlat * 2f;
			//SelectedPC.Destination = SelectedPC.transform.position + SelectedPC.transform.forward * 0.1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}
		else if((_moveDirection == PlayerMoveDirEnum.Down && _moveDirection2 == PlayerMoveDirEnum.Stop) ||
		        (_moveDirection == PlayerMoveDirEnum.Stop && _moveDirection2 == PlayerMoveDirEnum.Down))
		{
			Vector3 cameraForwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			SelectedPC.Destination = SelectedPC.transform.position + cameraForwardFlat * -1 * 2f;
			//SelectedPC.Destination = SelectedPC.transform.position + SelectedPC.transform.forward * -1 * 0.1f;
			SelectedPC.SendCommand(HumanCharCommands.GoToPosition);
		}


		if(_moveDirection == PlayerMoveDirEnum.Stop)
		{
			if(_isMoveKeyDown)
			{
				//keep updating move direction as the vector from character to mouse cursor's projection point with magnitude of 1
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
																			Input.mousePosition.y, 
																			Vector3.Distance(c.transform.position, Camera.main.transform.position)));

				c.Destination = c.transform.position + (mouseWorldPos - c.transform.position).normalized;

				/*
				RaycastHit hitMove;
				Ray rayMove = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit))
				{
					c.Destination = hit.point;// + new Vector3(0, 1, 0);
					//Debug.Log(c.Destination);
				}
				*/
				
			}

			/*
			Vector3 displacement = c.Destination.Value - c.transform.position;
			if(displacement.magnitude < 0.1f && !_isMoveKeyDown)
			{
				c.SendCommand(HumanCharCommands.Idle);
				//c.GetComponent<CharacterController>().Move(Vector3.zero);
			}
			*/
		}



		float aimHeight = 0f;

		if(_aimedObject != null && _aimedObject.tag == "GroundOrFloor")
		{
			if(c.CurrentStance == HumanStances.Crouch || c.CurrentStance == HumanStances.CrouchRun)
			{
				aimHeight = 1f;
			}
			else
			{
				aimHeight = 1.5f;
			}
		}
		else
		{
			aimHeight = 0;
		}



		if(Mathf.Abs(c.AimPoint.y - c.transform.position.y) > 0.5f)
		{
			aimHeight = 0;
		}

		if(c.UpperBodyState == HumanUpperBodyStates.Aim || c.UpperBodyState == HumanUpperBodyStates.Idle)
		{
			if(c.MyReference.CurrentWeapon != null)
			{
				//c.AimTarget.position = Vector3.Lerp(c.AimTarget.position, c.AimPoint + new Vector3(0, aimHeight, 0), 5 * Time.deltaTime);
				c.AimTargetRoot.position = c.MyReference.CurrentWeapon.transform.position;
				Vector3 cameraDir = (GameManager.Inst.CameraController.transform.position - c.AimPoint).normalized;
				Vector3 aimPoint = c.AimPoint + cameraDir * aimHeight;
				/*
				if(_aimedObject != null && _aimedObject.tag == "NPC")
				{
					aimPoint = c.MyAI.TargetingSystem.GetAimPointOnTarget(_aimedObject.GetComponent<Character>());
				}
				*/
				c.MyAI.BlackBoard.AimPoint = aimPoint;
				Vector3 aimDir = aimPoint - c.MyReference.CurrentWeapon.transform.position;

				Quaternion rotation = Quaternion.LookRotation(aimDir);
				c.AimTargetRoot.transform.rotation = Quaternion.Lerp(c.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 6);

			}
			else
			{
				c.AimTargetRoot.position = c.transform.position + Vector3.up * 1.5f;
				Vector3 cameraDir = (GameManager.Inst.CameraController.transform.position - c.AimPoint).normalized;
				Vector3 aimPoint = c.AimPoint + cameraDir;
				Vector3 aimDir = aimPoint - c.transform.position;
				Quaternion rotation = Quaternion.LookRotation(aimDir);
				c.AimTargetRoot.transform.rotation = Quaternion.Lerp(c.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 6);
			}

			Vector3 lookPosition = new Vector3(c.AimTarget.position.x, c.transform.position.y + 1.5f, c.AimTarget.position.z);
			c.LookTarget.position = Vector3.Lerp(c.LookTarget.position, lookPosition, 8 * Time.deltaTime);



		}



	}




	public void SwitchCameraMode()
	{
		CameraModeEnum cameraMode = GameManager.Inst.CameraController.GetCameraMode();

		if(cameraMode == CameraModeEnum.Leader)
		{
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Party);
		}
		else
		{
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Leader);
		}


	}

	public void OnPlayerMove()
	{
		if(SelectedPC.UpperBodyState == HumanUpperBodyStates.Aim)
		{
			return;
		}

		SelectedPC.GetComponent<HumanCharacter>().SendCommand(HumanCharCommands.GoToPosition);
		_isMoveKeyDown = true;
	}

	public void OnPlayerStopMove()
	{
		if(SelectedPC.UpperBodyState == HumanUpperBodyStates.Aim)
		{
			return;
		}

		HumanCharacter c = SelectedPC.GetComponent<HumanCharacter>();

		//get world position from mouse cursor
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit))
		{
			c.Destination = hit.point;
		}


		_isMoveKeyDown = false;
	}

	public void OnPlayerMoveLeft()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{
			if(_moveDirection == PlayerMoveDirEnum.Stop)
			{
				_moveDirection = PlayerMoveDirEnum.Left;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Stop)
			{
				_moveDirection2 = PlayerMoveDirEnum.Left;
			}
		}
	}

	public void OnPlayerMoveRight()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{

			if(_moveDirection == PlayerMoveDirEnum.Stop)
			{
				_moveDirection = PlayerMoveDirEnum.Right;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Stop)
			{
				_moveDirection2 = PlayerMoveDirEnum.Right;
			}
		}
	}

	public void OnPlayerMoveUp()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{

			if(_moveDirection == PlayerMoveDirEnum.Stop)
			{
				_moveDirection = PlayerMoveDirEnum.Up;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Stop)
			{
				_moveDirection2 = PlayerMoveDirEnum.Up;
			}
		}
	}

	public void OnPlayerMoveDown()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{

			if(_moveDirection == PlayerMoveDirEnum.Stop)
			{
				_moveDirection = PlayerMoveDirEnum.Down;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Stop)
			{
				_moveDirection2 = PlayerMoveDirEnum.Down;
			}
		}
	}

	public void OnPlayerStopMoveLeft()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{
			if(_moveDirection == PlayerMoveDirEnum.Left)
			{
				_moveDirection = PlayerMoveDirEnum.Stop;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Left)
			{
				_moveDirection2 = PlayerMoveDirEnum.Stop;
			}
		}
	}
	
	public void OnPlayerStopMoveRight()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{
			if(_moveDirection == PlayerMoveDirEnum.Right)
			{
				_moveDirection = PlayerMoveDirEnum.Stop;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Right)
			{
				_moveDirection2 = PlayerMoveDirEnum.Stop;
			}

		}
	}
	
	public void OnPlayerStopMoveUp()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{
			if(_moveDirection == PlayerMoveDirEnum.Up)
			{
				_moveDirection = PlayerMoveDirEnum.Stop;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Up)
			{
				_moveDirection2 = PlayerMoveDirEnum.Stop;
			}

		}
	}
	
	public void OnPlayerStopMoveDown()
	{
		if(GameManager.Inst.CameraController.GetCameraMode() == CameraModeEnum.Leader)
		{
			if(_moveDirection == PlayerMoveDirEnum.Down)
			{
				_moveDirection = PlayerMoveDirEnum.Stop;
			}
			else if(_moveDirection2 == PlayerMoveDirEnum.Down)
			{
				_moveDirection2 = PlayerMoveDirEnum.Stop;
			}
			
		}
	}

	public void OnPlayerStartSprint()
	{
		SelectedPC.SendCommand(HumanCharCommands.Sprint);


	}

	public void OnPlayerStopSprint()
	{
		SelectedPC.SendCommand(HumanCharCommands.StopSprint);


	}




	public void OnWeaponAim()
	{
		SelectedPC.GetComponent<HumanCharacter>().SendCommand(HumanCharCommands.Aim);

	}

	public void OnWeaponStopAim()
	{
		SelectedPC.GetComponent<HumanCharacter>().SendCommand(HumanCharCommands.StopAim);
	}
	

	public void OnWeaponPullTrigger()
	{
		if(SelectedPC.UpperBodyState != HumanUpperBodyStates.Aim)
		{
			return;
		}
		SelectedPC.MyAI.WeaponSystem.StartFiringRangedWeapon();
		//SelectedPC.GetComponent<HumanCharacter>().SendCommand(HumanCharCommands.PullTrigger);
	}

	public void OnWeaponReleaseTrigger()
	{
		SelectedPC.MyAI.WeaponSystem.StopFiringRangedWeapon();
		//HumanCharacter character = SelectedPC.GetComponent<HumanCharacter>();
		//character.SendCommand(HumanCharCommands.ReleaseTrigger);

	}

	public void OnWeaponReload()
	{
		HumanCharacter character = SelectedPC.GetComponent<HumanCharacter>();
		if(character.ActionState != HumanActionStates.Reload)
		{
			character.SendCommand(HumanCharCommands.Reload);
		}
		else
		{
			character.SendCommand(HumanCharCommands.CancelReload);
		}
	}


	public void OnKick()
	{
		SelectedPC.GetComponent<HumanCharacter>().SendCommand(HumanCharCommands.Knock);
	}

	public void OnToggleSneaking()
	{
		if(SelectedPC.CurrentStance == HumanStances.Crouch || SelectedPC.CurrentStance == HumanStances.CrouchRun)
		{
			SelectedPC.SendCommand(HumanCharCommands.StopCrouch);
		}
		else
		{
			SelectedPC.SendCommand(HumanCharCommands.Crouch);
		}
	}

	public void OnThrow()
	{
		SelectedPC.SendCommand(HumanCharCommands.ThrowGrenade);
	}

	/*
	public void OnThrowRock()
	{
		if(_numberOfRocks <= 0)
		{
			return;
		}

		HumanCharacter c = SelectedPC.GetComponent<HumanCharacter>();

		//calculate aimpoint
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit))
		{
			float distance = Vector3.Distance(hit.point, c.transform.position);
			distance = Mathf.Clamp(distance, 0, 15);
			SelectedPC.SendCommand(HumanCharCommands.Throw);
		}

		_numberOfRocks --;
	}
	*/

	public void OnToggleWeapon2()
	{
		WeaponAnimType weaponType = SelectedPC.GetCurrentAnimWeapon();
		if(weaponType != WeaponAnimType.Longgun)
		{
			SelectedPC.SendCommand(HumanCharCommands.SwitchWeapon2);
		}
		else
		{
			SelectedPC.SendCommand(HumanCharCommands.Unarm);
		}
	}

	public void OnToggleWeapon1()
	{
		WeaponAnimType weaponType = SelectedPC.GetCurrentAnimWeapon();
		if(weaponType != WeaponAnimType.Pistol)
		{
			SelectedPC.SendCommand(HumanCharCommands.SwitchWeapon1);
		}
		else
		{
			SelectedPC.SendCommand(HumanCharCommands.Unarm);
		}
	}
	#endregion
	
	#region Private Methods
	
	
	
	#endregion
}
