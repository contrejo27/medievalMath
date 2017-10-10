using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingArrow : ArrowClass {

	private ProjectileBehavior ArrowBehavior;
	private Transform Target;

	// Use this for initialization
	void Start () {

	}

	public override void ArrowLaunched()
	{
		ArrowBehavior = GetComponent<ProjectileBehavior> ();
		Target = AcquireTarget ();
		Debug.Log (Target);

		if(Target != null)
		{
			StartCoroutine (HomingFunction ());
		}

	}

	//do a raycast forward, if it hits something, do a sphere cast and pick the first enemy in that area to target
	public Transform AcquireTarget()
	{
		RaycastHit hit;

		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			
			Collider[] hitColliders = Physics.OverlapSphere(hit.point, 15);

			int i = 0;
			while (i < hitColliders.Length)
			{
				if (hitColliders [i].gameObject.tag == "Enemy") {
					return hitColliders [i].transform;
				}
				i += 1;
			}
		}

		return null;
	}

	IEnumerator HomingFunction()
	{

		while (Vector3.Distance (transform.position, Target.position) > 1f) {
			Vector3 heading = (Target.position - transform.position ).normalized;
			Debug.Log (heading);
			GetComponent<Rigidbody> ().velocity = heading * 100;

			yield return new WaitForFixedUpdate ();
		}

	}
	

}
