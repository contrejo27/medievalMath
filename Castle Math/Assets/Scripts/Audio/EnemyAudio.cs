using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {

	public AudioClip[] EnemyYells;
	public AudioClip[] EnemyTaunts;


	private GateControl G_Control;
	private AudioSource A_Source;

	// Use this for initialization
	void Start () {
		G_Control = GameObject.FindObjectOfType<GateControl> ();

		A_Source = this.GetComponent<AudioSource> ();

		StartCoroutine (SoundWait(1f));
	}

	void PlaySound()
	{

		if (G_Control.NumberOfEnemies == 0) {
			A_Source.clip = EnemyYells [Random.Range (0, EnemyYells.Length)];
			A_Source.pitch = .75f;
			A_Source.Play ();

		} else {
			A_Source.clip = EnemyTaunts [Random.Range (0, EnemyTaunts.Length)];
			A_Source.pitch = .75f;

			A_Source.Play ();

		}

		StartCoroutine (SoundWait (A_Source.clip.length));


	}

	//delay for the length of the clip
	IEnumerator SoundWait(float delay)
	{
		float waitMultiplier = Random.Range (1.5f, 5);

		yield return new WaitForSeconds (delay * waitMultiplier);
	

		PlaySound ();
	}

	

}
