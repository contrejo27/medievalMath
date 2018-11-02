using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarcophagusScript : EnemyBehavior {

	/*
	 * TO DO:
	 * 
	 * 
	 * 
	 * 
	 * 
	*/

	public GameObject MummyPrefab;

	public float speed;

	private Vector3 start;
	private Vector3 end;

	private void OnEnable() {
		start = transform.position;
		end = new Vector3 (start.x, start.y + 10f, start.z);

		StartCoroutine (Intro ());
	}

	protected override void OnReceiveDamage()
	{
		base.OnReceiveDamage();

		//TO DO: Shake when hit.

		if(hitPoints <= 2)
		{
			//TO DO: Rate at which Enemies spawn increases slightly

		}
	}

	IEnumerator Intro () {
		float timeToStart = Time.time;
		while (Vector3.Distance (transform.position, end) > 0.05f) {
			transform.position = Vector3.Lerp (transform.position, end, (Time.time - timeToStart) * .05f);

			yield return null;
		}

		yield return new WaitForSeconds (3f);
	}

	IEnumerator SpawnMummies () {
		yield return new WaitForSeconds (69f);
	}


}
