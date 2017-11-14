using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : ArrowClass {
	private AudioSource playerAudio;
	public AudioClip boom;
	
	// Use this for initialization
	void Start () {
		playerAudio = GameObject.Find ("UIAudio").GetComponent<AudioSource> ();

	}


	public override void ArrowLaunched()
	{

	}


	public override void ArrowImpact()
	{
			this.transform.GetChild (0).gameObject.SetActive (true);
			//get all the colliders within a 10 radius
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 8);
			playerAudio.clip = boom;
			playerAudio.Play ();
			
			int i = 0;
			while (i < hitColliders.Length) {
				if (hitColliders [i].gameObject.tag == "Enemy") {
					//hitColliders [i].gameObject.GetComponent<Rigidbody> ().AddExplosionForce (1000, this.transform.position, 15);
					hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().TakeDamage (2);

				}
				i += 1;
			}

		
	}


}
