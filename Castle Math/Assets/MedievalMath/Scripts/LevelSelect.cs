using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public Button level1;
	public Button level2;
	public Button level3;
	public Button level4;

	public Sprite Complete;

	//private MathController mymath;

	// Use this for initialization
	void Start () {
		
		level1.interactable = true;
		level2.interactable = false;
		level3.interactable = false;
		level4.interactable = false;
	}
	


    
}
