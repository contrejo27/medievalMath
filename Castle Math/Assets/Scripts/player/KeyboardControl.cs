using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//updates the rotation of the camera. 
		float CurrentY = this.transform.rotation.eulerAngles.y + Input.GetAxis ("Horizontal");

		float CurrentX = this.transform.rotation.eulerAngles.x - Input.GetAxis ("Vertical");

		this.transform.rotation = Quaternion.Euler (new Vector3(CurrentX, CurrentY, 0));


	}
}
