using UnityEngine;
using System.Collections;

public abstract class AIStateBase 
{
	public HumanCharacter ParentCharacter;

	public abstract void Update();

}
