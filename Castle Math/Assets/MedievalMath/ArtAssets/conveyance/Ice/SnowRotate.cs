using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowRotate : MonoBehaviour {

	public float spin;

	void Update ()
	{
		transform.Rotate (new Vector3 (0, spin, 0) * Time.deltaTime);		
	}
}

