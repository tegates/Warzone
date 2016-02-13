using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{

	#region Public Fields
	public float RotateSpeed;
	public float PanSpeed;

	#endregion

	#region Private Fields
	private CameraModeEnum _cameraMode;

	private bool _isRotatingLeft;
	private bool _isRotatingRight;
	private bool _isPanningLeft;
	private bool _isPanningRight;
	private bool _isPanningUp;
	private bool _isPanningDown;

	private bool _isLookingAhead;

	private float _delayTimer;

	private int _currentRotation; //1-8

	private float _rotation;
	#endregion

	void Update()
	{
		if(_rotation < 0)
		{
			transform.RotateAround(GameManager.Inst.PlayerControl.SelectedPC.transform.position, Vector3.up, _rotation);

		}
		else if(_rotation > 0)
		{
			transform.RotateAround(GameManager.Inst.PlayerControl.SelectedPC.transform.position, Vector3.up, _rotation);

		}
		_rotation = Mathf.Lerp(_rotation, 0, 5 * Time.deltaTime);

		if(_cameraMode == CameraModeEnum.Leader)
		{
			HumanCharacter pc = GameManager.Inst.PlayerControl.SelectedPC;
			Vector3 cameraFacing = Camera.main.transform.forward;

			Vector3 cameraPos = pc.transform.position - cameraFacing * 27;


			/*
			Vector3 targetEuler = new Vector3(0, _currentRotation * 45, 0);
			Quaternion rotation = Quaternion.Euler(targetEuler);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
			*/

			Vector3 mousePos = Input.mousePosition;
			mousePos.x -= Screen.width/2;
			mousePos.y -= Screen.height/2;


			Vector3 aimDir = pc.AimPoint - pc.transform.position;
			Vector3 cameraPanDir = aimDir.normalized;
			cameraPanDir = new Vector3(cameraPanDir.x, 0, cameraPanDir.z);

			//panning distance is 0 when aimDir magnitude is less than 2
			//when greater than 2, slowly increase the distance up to say 7
			float maxPanDist = 6;
			float panDist = 0;

			float maxMousePos = Screen.height * 0.5f;

			panDist = Mathf.Clamp((mousePos.magnitude) / (maxMousePos) * (maxPanDist), 0, maxPanDist);



			Vector3 lookAheadPos = cameraPos + cameraPanDir * panDist;

			if(pc.UpperBodyState != HumanUpperBodyStates.Aim)
			{
				transform.position = Vector3.Lerp(transform.position, cameraPos, 8 * Time.deltaTime);
			}
			else
			{

				transform.position = Vector3.Lerp(transform.position, lookAheadPos, 4 * Time.deltaTime);
			}



			/*
			float angleDiff = Quaternion.Angle(rotation, transform.rotation);
			if(angleDiff > 10)
			{
				Vector3 targetPosition = cameraPos + pc.GetComponent<CharacterController>().velocity.normalized * 0;
				float moveRate = 3;
				
				transform.position = Vector3.Lerp(transform.position, targetPosition, moveRate * Time.deltaTime);
			}
			else
			{

				if(pc.GetComponent<CharacterController>().velocity.magnitude >= 1)
				{
					if(_delayTimer >= 0.5f)
					{
						if(!pc.GetComponent<Character>().IsWeaponAiming)
						{
							//if player is walking, then set camera at 3 meters ahead moving dir with fast lerp
							Vector3 targetPosition = cameraPos + pc.GetComponent<CharacterController>().velocity.normalized * 0;

							float moveRate = 1;
							if(_delayTimer < 1)
							{
								moveRate = _delayTimer;
							}

							transform.position = Vector3.Lerp(transform.position, targetPosition, moveRate * Time.deltaTime);
						}
						else
						{
							//if player is walking and aiming, then set camera 3 meters ahead facing dir with fast lerp
							Vector3 targetPosition = cameraPos + pc.transform.forward * 0;
							float moveRate = 1;
							if(_delayTimer < 1)
							{
								moveRate = _delayTimer;
							}
							transform.position = Vector3.Lerp(transform.position, targetPosition, moveRate * Time.deltaTime);
						}

						_delayTimer += Time.deltaTime;
					}
					else
					{
						_delayTimer += Time.deltaTime;
					}
				}
				else
				{
					//if player is not walking, then set camera at 10 meters ahead facing dir with slow lerp
					Vector3 targetPosition = cameraPos + pc.transform.forward * 10;
					float distance = Mathf.Clamp(Vector3.Distance(targetPosition, transform.position), 0, 15);
					float moveRate = 0;
					if(Vector3.Distance(targetPosition, transform.position) > 3)
					{
						moveRate = Mathf.Clamp((1 - distance / 15), 0.4f, 2f);
					}
					else
					{
						moveRate = 0;
					}
					transform.position = Vector3.Lerp(transform.position, targetPosition, moveRate * Time.deltaTime);

					_delayTimer = 0;
				}


			}

			*/



		}

		if(_cameraMode == CameraModeEnum.Party)
		{
			Transform pc = GameManager.Inst.PlayerControl.SelectedPC.transform;
			transform.position = new Vector3(transform.position.x, 50, transform.position.z);


			if(_isRotatingLeft)
			{
				transform.RotateAround(pc.position, Vector3.up, RotateSpeed * Time.unscaledDeltaTime);
			}

			if(_isRotatingRight)
			{
				transform.RotateAround(pc.position, Vector3.up, -1 * RotateSpeed * Time.unscaledDeltaTime);
			}

		}
	}


	#region Public Methods
	public void Initialize()
	{
		_cameraMode = CameraModeEnum.Leader;
		_currentRotation = 1;


		InputEventHandler.OnCameraRotateLeft += RotateLeft;
		InputEventHandler.OnCameraRotateRight += RotateRight;


		InputEventHandler.OnCameraPanLeft += StartPanLeft;
		InputEventHandler.OnCameraPanRight += StartPanRight;
		InputEventHandler.OnCameraPanUp += StartPanUp;
		InputEventHandler.OnCameraPanDown += StartPanDown;

		InputEventHandler.OnCameraLookAhead += StartLookAhead;
		InputEventHandler.OnCameraStopLookAhead += StopLookAhead;

		Transform pc = GameManager.Inst.PlayerControl.SelectedPC.transform;
		Vector3 cameraFacing = Camera.main.transform.forward;
		
		Vector3 cameraPos = pc.position - cameraFacing * 10;
		Vector3 targetPosition = cameraPos + pc.transform.forward * 10;
		transform.position = targetPosition;
	}

	public void SetCameraMode(CameraModeEnum mode)
	{
		_cameraMode = mode;
	}



	public CameraModeEnum GetCameraMode()
	{
		return _cameraMode;
	}

	public void RotateLeft(float amount)
	{
		_rotation += amount * 15 * 1f;

	}

	public void RotateRight(float amount)
	{
		_rotation += amount * 15 * 1f;
	}

	public void StartRotateLeft()
	{
		_isRotatingLeft = true;
		_isRotatingRight = false;


		_currentRotation --;
		if(_currentRotation < 1)
		{
			_currentRotation = 8;
		}
	}

	public void StartRotateRight()
	{
		_isRotatingRight = true;
		_isRotatingLeft = false;


		_currentRotation ++;
		if(_currentRotation > 8)
		{
			_currentRotation = 1;
		}

	}
	
	public void StopRotating()
	{
		_isRotatingLeft = false;
		_isRotatingRight = false;
	}

	public void StartPanLeft()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			transform.Translate(Vector3.left * Time.unscaledDeltaTime * PanSpeed);
		}
	}

	public void StartPanRight()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			transform.Translate(Vector3.right * Time.unscaledDeltaTime * PanSpeed);
		}
	}

	public void StartPanUp()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			transform.Translate(Vector3.forward * Time.unscaledDeltaTime * PanSpeed);
		}
	}

	public void StartPanDown()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			transform.Translate(Vector3.back * Time.unscaledDeltaTime * PanSpeed);
		}
	}

	public void StartLookAhead()
	{
		_isLookingAhead = true;
	}

	public void StopLookAhead()
	{
		_isLookingAhead = false;
	}

	#endregion

	#region Private Methods



	#endregion
}


public enum CameraModeEnum
{
	Leader,
	Party,
}