using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explodingBarrel : MonoBehaviour {
	public AudioClip boom;
	public GameObject explosion;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnCollision () {
		this.transform.GetChild (0).gameObject.SetActive (true);
		//get all the colliders within a 10 radius
		Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 13);
		
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
