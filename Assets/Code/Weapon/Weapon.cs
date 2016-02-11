using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	public Vector3 InHandPosition;
	public Vector3 InHandAngles;
	public Transform AimTransform;
	public Transform ForeGrip;
	public bool IsRanged;

	public delegate void WeaponCallBack();

	public virtual void Rebuild(WeaponCallBack callBack)
	{

	}

	public virtual float GetTotalLessMoveSpeed()
	{
		return 0;
	}
}
