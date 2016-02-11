﻿using UnityEngine;
using System.Collections;

public class TimerEventHandler 
{

	#region Singleton 
	private static TimerEventHandler _instance;
	public static TimerEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new TimerEventHandler();

			return _instance;
		}
	}
	#endregion

	#region Constructor
	public TimerEventHandler()
	{
		
	}

	#endregion

	public delegate void TimerEventDelegate();
	public static event TimerEventDelegate OnOneSecondTimer;
	public static event TimerEventDelegate OnFiveSecondTimer;


	public void TriggerOneSecondTimer()
	{
		if(OnOneSecondTimer != null)
		{
			OnOneSecondTimer();
		}
	}
}
