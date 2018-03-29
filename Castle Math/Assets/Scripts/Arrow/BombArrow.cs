using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : ArrowClass {
	public AudioClip boom;
	public GameObject explosion;
	bool activated = false;
	private AudioSource A_Source;

	public void activate(bool isActivated){
		activated = isActivated;
	}

	public override void ArrowLaunched()
	{

	}
	
	void Start(){
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
	}
	
	public override void ArrowImpact()
	{
		if(activated){
			A_Source.clip = boom;
			A_Source.Play ();
			var expl = Instantiate(explosion, transform.position, Quaternion.identity);
			//get all the colliders within a 10 radius
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 13);
			Destroy(expl, 3);
			int i = 0;
			while (i < hitColliders.Length) {
				if (hitColliders [i].gameObject.tag == "Enemy") {
//hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().bomb = true;
					hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().TakeDamage (2);
					hitColliders [i].gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
					hitColliders [i].gameObject.GetComponent<Rigidbody> ().AddExplosionForce (1000, this.transform.position, 15,5f);
					
				}
				i += 1;
			}
		}
	}


}
