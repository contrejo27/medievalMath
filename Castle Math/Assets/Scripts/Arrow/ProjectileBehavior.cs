using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {


	public int ArrowDamge { get; set;}
	public bool isGrounded { get; set;}

	// Use this for initialization
	void Start () {
		ArrowDamge = 1;
	}

	void Update(){
		if (isGrounded == true) {
			return;
		}

		//orient the arrow in the direction of motion
		transform.rotation = Quaternion.LookRotation (this.GetComponent<Rigidbody>().velocity);

	}
	
	//this is a built in function that detects when two objects collide
	void OnCollisionEnter(Collision otherCollision)
	{
		if (isGrounded == false) {

				isGrounded = true;

				this.gameObject.GetComponent<BoxCollider> ().enabled = false;
				this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
				
				//an arrow can have multiple arrow class components
				ArrowClass[] ArrowModifers = this.GetComponents<ArrowClass> ();
				for (int i = 0; i < ArrowModifers.Length; i++) {
				ArrowModifers [i].ArrowImpact ();
				}

				if (otherCollision.transform.tag == "Enemy") {
				otherCollision.gameObject.GetComponent<EnemyBehavior> ().TakeDamage (ArrowDamge);

					this.transform.parent = otherCollision.transform;

				} 
				
			StartCoroutine (WaitToDestroy ());
		} 
			
	}


	IEnumerator WaitToDestroy()
	{
		yield return new WaitForSeconds (5);
		Destroy (this.gameObject);
	}


}
