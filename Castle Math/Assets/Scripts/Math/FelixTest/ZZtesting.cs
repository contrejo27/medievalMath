using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZZtesting : MonoBehaviour {
	Text qstring; 
	Rational testNumber;
	Tester testfrac;
	int i = 0;
	// Use this for initialization
	void Start () {
		qstring = GameObject.FindGameObjectsWithTag ("Question")[1].GetComponent<Text>();
		testNumber = new Rational (5, 1);
		Cursor.lockState = CursorLockMode.Confined;
		Debug.Log ("Locked cursor");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Tester.RandomizeTestVar ();
			Debug.Log ("test num" + testNumber.num + " den: " + testNumber.den);
		}

	}
}
