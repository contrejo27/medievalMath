using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

	public float MoveSpeed;
	public int HitPoints;

	public AudioClip[] DeathSounds;

	public GameObject Target;

	private GameStateManager GameManager;

	public bool AtTarget {get; set;}

	private bool isMoving;

	private AudioSource A_Source;

	private Animator Anim; 

	private doorHealth dH;
	public GameObject doorTarget;
	public int damageVal;

	private float nextActionTime = 0.0f;
	public float period = 2f;
	bool dead = false;
	// Use this for initialization
	void Start () {

		Anim = GetComponent<Animator> ();
		dH = GameObject.Find ("gateCollision").GetComponent<doorHealth>();

		isMoving = false;
		GameManager = GameObject.FindObjectOfType<GameStateManager> ();

		A_Source = GameObject.Find ("EnemyAudio").GetComponent<AudioSource>();

		//Out of the available targets choose a one randomly as our target
		GameObject[] Targets = GameObject.FindGameObjectsWithTag ("Target");

		int RanNum = Random.Range (0, Targets.Length);

		Target = Targets [RanNum];

		//attaching gate1 as target
		doorTarget = GameObject.Find ("gateCollision");

		StartCoroutine (WalkToTarget());
	}

	void Update()
	{
		if (HitPoints > 0) {
			float distance = Mathf.Abs (Vector3.Distance (this.transform.position, Target.transform.position));

			//rotate the character correctly in the direction of the heading
			Quaternion newRot = Quaternion.LookRotation (Target.transform.position - this.transform.position);
			transform.rotation = Quaternion.Euler (new Vector3 (0, newRot.eulerAngles.y, 0));


			if (isMoving == false) {//distance > 8f && 
				StartCoroutine (WalkToTarget());

				if (AtTarget == true && HitPoints > 0) {
					//trigger hitting animation
				}
			}
		}
		
		if (dH.GetComponent<Renderer> ().enabled == false) {
			Anim.SetBool ("isMoving", true);
			Anim.Play ("move");
			this.GetComponent<Rigidbody> ().velocity = transform.forward * MoveSpeed;

		}

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
		while (distance > 10 && HitPoints>0) {

			//Calculate the current heading, normalized
			heading = (Target.transform.position  - this.transform.position).normalized;

			//what is the current distance between the characters
			distance = Mathf.Abs(Vector3.Distance (this.transform.position, Target.transform.position));

			//set velocity
			this.GetComponent<Rigidbody> ().velocity = (heading * MoveSpeed);

			yield return new WaitForFixedUpdate ();

		}

		//make sure this only happens when the soldier is alive
		if (HitPoints > 0) {

			//Add an enemy to the gate
			AtTarget = true;

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
		A_Source.clip = DeathSounds[Random.Range(0, DeathSounds.Length)];
		A_Source.Play ();

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
		dH.TakeDamageGate (damage);
	}


}
