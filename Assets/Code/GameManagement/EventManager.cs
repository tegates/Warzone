using UnityEngine;
using System.Collections;

public class EventManager
{
	#region Public fields
	
	
	#endregion
	
	#region Private fields
	
	#endregion
	
	#region Public methods
	public void Initialize()
	{
		
	}
	
	public void ManagerPerFrameUpdate()
	{
		//PlayerInputEventHandler.Instance.HandleInputs();
		InputEventHandler.Instance.HandleKeyInputs();

		//PlayerInputEventHandler.Instance.HandleMouseMovementInputs();

	}
	#endregion
	
	
	#region Private methods
	
	
	
	#endregion



}
