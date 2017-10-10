using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hudManager : MonoBehaviour {

	public Text arrowText;
	public ArrowSupplier arrows;
	public GameObject leftArrow;
	public GameObject rightArrow;
	
	public GameObject mainCamera; // used to figure out what angle they're facing
	
	void Update () {
		arrowText.text = arrows.NumberOfArrows.ToString();
		
		//give direction if player is looking the wrong way
		float cameraYAngle = mainCamera.transform.eulerAngles.y;
		if(cameraYAngle < 70){
			rightArrow.SetActive(true);
			leftArrow.SetActive(false);
		}
		else if (cameraYAngle > 245f){
			rightArrow.SetActive(false);
			leftArrow.SetActive(true);
		}
		else{
			rightArrow.SetActive(false);
			leftArrow.SetActive(false);

		}
	}
}
