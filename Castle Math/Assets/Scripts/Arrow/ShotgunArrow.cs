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

		/*
		int x = Random.Range (1, 3);
		int y = Random.Range (-2, 2);
		int z = Random.Range (-2, 2);

		yield return new WaitForSeconds (.03f);
		GameObject newArrow = Instantiate (Arrow, this.transform.position, this.transform.rotation);

		newArrow.GetComponent<ProjectileBehavior> ().isGrounded = false;

		newArrow.transform.Rotate(x, y, z);

		newArrow.GetComponent<Rigidbody> ().AddForce (transform.forward * 7000);

	*/
	}


		
	

}
