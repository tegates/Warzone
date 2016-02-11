using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScheduler
{
	private int _humanIndex;
	private int _oneSecHumanIndex;

	public void Initialize()
	{
		TimerEventHandler.OnOneSecondTimer -= OnOneSecondTimer;
		TimerEventHandler.OnOneSecondTimer += OnOneSecondTimer;
	}

	public void OnOneSecondTimer()
	{
		//start calling each AI's OnOneSecondTimer one per frame
		_oneSecHumanIndex = 0;
	}

	public void UpdatePerFrame()
	{
		//call each AI's per frame udpate
		List<HumanCharacter> humans = GameManager.Inst.NPCManager.HumansInScene;
		if(humans.Count > _humanIndex && humans[_humanIndex] != null && humans[_humanIndex].MyAI.ControlType != AIControlType.Player)
		{
			humans[_humanIndex].MyAI.PerFrameUpdate();


		}

		_humanIndex ++;
		if(_humanIndex >= humans.Count)
		{
			_humanIndex = 0;
		}


		if(_oneSecHumanIndex >= 0 && humans.Count > _oneSecHumanIndex && humans[_oneSecHumanIndex] != null && humans[_oneSecHumanIndex].MyAI.ControlType != AIControlType.Player)
		{
			humans[_oneSecHumanIndex].MyEventHandler.TriggerOnOneSecondTimer();

		}
		_oneSecHumanIndex ++;
		if(_oneSecHumanIndex >= humans.Count)
		{
			_oneSecHumanIndex = -1000;
		}

	}
}
