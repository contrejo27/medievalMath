using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingUI : MonoBehaviour {

	public TutorialBehavior tutorial;

	// Use this for initialization
	public void Select () {
		print("Target hit");
		tutorial.Next();
	}

	void OnCollisionEnter(Collision otherCollision)
	{
		Select(); 
	}

}
