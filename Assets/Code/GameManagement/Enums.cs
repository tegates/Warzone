public enum PlayerMoveDirEnum
{
	Stop,
	Left,
	Right,
	Up,
	Down,
}

public enum HumanBodyStates
{
	StandIdle,
	WalkForward,
	WalkAiming,//when aiming opposite of movement direction, must be walkAiming
	RunForward,
	RunAiming,
	Sprint,
	CrouchIdle,
	CrouchWalk,
	CrouchWalkAiming,
	Knock,
}

public enum HumanUpperBodyStates
{
	None,
	Idle,
	Aim,
}

public enum HumanActionStates
{
	None,
	Reload,
	Throw,
	SwitchWeapon,
	Kick,

}

public enum HumanCharCommands
{
	Idle,
	GoToPosition,
	GoDirection,
	Walk,
	StopWalk,
	Sprint,
	StopSprint,
	Crouch,
	StopCrouch,
	Aim,
	StopAim,
	PullTrigger,
	ReleaseTrigger,
	Reload,
	FinishReload,
	CancelReload,
	Unarm,
	SwitchWeapon1,
	SwitchWeapon2,
	Knock,
	ThrowGrenade,
	FinishThrow,
}

public enum HumanStances
{
	Walk,
	Crouch,
	CrouchRun,
	Run,
	Sprint,
}


public enum WeaponAnimType
{
	Unarmed,
	Pistol,
	Longgun,
	Melee,
}

public enum ReloadAnimType
{
	SemiAuto,
	Revolver,
	Rifle,
	Shotgun,
}

public enum FXType
{
	BulletHole,
	BulletImpact,
	Explosion,
}

public enum GunFireModes
{
	Semi,
	Full,
	Burst,
}

public enum GunCalibers
{
	m9x19,
	m9x18,
	m9x39,
	m762x39,
	m762x54R,
	m556x45,
	i45,
	i357,
	i44,
	i308,
	g12,
}

public enum AIControlType
{
	Player,
	PlayerTeam,
	NPC,
}

public enum Faction
{
	Civilian,
	FactionA,
	FactionB,
}