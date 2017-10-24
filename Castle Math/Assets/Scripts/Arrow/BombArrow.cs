using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : ArrowClass {

	// Use this for initialization
	void Start () {
		
	}


	public override void ArrowLaunched()
	{

	}


	public override void ArrowImpact()
	{
		int ranNum = Random.Range (0, 5);

		//1 in 5 chance the bomb arrow actually blows up
		if (ranNum == 0) {

			this.transform.GetChild (0).gameObject.SetActive (true);
			//get all the colliders within a 10 radius
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 15);

			int i = 0;
			while (i < hitColliders.Length) {
				if (hitColliders [i].gameObject.tag == "Enemy") {
					hitColliders [i].gameObject.GetComponent<Rigidbody> ().AddExplosionForce (1000, this.transform.position, 15);
					hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().TakeDamage (GetComponent<ProjectileBehavior> ().ArrowDamge);

				}
				i += 1;
			}

		}
	}


}
