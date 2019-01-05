using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinousRotate : MonoBehaviour {

	public Vector3 RotateAxis;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (RotateAxis);

	}
}
