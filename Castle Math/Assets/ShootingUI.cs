using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingUI : MonoBehaviour {

	public TutorialBehavior tutorial;

	// Use this for initialization
	public void Select () {
		tutorial.Next();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision otherCollision)
	{
		Select(); 
	}

}
