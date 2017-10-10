using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {


		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
