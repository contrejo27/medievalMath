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
	
	// Update is called once per frame
	void Update () {
		if (level1.interactable == true && GameObject.Find("mathController").GetComponent<MathController>().level1_Completed) 
		{
			level1.image.sprite = Complete;
			level2.interactable = true;
		}
		if (level1.interactable == true && GameObject.Find("mathController").GetComponent<MathController>().level2_Completed) 
		{
			level2.image.sprite = Complete;
			level3.interactable = true;
		}
		if (level1.interactable == true && GameObject.Find("mathController").GetComponent<MathController>().level3_Completed) 
		{
			level3.image.sprite = Complete;
			level4.interactable = true;
		}
		if (level1.interactable == true && GameObject.Find("mathController").GetComponent<MathController>().level4_Completed) 
		{
			//make level 4 completed
		}
	}

    
}
