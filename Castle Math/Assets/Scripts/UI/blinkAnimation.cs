using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blinkAnimation : MonoBehaviour {
	public Image arrow;

	void Start () {
		InvokeRepeating("blink", .3f,.3f);
	}
	
	void blink () {
		arrow.enabled = !arrow.enabled;
	}
}
