using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstArrow : ArrowClass {

	private ProjectileBehavior ArrowBehavior;
	private Transform Target;

	// Use this for initialization
	void Start () {

	}

	//duplicate the current arrow twice
	public override void ArrowLaunched()
	{

		GameObject Arrow = this.gameObject;
		StartCoroutine (DelayCreate (Arrow));

	}

	IEnumerator DelayCreate(GameObject Arrow)
	{
		GameObject[] burstArrows = new GameObject[3];
		yield return new WaitForSeconds (.05f);
		
		for (int i = 0; i < 3; i++) {
			burstArrows[i] = Instantiate (Arrow, this.transform.position - (transform.forward * (i + 1.5f)), this.transform.rotation);
			burstArrows[i].GetComponent<ProjectileBehavior> ().isGrounded = false;
			burstArrows[i].GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);
			burstArrows[i].GetComponent<BoxCollider> ().enabled = true; 
			yield return new WaitForSeconds (.05f);
		}
	}

		
	

}
