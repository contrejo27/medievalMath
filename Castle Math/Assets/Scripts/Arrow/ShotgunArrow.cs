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

		DelayCreate(Arrow);
		//StartCoroutine (DelayCreate(transform.right * -1, Arrow));
		//StartCoroutine (DelayCreate(transform.up * 1, Arrow));
		//StartCoroutine (DelayCreate(transform.up * -1, Arrow));

	}

	void DelayCreate(GameObject Arrow)
	{
		GameObject[] spreadArrows = new GameObject[4];
		for (int i = 0; i < 4; i++) {
			spreadArrows[i] = Instantiate (Arrow, this.transform.position , this.transform.rotation);
			spreadArrows[i].transform.Rotate(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
			spreadArrows[i].GetComponent<ProjectileBehavior> ().isGrounded = false;
			spreadArrows[i].GetComponent<Rigidbody> ().AddForce (spreadArrows[i].transform.forward * 7000);
			StartCoroutine (delayedCollider(spreadArrows[i]));
		}
	}
	IEnumerator delayedCollider(GameObject arrow){
		yield return new WaitForSeconds (.1f);
		arrow.GetComponent<BoxCollider> ().enabled = true; 

	}
}
