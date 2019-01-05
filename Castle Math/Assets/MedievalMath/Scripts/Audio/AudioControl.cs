using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour {

	private AudioSource A_Source;

	// Use this for initialization
	void Start () {
		A_Source = this.GetComponent<AudioSource> ();
	}

	public void PlayClip(AudioClip clipToPlay)
	{
		A_Source.clip = clipToPlay;
		A_Source.Play ();

	}

}
