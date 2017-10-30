﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour {

	public AudioClip LaunchSound;
	public AudioClip ReloadSound;

	public List<ArrowModifier> CurrentArrowModifiers; 
	private int[] ModiferEffectCounter;

	public GameObject[] Projectiles;
	public GameObject FirePoint;

	public bool isAlive { get; set;}

	private bool lookingAtMathInterface;

	private ArrowSupplier A_Supply;

	private GameObject ArrowToLaunch;

	private bool ArrowLoaded;

	private AudioSource A_Source;
	private ManaBar PowerUpDisplay;

	// Use this for initialization
	void Start () {
		PowerUpDisplay = FindObjectOfType<ManaBar> ();
		ModiferEffectCounter = new int[System.Enum.GetValues (typeof(ArrowModifier)).Length];

		isAlive = true;

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();

		//CreateShot ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Fire1") && lookingAtMathInterface == false && isAlive == true) {

			if (A_Supply.NumberOfArrows > 0) {

				if (ArrowLoaded == false) {
					CreateShot ();
				}

				A_Source.clip = LaunchSound;
				A_Source.volume = .5f;
				A_Source.pitch = .5f;

				A_Source.Play ();


				Launch ();
			} else {
				A_Source.clip = ReloadSound;
				A_Source.volume = 1f;
				A_Source.pitch = 1f;
				A_Source.Play ();
			}

		}
		
	}

	//adds a modification and sets how long that mod will last
	public void AddModifier(ArrowModifier newModification, int PowerUpIndex)
	{
		CurrentArrowModifiers.Add (newModification);

		StartCoroutine (DelayRemovePowerUp (newModification, PowerUpIndex));

		//set the counter of the associated int
		//ModiferEffectCounter [(int)newModification] = ArrowDuration;

	}

	IEnumerator DelayRemovePowerUp(ArrowModifier removeModification, int PowerUpIndex)
	{
		yield return new WaitForSeconds (30);

		RemoveModifier (removeModification);
		PowerUpDisplay.ClearPowerUp (PowerUpIndex);

	}

	public void ClearModifiers()
	{
		CurrentArrowModifiers.Clear ();

		//set all array values to zero
		for (int i = 0; i < ModiferEffectCounter.Length; i++) {
			ModiferEffectCounter [i] = 0;
		}

	}

	public void RemoveModifier(ArrowModifier removeModification)
	{
		//reduce count by 1
		//ModiferEffectCounter[(int)(removeModification)] -= 1;

		//if the count reaches zero, remove this modifier
		//if (ModiferEffectCounter [(int)(removeModification)] <= 0) {
		CurrentArrowModifiers.Remove (removeModification);
		//}
	}


	public void SetLookingAtInterface(bool isLooking)
	{
		//Debug.Log ("Here");
		lookingAtMathInterface = isLooking;
	}

	void CreateShot()
	{
		//we Instantiate(create) a bullet at the postion and rotation of fire point
		ArrowToLaunch = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], FirePoint.transform.position, FirePoint.transform.rotation);

		ArrowToLaunch.transform.parent = FirePoint.transform;
		ArrowToLaunch.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));

		//go through the list of modifiers and add them to the Arrow to give special abilities
		for (int i = 0; i < CurrentArrowModifiers.Count; i++) {
			switch(CurrentArrowModifiers[i])
			{
			case ArrowModifier.Bomb:
				//RemoveModifier (ArrowModifier.Bomb);
				ArrowToLaunch.AddComponent<BombArrow> ();
				break;

		/*	case ArrowModifier.Homing:
				//RemoveModifier (ArrowModifier.Homing);
				ArrowToLaunch.AddComponent<HomingArrow> ();
				break;*/

			case ArrowModifier.Burst:
				//RemoveModifier (ArrowModifier.Burst);
				ArrowToLaunch.AddComponent<BurstArrow> ();
				break;

			case ArrowModifier.Shotgun:
				//RemoveModifier (ArrowModifier.Shotgun);
				ArrowToLaunch.AddComponent<ShotgunArrow> ();
				break;

			case ArrowModifier.Giant:
				//RemoveModifier (ArrowModifier.Giant);
				ArrowToLaunch.AddComponent<GiantSlayerArrow> ();
				break;

			}
		}

		ArrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = true;

		ArrowToLaunch.GetComponent<BoxCollider> ().enabled = false; 

		ArrowToLaunch.GetComponent<Rigidbody> ().useGravity = false;
	}

	void Launch()
	{
		ArrowToLaunch.transform.parent = null;

		ArrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = false;

		//ArrowToLaunch.GetComponent<Rigidbody> ().useGravity = true;

		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		ArrowToLaunch.GetComponent<Rigidbody> ().AddForce (FirePoint.transform.right * -7000);

		ArrowToLaunch.GetComponent<BoxCollider> ().enabled = true; 

		//an arrow can have multiple arrow class components
		ArrowClass[] ArrowModifers = ArrowToLaunch.GetComponents<ArrowClass> ();
		for (int i = 0; i < ArrowModifers.Length; i++) {
			ArrowModifers [i].ArrowLaunched ();
		}

		StartCoroutine (ReloadTime ());
	

	}


	IEnumerator ReloadTime()
	{
		yield return new WaitForSeconds (.2f);

		A_Supply.UseArrow ();

		if (A_Supply.NumberOfArrows > 0) {
			
			CreateShot ();
			ArrowLoaded = true;
		} else {
			ArrowLoaded = false;
		}
	}

}
