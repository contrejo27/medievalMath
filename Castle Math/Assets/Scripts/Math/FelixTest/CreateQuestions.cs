using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuestions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		fractions = GetComponent<Fractions> ();

		fractions.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
