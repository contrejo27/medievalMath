using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {


	private LineRenderer L_Render;

	public GameObject otherPoint;

	// Use this for initialization
	void Start () {

		L_Render = this.GetComponent<LineRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {

		L_Render.SetPosition (0, this.transform.position);
		L_Render.SetPosition (1, otherPoint.transform.position);

	}
}
