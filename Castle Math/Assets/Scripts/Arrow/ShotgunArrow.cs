using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunArrow : ArrowClass {

	private ProjectileBehavior ArrowBehavior;
	private Transform Target;

	// Use this for initialization
	void Start () {

	}

	//duplicate the current arrow twice
	public override void ArrowLaunched()
	{

		GameObject Arrow = this.gameObject;

		StartCoroutine (DelayCreate(transform.right * 1, Arrow));
		StartCoroutine (DelayCreate(transform.right * -1, Arrow));
		StartCoroutine (DelayCreate(transform.up * 1, Arrow));
		StartCoroutine (DelayCreate(transform.up * -1, Arrow));
	}

	IEnumerator DelayCreate(Vector3 Direction, GameObject Arrow)
	{

		yield return new WaitForSeconds (.03f);

		GameObject newArrow = Instantiate (Arrow, this.transform.position + Direction, this.transform.rotation);

		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;

		newArrow.GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);

	}


		
	

}
