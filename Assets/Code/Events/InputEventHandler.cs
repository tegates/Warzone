using UnityEngine;
using System.Collections;

public class InputEventHandler
{

	#region Singleton 
	private static InputEventHandler _instance;
	public static InputEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new InputEventHandler();
			
			return _instance;
		}
	}
	#endregion
	
	#region Constructor
	public InputEventHandler()
	{
		Initialize();
	}
	
	#endregion

	#region Public Events
	public delegate void KeyEventDelegate();
	public delegate void MouseEventDelegate(float movement);
	
	
	public static event KeyEventDelegate OnPlayerMove;
	public static event KeyEventDelegate OnPlayerStopMove;

	public static event KeyEventDelegate OnWeaponPullTrigger;
	public static event KeyEventDelegate OnWeaponReleaseTrigger;
	public static event KeyEventDelegate OnWeaponAim;
	public static event KeyEventDelegate OnWeaponStopAim;
	public static event KeyEventDelegate OnKick;

	public static event KeyEventDelegate OnCameraSwitchMode;

	public static event MouseEventDelegate OnCameraRotateLeft;
	public static event MouseEventDelegate OnCameraRotateRight;
	public static event KeyEventDelegate OnCameraStopRotate;

	public static event KeyEventDelegate OnCameraPanLeft;
	public static event KeyEventDelegate OnCameraPanRight;
	public static event KeyEventDelegate OnCameraPanUp;
	public static event KeyEventDelegate OnCameraPanDown;

	public static event KeyEventDelegate OnCameraLookAhead;
	public static event KeyEventDelegate OnCameraStopLookAhead;

	public static event KeyEventDelegate OnPlayerMoveLeft;
	public static event KeyEventDelegate OnPlayerMoveRight;
	public static event KeyEventDelegate OnPlayerMoveUp;
	public static event KeyEventDelegate OnPlayerMoveDown;
	public static event KeyEventDelegate OnPlayerStopMoveLeft;
	public static event KeyEventDelegate OnPlayerStopMoveRight;
	public static event KeyEventDelegate OnPlayerStopMoveUp;
	public static event KeyEventDelegate OnPlayerStopMoveDown;

	public static event KeyEventDelegate OnPlayerToggleSneak;

	public static event KeyEventDelegate OnPlayerStartSprint;
	public static event KeyEventDelegate OnPlayerStopSprint;


	public static event KeyEventDelegate OnPlayerSwitchWeapon2;
	public static event KeyEventDelegate OnPlayerSwitchWeapon1;

	public static event KeyEventDelegate OnPlayerReload;

	public static event KeyEventDelegate OnPlayerThrow;

	#endregion


	#region Public Methods
	public void HandleKeyInputs()
	{
		#region Testing
		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			Debug.Log("injured");
			GameManager.Inst.PlayerControl.SelectedPC.MyEventHandler.TriggerOnBulletInjury();
		}

		if(Input.GetKeyDown(KeyCode.Backspace))
		{
			GoapUnitTest goapTest = new GoapUnitTest();
			goapTest.RunUnitTest();
		}

		#endregion


		#region Camera Controls

		float wheelInput = Input.GetAxis("Mouse ScrollWheel");
		if(wheelInput > 0)
		{
			if(OnCameraRotateLeft != null)
			{
				OnCameraRotateLeft(wheelInput);
			}
		}
		else if(wheelInput < 0)
		{
			if(OnCameraRotateRight != null)
			{
				OnCameraRotateRight(wheelInput);
			}
		}


		if(Input.GetKeyDown(KeyCode.Q))
		{
			if(OnCameraRotateLeft != null)
			{
				//OnCameraRotateLeft();
			}
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			if(OnCameraRotateRight != null)
			{
				//OnCameraRotateRight();
			}
		}

		if((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q)) &&
		   !(Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q)))
		{
			if(OnCameraStopRotate != null)
			{
				OnCameraStopRotate();
			}
		}




		if(Input.GetKey(KeyCode.A))
		{
			if(OnCameraPanLeft != null)
			{
				OnCameraPanLeft();
			}
		}

		if(Input.GetKey(KeyCode.D))
		{
			if(OnCameraPanRight != null)
			{
				OnCameraPanRight();
			}
		}

		if(Input.GetKey(KeyCode.W))
		{
			if(OnCameraPanUp != null)
			{
				OnCameraPanUp();
			}
		}

		if(Input.GetKey(KeyCode.S))
		{
			if(OnCameraPanDown != null)
			{
				OnCameraPanDown();
			}
		}


		if(Input.GetKeyDown(KeyCode.LeftAlt))
		{
			if(OnCameraLookAhead != null)
			{
				OnCameraLookAhead();
			}
		}

		if(Input.GetKeyUp(KeyCode.LeftAlt))
		{
			if(OnCameraStopLookAhead != null)
			{
				OnCameraStopLookAhead();
			}
		}

		#endregion

		#region Character Control

		if(Input.GetKeyDown(KeyCode.A))
		{
			if(OnPlayerMoveLeft != null)
			{
				OnPlayerMoveLeft();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.D))
		{
			if(OnPlayerMoveRight != null)
			{
				OnPlayerMoveRight();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.W))
		{
			if(OnPlayerMoveUp != null)
			{
				OnPlayerMoveUp();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.S))
		{
			if(OnPlayerMoveDown != null)
			{
				OnPlayerMoveDown();
			}
		}


		//


		if(Input.GetKeyUp(KeyCode.A))
		{
			if(OnPlayerStopMoveLeft != null)
			{
				OnPlayerStopMoveLeft();
			}
		}
		
		if(Input.GetKeyUp(KeyCode.D))
		{
			if(OnPlayerStopMoveRight != null)
			{
				OnPlayerStopMoveRight();
			}
		}
		
		if(Input.GetKeyUp(KeyCode.W))
		{
			if(OnPlayerStopMoveUp != null)
			{
				OnPlayerStopMoveUp();
			}
		}
		
		if(Input.GetKeyUp(KeyCode.S))
		{
			if(OnPlayerStopMoveDown != null)
			{
				OnPlayerStopMoveDown();
			}
		}

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			if(OnPlayerStartSprint != null)
			{
				OnPlayerStartSprint();
			}
		}

		if(Input.GetKeyUp(KeyCode.LeftShift))
		{
			if(OnPlayerStopSprint != null)
			{
				OnPlayerStopSprint();
			}
		}


		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			if(OnPlayerMove != null)
			{
				OnPlayerMove();
			}

			if(OnWeaponPullTrigger != null)
			{
				OnWeaponPullTrigger();
			}
		}
		
		if(Input.GetKeyUp(KeyCode.Mouse0))
		{
			if(OnPlayerStopMove != null)
			{
				OnPlayerStopMove();
			}

			if(OnWeaponReleaseTrigger != null)
			{
				OnWeaponReleaseTrigger();
			}
		}
			

		if(Input.GetKeyDown(KeyCode.F))
		{
			if(OnKick != null)
			{
				OnKick();
			}
		}

		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			if(OnWeaponAim != null)
			{
				OnWeaponAim();
			}
		}

		if(Input.GetKeyUp(KeyCode.Mouse1))
		{
			if(OnWeaponStopAim != null)
			{
				OnWeaponStopAim();
			}
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			if(OnPlayerToggleSneak != null)
			{
				OnPlayerToggleSneak();
			}
		}


		#endregion

		#region Use Prop Events

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(OnPlayerSwitchWeapon2 != null)
			{
				OnPlayerSwitchWeapon2();
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(OnPlayerSwitchWeapon1 != null)
			{
				OnPlayerSwitchWeapon1();
			}
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			if(OnPlayerReload != null)
			{
				OnPlayerReload();
			}
		}

		if(Input.GetKeyDown(KeyCode.G))
		{
			if(OnPlayerThrow != null)
			{
				OnPlayerThrow();
			}
		}

		#endregion
	}

	#endregion

	#region Private Methods

	private void Initialize()
	{

	}

	#endregion

}
