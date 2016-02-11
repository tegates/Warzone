using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

[RequireComponent(typeof(CharacterEventHandler))]
[RequireComponent(typeof(LeftHandIKControl))]
[RequireComponent(typeof(LookAtIK))]
[RequireComponent(typeof(AimIK))]




public class CharacterReference : MonoBehaviour
{


	public GameObject RightHandWeaponMount;
	public GameObject TorsoWeaponMount;
	public GameObject CurrentWeapon;
	public GameObject Eyes;
	public Character ParentCharacter;
}
