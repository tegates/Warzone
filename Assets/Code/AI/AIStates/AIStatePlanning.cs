using UnityEngine;
using System.Collections;

public class AIStatePlanning : AIStateBase
{

	// This constructor will create new state taking values from old state
	public AIStatePlanning(AIStateBase state)     
		:this(state.ParentCharacter)
	{

	}

	// this constructor will be used by the other one
	public AIStatePlanning(HumanCharacter parentCharacter)
	{
		this.ParentCharacter = parentCharacter;

		Initialize();
	}



	public override void Update()
	{
		
	}

	private void Initialize()
	{

	}
}
