using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArrowModifier{Burst, Bomb, Invincible, Spread, Health, Shock, Fire, Ice}

public class ArrowClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	//Called when an arrow gets launched
	public virtual void ArrowLaunched()
	{

	}


	//Called when an arrow Hits an enemy something
	public virtual void ArrowImpact()
	{

	}
}
