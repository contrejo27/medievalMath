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

	public int target;
	public Transform[] fenceTargets;

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

	public void SpawnMummies() {

		//Random target purely for testing purposes, will change once spawn points are set
		target = 1;

		GameObject enemyObject = Instantiate(MummyPrefab, transform.position, transform.rotation);
		enemyObject.GetComponent<EnemyBehavior> ().SetTarget (fenceTargets[target]);

		Debug.Log ("Mummy Time!");
	}

	IEnumerator Intro () {
		float timeToStart = Time.time;
		while (Vector3.Distance (transform.position, end) > 0.05f) {
			transform.position = Vector3.Lerp (transform.position, end, (Time.time - timeToStart) * .05f);

			yield return null;
		}

		yield return new WaitForSeconds (.1f);

		InvokeRepeating ("SpawnMummies", 4.0f, 4.0f);
	}
}
