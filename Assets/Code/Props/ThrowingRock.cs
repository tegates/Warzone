using UnityEngine;
using System.Collections;

public class ThrowingRock : MonoBehaviour 
{
	public bool IsThrown;

	private float _lifeTime; //time to live
	private float _lifeTimer;
	private float _onGroundTime; //time to live after first collision
	private float _groundTimer;

	private bool _hasCollided;


	// Use this for initialization
	void Start () 
	{
		_lifeTime = 5;
		_onGroundTime = 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(IsThrown)
		{
			_lifeTimer += Time.deltaTime;
			if(_lifeTimer >= _lifeTime)
			{
				//destroy the rock
				GameObject.Destroy(gameObject);
			}
		}
		if(_hasCollided)
		{
			_groundTimer += Time.deltaTime;
			if(_groundTimer >= _onGroundTime)
			{
				//destroy the rock
				GameObject.Destroy(gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(!_hasCollided && IsThrown)
		{
			_hasCollided = true;
			Noise noise = new Noise();
			noise.Enabled = true;
			noise.NoiseType = NoiseTypeEnum.Impact;
			noise.Location = collision.contacts[0].point;
			noise.SourceCharacter = null;
			noise.Volume = 0.35f;

			SoundEventHandler.Instance.TriggerNoiseEvent(noise);
		}
	}

}
