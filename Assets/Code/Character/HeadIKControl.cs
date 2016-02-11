using UnityEngine;
using System.Collections;

public class HeadIKControl : MonoBehaviour 
{

	public Transform LookTarget; 
	public bool Enabled;
	protected Animator MyAnimator;
	
	public void Initialize()
	{
		MyAnimator = GetComponent<Animator>();
		
		Enabled = true;
	}
	
	void OnAnimatorIK()
	{
		if(MyAnimator) 
		{
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(Enabled) 
			{
				//set look target
				if(LookTarget != null)
				{
					MyAnimator.SetLookAtWeight(1);
					MyAnimator.SetLookAtPosition(LookTarget.position);
				}
			}
		}
	}
}
