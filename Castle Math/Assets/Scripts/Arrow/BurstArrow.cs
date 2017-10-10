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

		StartCoroutine (DelayCreate(1.5f, Arrow));
		StartCoroutine (DelayCreate(-1.5f, Arrow));

	}

	IEnumerator DelayCreate(float Direction, GameObject Arrow)
	{

		yield return new WaitForSeconds (.01f);

		GameObject newArrow = Instantiate (Arrow, this.transform.position + this.transform.right*Direction, this.transform.rotation);

		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;

		newArrow.GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);

		Debug.Log ("Here");


	}


		
	

}
