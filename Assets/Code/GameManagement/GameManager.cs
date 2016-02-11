using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour 
{

	#region Singleton
	
	public static GameManager Inst;


	#endregion

	#region Public Fields

	public EventManager EventManager;
	public FXManager FXManager;
	public NPCManager NPCManager;
	public DBManager DBManager;

	public CameraController CameraController;
	public PlayerControl PlayerControl;

	public AIScheduler AIScheduler;

	#endregion

	void Start()
	{
		UnityEngine.Debug.Log("Game Manager Started");
		Initialize();
	}

	void Update()
	{
		EventManager.ManagerPerFrameUpdate();
		PlayerControl.PerFrameUpdate();
		AIScheduler.UpdatePerFrame();
	}


	#region Private Methods

	private void Initialize()
	{
		Inst = this;

		//Initializing CsDebug
		CsDebug debug = GetComponent<CsDebug>();
		debug.Initialize();

		//Initializing DBManager
		DBManager = new DBManager();
		DBManager.Initialize();

		//Initializing Event Manager
		EventManager = new EventManager();
		EventManager.Initialize();

		//Initializing NPC Manager
		NPCManager = new NPCManager();
		NPCManager.Initialize();


		//SelectedPC = GameObject.Find("HumanCharacter").GetComponent<HumanCharacter>();
		//SelectedPC.Initialize();
		PlayerControl = new PlayerControl();
		PlayerControl.Initialize();
		GameObject.Find("HumanCharacter2").GetComponent<HumanCharacter>().Initialize();
		//GameObject.Find("HumanCharacter3").GetComponent<HumanCharacter>().Initialize();

		CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
		CameraController.Initialize();

		FXManager = new FXManager();
		FXManager.Initialize(50);

		AIScheduler = new AIScheduler();
		AIScheduler.Initialize();

		StartCoroutine(DoPerSecond());

	}



	private void PerSecondUpdate()
	{
		TimerEventHandler.Instance.TriggerOneSecondTimer();
	}







	#endregion

	#region Coroutines
	IEnumerator DoPerSecond()
	{
		for(;;)
		{
			PerSecondUpdate();
			yield return new WaitForSeconds(1);
		}

	}


	#endregion
}

