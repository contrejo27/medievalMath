using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {


	public bool isGrounded { get; set;}
	public bool isBomb;

	// Use this for initialization
	void Start () {
		
	}

	void Update(){
		if (isGrounded == true) {
			return;
		}

		transform.rotation = Quaternion.LookRotation (this.GetComponent<Rigidbody>().velocity);

	}
	
	//this is a built in function that detects when two objects collide
	void OnCollisionEnter(Collision otherCollision)
	{
		if (isGrounded == false) {

			if (isBomb == false) {

				isGrounded = true;

				this.gameObject.GetComponent<BoxCollider> ().enabled = false;
				this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;


				if (otherCollision.transform.tag == "Enemy") {
					otherCollision.gameObject.GetComponent<EnemyBehavior> ().TakeDamage ();

					this.transform.parent = otherCollision.transform;

				} else {

					StartCoroutine (WaitToDestroy ());
				}
			} else {

				this.transform.GetChild (0).gameObject.SetActive (true);
				//get all the colliders within a 10 radius
				Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 15);

				int i = 0;
				while (i < hitColliders.Length)
				{
					if (hitColliders [i].gameObject.tag == "Enemy") {
						hitColliders [i].gameObject.GetComponent<Rigidbody> ().AddExplosionForce (1000, this.transform.position, 15);
						hitColliders [i].gameObject.GetComponent<EnemyBehavior> ().TakeDamage ();

					}
					i += 1;
				}

				StartCoroutine (WaitToDestroy ());

			}
		}
	}

	IEnumerator WaitToDestroy()
	{
		yield return new WaitForSeconds (5);
		Destroy (this.gameObject);


	}


}
