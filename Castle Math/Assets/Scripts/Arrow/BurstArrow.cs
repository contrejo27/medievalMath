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
/*
		yield return new WaitForSeconds (.1f);

		GameObject newArrow = Instantiate (Arrow, this.transform.position - (transform.forward * 1.5f), this.transform.rotation);

		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;

		newArrow.GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);

		yield return new WaitForSeconds (.1f);

		newArrow = Instantiate (Arrow, this.transform.position - (transform.forward * 2.75f), this.transform.rotation);

		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;

		newArrow.GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);*/
		yield return new WaitForSeconds (.1f);

	}


		
	

}
