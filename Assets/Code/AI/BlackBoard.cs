using UnityEngine;
using System.Collections;

public class BlackBoard
{
	public Vector3 AimPoint;

	private Vector3 _navTarget;
	public bool IsNavTargetSet;
	public Vector3 NavTarget
	{
		get { return _navTarget;}
		set { _navTarget = value; }
	}


	private Character _targetEnemy;
	public Character TargetEnemy
	{
		get { return _targetEnemy; }
		set { _targetEnemy = value; }
	}

	public bool IsTargetEnemyHittable;
	public float TargetEnemyThreat;

	public Vector3 LastKnownEnemyPosition;
	private Character _invisibleEnemy;
	public Character InvisibleEnemy
	{
		get { return _invisibleEnemy; }
		set { _invisibleEnemy = value; }
	}

	public float HighestPersonalThreat;
	public Vector3 AvgPersonalThreatDir;

	public bool HasPatrolInfo;
	private Vector3 _patrolLoc;
	public Vector3 PatrolLoc
	{
		get { return _patrolLoc; }
		set { _patrolLoc = value; }
	}

	private Vector3 _patrolRange;
	public Vector3 PatrolRange
	{
		get { return _patrolRange; }
		set { _patrolRange = value; }
	}


	private Weapon _focusedWeapon;
	public Weapon FocusedWeapon
	{
		get { return _focusedWeapon; }
		set { _focusedWeapon = value; }
	}

	private Weapon _equippedWeapon;
	public Weapon EquippedWeapon
	{
		get { return _equippedWeapon; }
		set { _equippedWeapon = value; }
	}
}
