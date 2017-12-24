using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour {

	//player
	public doorHealth health;

	//UI
	private bool lookingAtMathInterface;
	private ManaBar PowerUpDisplay;

	//arrow
	private ArrowSupplier A_Supply;
	private GameObject ArrowToLaunch;
	private bool burst;
	public bool isAlive { get; set;}
	private bool ArrowLoaded;
	public List<ArrowModifier> CurrentArrowModifiers; 
	private int[] ModiferEffectCounter;
	public GameObject[] Projectiles;
	public GameObject FirePoint;
	private GameObject tempArrow;
	private bool firstShot = true;
	
	//Audio
	private AudioSource A_Source;
	public AudioClip[] LaunchSounds;
	public AudioClip LaunchSound;
	public AudioClip ReloadSound;

	// Use this for initialization
	void Start () {
		PowerUpDisplay = FindObjectOfType<ManaBar> ();
		ModiferEffectCounter = new int[System.Enum.GetValues (typeof(ArrowModifier)).Length];

		isAlive = true;

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		//we Instantiate(create) a bullet at the postion and rotation of fire point
		
		//arrow we create and then delete after first one is shot.
		tempArrow = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], FirePoint.transform.position, FirePoint.transform.rotation);
		tempArrow.transform.parent = FirePoint.transform;
		tempArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		tempArrow.GetComponent<Rigidbody> ().useGravity = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Fire1") && lookingAtMathInterface == false && isAlive == true) {

			if (A_Supply.NumberOfArrows > 0) {

				if (ArrowLoaded == false) {
					CreateShot ();
				}

				playShootingSound(Random.Range (0, LaunchSounds.Length));
				Launch ();
			} else {
				A_Source.clip = ReloadSound;
				A_Source.volume = .6f;
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
	
	void setModifiers(){
		//an arrow can have multiple arrow class components
		ArrowClass[] ArrowModifiers = ArrowToLaunch.GetComponents<ArrowClass> ();
		for (int i = 0; i < ArrowModifiers.Length; i++) {
			ArrowModifiers [i].ArrowLaunched();
		}
	}
	
	IEnumerator DelayRemovePowerUp(ArrowModifier removeModification, int PowerUpIndex)
	{
		yield return new WaitForSeconds (15);

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
		burst = false;
		//}
	}


	public void SetLookingAtInterface(bool isLooking)
	{
		lookingAtMathInterface = isLooking;
	}

	void CreateShot()
	{
		if(firstShot){
			Destroy(tempArrow);
			firstShot = false;
		}
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
				ArrowToLaunch.GetComponent<BombArrow>().activate(true);
				break;
			case ArrowModifier.Burst:
				//RemoveModifier (ArrowModifier.Burst);
				burst = true;
				break;
			case ArrowModifier.Invincible:
				break;
			case ArrowModifier.Spread:
				//RemoveModifier (ArrowModifier.Shotgun);
				ArrowToLaunch.AddComponent<ShotgunArrow> ().activate(true);
				break;
			}
		}
		ArrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = true;
		ArrowToLaunch.GetComponent<BoxCollider> ().enabled = false; 
		ArrowToLaunch.GetComponent<Rigidbody> ().useGravity = false;
	}

	public void Launch()
	{
		ArrowToLaunch.transform.parent = null;
		ArrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = false;
		setModifiers();
		if(burst) LaunchBurst();
		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		ArrowToLaunch.GetComponent<Rigidbody> ().useGravity = true;
		ArrowToLaunch.GetComponent<Rigidbody> ().AddForce (FirePoint.transform.right * -5000);
		ArrowToLaunch.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(ArrowToLaunch, 1.2f);
		StartCoroutine (ReloadTime ());
	}

	void playShootingSound(int soundNum){
		A_Source.clip = LaunchSounds [soundNum];
		A_Source.Play ();
	}
	
	IEnumerator ReloadTime()
	{
		yield return new WaitForSeconds (.3f);

		A_Supply.UseArrow ();

		if (A_Supply.NumberOfArrows > 0) {
			CreateShot ();
			ArrowLoaded = true;
		} else {
			ArrowLoaded = false;
		}
	}
	
	public void LaunchBurst()
	{
		StartCoroutine (burstShot());
	}
	
	//creates 2 extra shots after the first shot
	IEnumerator burstShot()
	{
		yield return new WaitForSeconds (.12f);
		//Instantiate arrow at the postion and rotation of fire point
		GameObject burstArrow = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], FirePoint.transform.position, FirePoint.transform.rotation);
		burstArrow.transform.parent = FirePoint.transform;
		burstArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		burstArrow.transform.parent = null;
		burstArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;
		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		burstArrow.GetComponent<Rigidbody> ().AddForce (FirePoint.transform.right * -5000);
		burstArrow.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(burstArrow, 1.2f);

		playShootingSound(0);
		yield return new WaitForSeconds (.12f);
		GameObject burstArrow2 = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], FirePoint.transform.position, FirePoint.transform.rotation);
		burstArrow2.transform.parent = FirePoint.transform;
		burstArrow2.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		burstArrow2.transform.parent = null;
		burstArrow2.GetComponent<ProjectileBehavior> ().isGrounded = false;
		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		burstArrow2.GetComponent<Rigidbody> ().AddForce (FirePoint.transform.right * -5000);
		burstArrow2.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(burstArrow2, 1.2f);
		playShootingSound(4);
	}

}
