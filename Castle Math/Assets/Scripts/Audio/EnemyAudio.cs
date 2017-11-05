using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {

	public AudioClip[] EnemyYells;
	public AudioClip[] EnemyTaunts;
	private AudioSource A_Source;

	// Use this for initialization
	void Start () {
		A_Source = this.GetComponent<AudioSource> ();
		StartCoroutine (SoundWait(1f));
	}

	void PlaySound()
	{
			A_Source.clip = EnemyTaunts [Random.Range (0, EnemyTaunts.Length)];
			A_Source.pitch = .75f;

			A_Source.Play ();
	}

	//delay for the length of the clip
	IEnumerator SoundWait(float delay)
	{
		float waitMultiplier = Random.Range (1.5f, 5);

		yield return new WaitForSeconds (delay * waitMultiplier);
	

		PlaySound ();
	}

	

}
