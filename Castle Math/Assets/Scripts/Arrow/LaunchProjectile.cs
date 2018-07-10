using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour {

	//player
	DoorHealth health;

	//UI
	bool lookingAtMathInterface;
	ManaBar PowerUpDisplay;

	//arrow
	public GameObject crossbow;
	public GameObject firePoint;
	public GameObject[] Projectiles;
	public bool isAlive { get; set;}
	public List<ArrowModifier> CurrentArrowModifiers;
    public TutorialBehavior tutorialBehavior;

    ArrowSupplier A_Supply;
	GameObject arrowToLaunch;
	bool burst;
    bool rapidFire;
    float rapidFireMod = 1 / 3;
    float quickShotMod = 1 / 1.75f;
    bool quickShot;
	bool ArrowLoaded;
	int[] ModiferEffectCounter;
	GameObject tempArrow;
	bool firstShot = true;
	Animator crossbowAnim;
	bool reloading = false;
    float reloadTime = .2f;

	//Audio
	AudioSource A_Source;
	public AudioClip[] LaunchSounds;
	public AudioClip LaunchSound;
	public AudioClip ReloadSound;

    void Awake()
    {
       // GameStateManager.instance.player = this;
    }

	// Use this for initialization
	void Start () {
        GameStateManager.instance.player = this;
        PowerUpDisplay = FindObjectOfType<ManaBar> ();
		ModiferEffectCounter = new int[System.Enum.GetValues (typeof(ArrowModifier)).Length];
		crossbowAnim = crossbow.GetComponent<Animator>();
		
		isAlive = true;

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		//we Instantiate(create) a bullet at the postion and rotation of fire point
		
		//arrow we create and then delete after first one is shot.
		tempArrow = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], firePoint.transform.position, firePoint.transform.rotation);
		tempArrow.transform.parent = firePoint.transform;
		tempArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		tempArrow.GetComponent<Rigidbody> ().useGravity = false;

	}

    // Update is called once per frame
    void Update()
    {
        if (!lookingAtMathInterface && isAlive && !GameStateManager.instance.levelManager.isGamePaused)
        { 
            if (Input.GetButton("Fire1") && rapidFire)
            {
                if (A_Supply.NumberOfArrows > 0)
                {

                    if (ArrowLoaded == false)
                    {
                        CreateShot();
                    }
                    
                    if (!reloading)
                    {
                        playShootingSound(Random.Range(0, LaunchSounds.Length));
                        Launch(rapidFireMod);
                    }
                }
                
            }
            else if (Input.GetButtonDown("Fire1"))
            {

                if (A_Supply.NumberOfArrows > 0)
                {

                    if (ArrowLoaded == false)
                    {
                        CreateShot();
                    }

                    playShootingSound(Random.Range(0, LaunchSounds.Length));

                    if (!reloading)
                    {
                        if (quickShot) Launch(quickShotMod);
                        else Launch();
                    }
                }
                else
                {
                    A_Source.clip = ReloadSound;
                    A_Source.volume = .6f;
                    A_Source.pitch = 1f;
                    A_Source.Play();
                }

            }
        }
		
	}

    public void RefreshArrow()
    {
        CreateShot();
    }

	//adds a modification and sets how long that mod will last
	public void AddModifier(ArrowModifier newModification, int PowerUpIndex)
	{
		CurrentArrowModifiers.Add (newModification);

        CreateShot();

		StartCoroutine (DelayRemovePowerUp (newModification, PowerUpIndex));

		//set the counter of the associated int
		//ModiferEffectCounter [(int)newModification] = ArrowDuration;
	}
	
	void setModifiers(){
		//an arrow can have multiple arrow class components
		ArrowClass[] ArrowModifiers = arrowToLaunch.GetComponents<ArrowClass> ();
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

	public void RemoveModifier(ArrowModifier removeModification){
		//reduce count by 1
		//ModiferEffectCounter[(int)(removeModification)] -= 1;

		//if the count reaches zero, remove this modifier
		//if (ModiferEffectCounter [(int)(removeModification)] <= 0) {
		CurrentArrowModifiers.Remove (removeModification);
		burst = false;
		//}
	}


	public void SetLookingAtInterface(bool isLooking){
		lookingAtMathInterface = isLooking;
	}

	void CreateShot(){
		reloading = false;
		ArrowLoaded = true;

		if(firstShot){
			Destroy(tempArrow);
			firstShot = false;
		}
        //we Instantiate(create) a bullet at the postion and rotation of fire point
        foreach(Transform child in firePoint.GetComponentsInChildren<Transform>())
        {
            if (child != firePoint.transform)
            {
                Destroy(child.gameObject);
            }
        }
		arrowToLaunch = Instantiate (Projectiles[Mathf.Clamp(A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1], 0, A_Supply.ArrowIndex.Count-1)], firePoint.transform.position, firePoint.transform.rotation);

		arrowToLaunch.transform.parent = firePoint.transform;
		arrowToLaunch.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));

		//go through the list of modifiers and add them to the Arrow to give special abilities
		for (int i = 0; i < CurrentArrowModifiers.Count; i++) {
			switch(CurrentArrowModifiers[i])
			{
			case ArrowModifier.Bomb:
				//RemoveModifier (ArrowModifier.Bomb);
				arrowToLaunch.GetComponent<BombArrow>().activate(true);
				break;
			case ArrowModifier.Burst:
				//RemoveModifier (ArrowModifier.Burst);
				burst = true;
				break;
			case ArrowModifier.Invincible:
				break;
			case ArrowModifier.Spread:
				//RemoveModifier (ArrowModifier.Shotgun);
				arrowToLaunch.AddComponent<ShotgunArrow> ().activate(true);
				break;
            case ArrowModifier.Fire:
                arrowToLaunch.AddComponent<FireArrowScript>();
                break;
            case ArrowModifier.Ice:
                arrowToLaunch.AddComponent<IceArrowScript>();
                break;
            case ArrowModifier.Shock:
                arrowToLaunch.AddComponent<ShockArrowScript>();
                break;

			}
		}
		arrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = true;
		arrowToLaunch.GetComponent<BoxCollider> ().enabled = false; 
		arrowToLaunch.GetComponent<Rigidbody> ().useGravity = false;
	}

	public void Launch(float reloadModifier = 1){
		crossbowAnim.Play("crossbowShot");
		arrowToLaunch.transform.parent = null;
		arrowToLaunch.GetComponent<ProjectileBehavior> ().isGrounded = false;
        ArrowClass[] arrowAttributes = arrowToLaunch.GetComponents<ArrowClass>();
        foreach(ArrowClass ac in arrowAttributes)
        {
            ac.ArrowLaunched();
        }
		setModifiers();
		if(burst) LaunchBurst();
		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		arrowToLaunch.GetComponent<Rigidbody> ().useGravity = true;
		arrowToLaunch.GetComponent<Rigidbody> ().AddForce (firePoint.transform.right * -5000);
		arrowToLaunch.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(arrowToLaunch, 1.2f);
		StartCoroutine (ReloadTime (reloadModifier));
	}

	void playShootingSound(int soundNum){
		A_Source.clip = LaunchSounds [soundNum];
		A_Source.Play();
	}
	
	IEnumerator ReloadTime(float reloadModifier = 1){
		reloading = true;
		yield return new WaitForSeconds (reloadTime*reloadModifier);

		if(tutorialBehavior && tutorialBehavior.tutorialDone) A_Supply.UseArrow ();

		if (A_Supply.NumberOfArrows > 0) {
			CreateShot();
		} else {
			ArrowLoaded = false;
		}
	}
	
	public void LaunchBurst(){
		StartCoroutine (BurstShot());
	}

    public void SetQuickShot(float duration)
    {
        StartCoroutine(QuickShotCooldown(duration));
    }

    IEnumerator QuickShotCooldown(float duration)
    {
        quickShot = true;
        yield return new WaitForSeconds(duration);
        quickShot = false;
    }

    public void SetRapidFire(float duration)
    {
        StartCoroutine(RapidFireCooldown(duration));
    }

    IEnumerator RapidFireCooldown(float duration)
    {
        rapidFire = true;
        yield return new WaitForSeconds(duration);
        rapidFire = false;
    }

    /*
    IEnumerator Booldown(ref bool targetBool, float duration)
    {
        targetBool = true;
        yield return new WaitForSeconds(duration);
        targetBool = false;
    }
    */
	
	//creates 2 extra shots after the first shot
	IEnumerator BurstShot(){
		yield return new WaitForSeconds (.12f);
		//Instantiate arrow at the postion and rotation of fire point
		GameObject burstArrow = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], firePoint.transform.position, firePoint.transform.rotation);
		burstArrow.transform.parent = firePoint.transform;
		burstArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		burstArrow.transform.parent = null;
		burstArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;
		//We then access the rigidbody of the bullet and apply a strong forward force to it. 
		burstArrow.GetComponent<Rigidbody> ().AddForce (firePoint.transform.right * -5000);
		burstArrow.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(burstArrow, 1.2f);

		playShootingSound(0);
		yield return new WaitForSeconds (.12f);
		GameObject burstArrow2 = Instantiate (Projectiles[A_Supply.ArrowIndex[A_Supply.NumberOfArrows-1]], firePoint.transform.position, firePoint.transform.rotation);
		burstArrow2.transform.parent = firePoint.transform;
		burstArrow2.transform.localRotation = Quaternion.Euler (new Vector3 (0, -83, 0));
		burstArrow2.transform.parent = null;
		burstArrow2.GetComponent<ProjectileBehavior> ().isGrounded = false;
		//we then access the rigidbody of the bullet and apply a strong forward force to it. 
		burstArrow2.GetComponent<Rigidbody> ().AddForce (firePoint.transform.right * -5000);
		burstArrow2.GetComponent<BoxCollider> ().enabled = true; 
		Destroy(burstArrow2, 1.2f);
		playShootingSound(4);
	}

    public bool IsUnderTheInfluence()
    {
        return CurrentArrowModifiers.Count > 0;
    }

}
