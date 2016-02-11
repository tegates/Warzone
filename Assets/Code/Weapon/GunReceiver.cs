using UnityEngine;
using System.Collections;

public class GunReceiver : MonoBehaviour 
{
	public float Recoil;
	public float SemiFireRate;
	public float AutoFireRate;
	public float BurstFireRate;
	public float LessMoveSpeed;

	public GunFireModes [] FireModes;

	public GunReceiver()
	{
		

	}
}
