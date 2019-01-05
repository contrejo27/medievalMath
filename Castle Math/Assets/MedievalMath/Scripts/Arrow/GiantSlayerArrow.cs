using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantSlayerArrow : ArrowClass {

	private ProjectileBehavior ArrowBehavior;
	private Transform Target;

	// Use this for initialization
	void Start () {

	}

	public override void ArrowLaunched()
	{
		ArrowBehavior = GetComponent<ProjectileBehavior> ();

		ArrowBehavior.ArrowDamge = 7;
	}


	

}
