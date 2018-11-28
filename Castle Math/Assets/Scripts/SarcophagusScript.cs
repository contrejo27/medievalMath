using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarcophagusScript : MonoBehaviour {

	/*
	 * TO DO:
	 * 
	 * 
	 * 
	 * 
	 * 
	*/

	private WaveManager wManager;

	public GameObject MummyPrefab;

	Queue<int> hitQueue = new Queue<int>();
	public int hitPoints;
	protected bool dead = false;
	protected bool ignoreDamage = false;

	public float speed;

	private Vector3 start;
	private Vector3 end;

	public Transform currentTarget;

	//public int target;
	//public Transform[] fenceTargets;

	void Start() {
		wManager = GameObject.FindObjectOfType<WaveManager> ();
	
		start = transform.position;
		end = new Vector3 (start.x, start.y + 10f, start.z);

		StartCoroutine (Intro ());
	}

	void Update() {
		if (hitQueue.Count > 0)
		{
			ReceiveDamage(hitQueue.Dequeue());
		}
	}

	public virtual void TakeDamage(int DMG)
	{
		if(!ignoreDamage)
			hitQueue.Enqueue(DMG);

	}

	private void ReceiveDamage(int dmg)
	{
		hitPoints -= dmg;
		if (hitPoints <= 0)
		{
			if (!dead)
			{
				Killed();
				dead = true;
			}
		}
	}
		

	public void SetTarget(Transform initialTarget){
		currentTarget = initialTarget;
	}

	public void SpawnMummies() {

		//Random target purely for testing purposes, will change once spawn points are set

		GameObject enemyObject = Instantiate(MummyPrefab, transform.position, transform.rotation);
		enemyObject.GetComponent<EnemyBehavior> ().SetTarget (currentTarget);
		wManager.addEnemyToWaveSize();

		Debug.Log ("Mummy Time!");
	}

	public void Killed()
	{

		Collider enemyHitbox = this.GetComponent<Collider>();
		Destroy(enemyHitbox);

		wManager.EnemyKilled ();
		StartCoroutine (WaitToDestroy());
	}

	IEnumerator WaitToDestroy()
	{
		yield return new WaitForSeconds (2);
		Destroy (this.gameObject);
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
