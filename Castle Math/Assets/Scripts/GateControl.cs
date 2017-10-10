using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateControl : MonoBehaviour {

	public AudioClip BridgeHalf;
	public AudioClip BridgeLow;

	public AudioClip LoweringSound;

	private bool Half = false;
	private bool Low = false;

	private bool GateLowering = false;

	private bool AtBottom = false;

	private AudioSource A_Source;

	public int NumberOfEnemies;

	public float LowerRate = .005f;

	private gameStateManager GameManager;

	// Use this for initialization
	void Start () {
		GameManager = GameObject.FindObjectOfType<gameStateManager> ();

		A_Source = GameObject.Find ("BridgeAudio").GetComponent<AudioSource> ();


		StartCoroutine (GateLowerSound ());
	}

	void Update()
	{
		if (AtBottom == true) {
			return;
		}

		if (NumberOfEnemies > 0) {
			GateLowering = true;
		} else {
			GateLowering = false;
		}

		//have the gate raise slowly if no enemies are at the cranks
		if (NumberOfEnemies <= 0 && this.transform.rotation.eulerAngles.x > 0) {

			this.transform.rotation = Quaternion.Euler (new Vector3(this.transform.rotation.eulerAngles.x - LowerRate,this.transform.rotation.eulerAngles.y,this.transform.rotation.eulerAngles.z));
		}
		//have the enemies lower the gate, the more enemies the faster it goes
		else if (this.transform.rotation.eulerAngles.x < 90){
			this.transform.rotation = Quaternion.Euler (new Vector3(this.transform.rotation.eulerAngles.x + (LowerRate * NumberOfEnemies),this.transform.rotation.eulerAngles.y,this.transform.rotation.eulerAngles.z));


			if (this.transform.rotation.eulerAngles.x > 89f) {
				GameManager.LoseState ();
				AtBottom = true;

			}
		}

		//Audio play
		if (this.transform.rotation.eulerAngles.x > 45 && Half == false) {
			Half = true;

			A_Source.clip = BridgeHalf;
			A_Source.Play ();
		}else if (this.transform.rotation.eulerAngles.x < 45 && Half == true)
		{
			Half = false;
		}


		if(this.transform.rotation.eulerAngles.x > 80 && Low == false)
		{
			Low = true;

			A_Source.clip = BridgeLow;
			A_Source.Play ();
		}else if (this.transform.rotation.eulerAngles.x < 80 && Low == true)
		{
			Low = false;
		}

	}


	IEnumerator GateLowerSound()
	{
		float delaylength = LoweringSound.length;  
		float t = 0;


		if (GateLowering == true) {
			A_Source.clip = LoweringSound;
			A_Source.volume = .2f;

			A_Source.Play ();
		}
			
		while (GateLowering == true && t<delaylength && AtBottom == false) {
			t += Time.deltaTime;

			yield return new WaitForFixedUpdate ();

		}

		if (GateLowering == false || AtBottom == true) {
			A_Source.Stop ();
		}

		yield return new WaitForFixedUpdate ();

		if (AtBottom == false) {
			StartCoroutine (GateLowerSound ());
		}

	}

	public void AddEnemy()
	{
		NumberOfEnemies += 1;
	}

	public void RemoveEnemy()
	{

		if (NumberOfEnemies > 0) {
			NumberOfEnemies -= 1;
		}
	}

}
