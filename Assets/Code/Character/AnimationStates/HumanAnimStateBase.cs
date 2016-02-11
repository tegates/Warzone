using UnityEngine;
using System.Collections;

public abstract class HumanAnimStateBase
{
	public HumanCharacter ParentCharacter;

	public abstract void SendCommand(HumanCharCommands command);
	public abstract void Update();
}
