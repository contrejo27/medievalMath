using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrankRotation : MonoBehaviour {

	public bool isRotating = false;
	public int NumberofSoldiers = 0;

	private float rotSpeed = .1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (isRotating == false) {
			return;
		}

		this.transform.rotation = Quaternion.Euler (new Vector3 (0,this.transform.rotation.eulerAngles.y + (rotSpeed * NumberofSoldiers),0));
		
	}
}
