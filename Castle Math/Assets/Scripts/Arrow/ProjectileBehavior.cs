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
	}
	
	//this is a built in function that detects when two objects collide
	void OnCollisionEnter(Collision otherCollision)
	{
        GameObject root = otherCollision.transform.root.gameObject;
        Debug.Log(root.name);
		if (isGrounded == false) {
				isGrounded = true;

				this.gameObject.GetComponent<BoxCollider> ().enabled = false;
				this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
				
				//an arrow can have multiple arrow class components
				ArrowClass[] ArrowModifers = this.GetComponents<ArrowClass> ();
				for (int i = 0; i < ArrowModifers.Length; i++) {
					ArrowModifers [i].ArrowImpact ();
				}

				if (root.tag == "Enemy") {
                    EnemyBehavior eb = root.GetComponent<EnemyBehavior>();
                    if(eb)
                        eb.TakeDamage (ArrowDamge);
                    else
                        otherCollision.gameObject.GetComponentInParent<EnemyBehavior>();
                this.transform.parent = otherCollision.transform;
				} 

				Destroy (this.gameObject,1f);
		} 	
	}
}
