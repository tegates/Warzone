using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager
{
	public List<HumanCharacter> HumansInScene
	{
		get { return _humansInScene; }
	}

	private List<HumanCharacter> _humansInScene;

	public void Initialize()
	{
		_humansInScene = new List<HumanCharacter>();
	}

	public void AddHumanCharacter(HumanCharacter character)
	{
		_humansInScene.Add(character);
	}
}
