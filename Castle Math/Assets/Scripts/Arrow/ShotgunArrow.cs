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
		/*
		StartCoroutine (DelayCreate(transform.position, Arrow));
		StartCoroutine (DelayCreate(transform.position, Arrow));
		StartCoroutine (DelayCreate(transform.position, Arrow));
		StartCoroutine (DelayCreate(transform.position, Arrow));
		*/

		DelayCreate(transform.right * 1, Arrow);
		//StartCoroutine (DelayCreate(transform.right * -1, Arrow));
		//StartCoroutine (DelayCreate(transform.up * 1, Arrow));
		//StartCoroutine (DelayCreate(transform.up * -1, Arrow));

	}

	void DelayCreate(Vector3 Direction, GameObject Arrow)
	{
		
		GameObject newArrow = Instantiate (Arrow, this.transform.position, this.transform.rotation);
		newArrow.transform.Rotate(Random.Range(-30.0f, 30.0f), 180f, Random.Range(-30.0f, 30.0f));
		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;
		newArrow.GetComponent<Rigidbody> ().AddForce (newArrow.transform.forward * 7000);
	}
}
