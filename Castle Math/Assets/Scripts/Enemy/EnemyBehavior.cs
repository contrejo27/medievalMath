using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
	private GameStateManager GameManager;

	//enemy
	public int HitPoints;
	bool dead = false;
	public float MoveSpeed;
	bool attacking = false;
	
	//audio
	public AudioClip[] deathSounds;
	public AudioClip[] attackSounds;
	public AudioClip footstepSound;
	private AudioSource[] A_Source;

	//animation
	private Animator Anim; 
	public float attackDistance;
	private bool isMoving;
	public bool AtTarget {get; set;}
	
	//environment
	private doorHealth dH;
	public GameObject Target;

	private int currentAudioSource;
	
	// Use this for initialization
	void Start () {

		Anim = GetComponent<Animator> ();

		isMoving = false;
		GameManager = GameObject.FindObjectOfType<GameStateManager> ();

		//get 3 different audio sources so they don't overlap all the time
		A_Source = new AudioSource[] {GameObject.Find ("EnemyAudio").GetComponent<AudioSource>(),
					GameObject.Find ("EnemyAudio2").GetComponent<AudioSource>(),
					GameObject.Find ("EnemyAudio3").GetComponent<AudioSource>()};

		//Out of the available targets choose a one randomly as our target
		//GameObject[] Targets = GameObject.FindGameObjectsWithTag ("Target");
		//int RanNum = Random.Range (0, Targets.Length);
		//Target = Targets [RanNum];
		StartCoroutine(waitToPlay(2f));

		StartCoroutine (WalkToTarget());
	}
	
	IEnumerator waitToPlay(float time){
		yield return new WaitForSeconds (time);
		currentAudioSource = Random.Range(0, A_Source.Length);
		A_Source[currentAudioSource].loop = true;
		A_Source[currentAudioSource].clip = footstepSound;
		A_Source[currentAudioSource].Play ();
	}
	void Update()
	{
		//if he's alive have him walk to target
		if (HitPoints > 0) {
			//rotate the character correctly in the direction of the heading
			Quaternion newRot = Quaternion.LookRotation (Target.transform.position - this.transform.position);
			transform.rotation = Quaternion.Euler (new Vector3 (0, newRot.eulerAngles.y, 0));

			if (isMoving == false) StartCoroutine (WalkToTarget());
		}
		/*
		if (dH.GetComponent<Renderer> ().enabled == false) {
			Anim.SetBool ("isMoving", true);
			Anim.Play ("move");
			this.GetComponent<Rigidbody> ().velocity = transform.forward * MoveSpeed;
		}*/
	}
		
	public void SetTarget(int initialTarget){
		string targetName = "gateCollision" + initialTarget;
		Target =  GameObject.Find (targetName);
		dH = GameObject.Find (targetName).GetComponent<doorHealth>();
	}
	
	public void TakeDamage(int DMG)
	{
//		Anim.SetBool ("isHit", true);
		if(!Anim.GetCurrentAnimatorStateInfo(0).IsName("death")) Anim.Play("wound1");

		//Anim.SetBool ("isMoving", false);
		//Anim.SetBool ("isHit", true);

		HitPoints -= DMG;
		if (HitPoints <= 0) {
			if(!dead){
				Killed ();
				dead=true;
			}
		}
	}

	IEnumerator WalkToTarget()
	{
		isMoving = true;
		Vector3 StartPos = this.transform.position;
		Vector3 heading = (Target.transform.position - StartPos);
		float distance = heading.magnitude;
		Anim.SetBool ("isMoving", true);

		//move tbe player at a constant velocity to the target until they are a certain disstance away
		while (distance > attackDistance && HitPoints>0) {
			//Calculate the current heading, normalized
			heading = (Target.transform.position  - this.transform.position).normalized;

			//what is the current distance between the characters
			distance = Mathf.Abs(Vector3.Distance (this.transform.position, Target.transform.position));

			//set velocity
			this.GetComponent<Rigidbody> ().velocity = (heading * MoveSpeed);
			transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position)-9.3f,transform.position.z);
			yield return new WaitForFixedUpdate ();

		}

		//make sure this only happens when the soldier is alive
		if (HitPoints > 0 && !attacking) {
			attacking = true;
			print("*****");
			currentAudioSource = Random.Range(0, A_Source.Length);

			A_Source[currentAudioSource].loop = false;
			A_Source[currentAudioSource].Stop();

			isMoving = false;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			Anim.SetBool ("isMoving", false);
			/*
			 if (Time.time > nextActionTime ) {
				nextActionTime = Time.time + period; 
				dH.Health -= 8;
			 }
			print(dH.Health);
			*/
			Anim.SetBool ("isAttacking", true);
			//ChooseWalkPoint ();
		}


	}

	//attach the soldier to the crank 
	void ChooseWalkPoint()
	{
        this.transform.parent = Target.transform.GetChild (0).transform;
        this.transform.position = Target.transform.GetChild (0).transform.position;
	}


	//do this when the player gets killed
	public void Killed()
	{
		currentAudioSource = Random.Range(0, A_Source.Length);
		A_Source[currentAudioSource].loop = false;
		A_Source[currentAudioSource].clip = deathSounds[Random.Range(0, deathSounds.Length)];
		A_Source[currentAudioSource].Play ();

		Collider enemyHitbox = this.GetComponent<Collider>();
		Destroy(enemyHitbox);
		
		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;		
		
		Anim.SetBool ("isAttacking", false);
		Anim.SetBool ("isMoving", false);

		Anim.SetBool ("isDead", true);
		Anim.Play("death");

		GameManager.EnemyKilled ();
		StartCoroutine (WaitToDestroy());
	}

	//wait to 
	IEnumerator WaitToDestroy()
	{
		yield return new WaitForSeconds (2);
		Destroy (this.gameObject);
	}

	public void DamageGate(int damage) {
		currentAudioSource = Random.Range(0, A_Source.Length);
		A_Source[currentAudioSource].clip = attackSounds[Random.Range(0, attackSounds.Length)];
		A_Source[currentAudioSource].Play ();
		dH.TakeDamageGate (damage);
	}


}
