using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour {

	Image tutorialImage;
	public Sprite[] tutorialImages;
	public bool tutorialDone = false;
	public GameObject startGame;
	public GameObject billboard;
	public UIEffects mathCanvas;
	public UIEffects interMathButtons;
	public UIEffects interMathCanvas;
	public GameObject target;
	int currentImage = 0;

	// Use this for initialization
	void Start () {
		tutorialImage = GetComponent<Image>();
	}

	//goes through tutorial images when next gets clicked
	public void Next () {
		currentImage++;
		//done before the others so we don't assign a new sprite

		//tutorial ends
		if(currentImage == 3) {
			tutorialDone = true;
			startGame.SetActive(false);	
			//TODO add line back in when tutorial is done
			//startGame.GetComponent<Button>().onClick.Invoke();
			return;
		}

		//load next tutorial image
		tutorialImage.sprite = tutorialImages[currentImage];

		if(currentImage == 1){
			mathCanvas.fadeIn(1);
			target.SetActive(false);
		}

		if(currentImage == 2){
			target.SetActive(true);
			startGame.SetActive(true);	
		}

	}

	public void Activate(){
		tutorialImage.enabled = false;
		target.SetActive(false);
		mathCanvas.fadeOut(1);
		interMathCanvas.fadeIn(1);
		interMathButtons.fadeIn(1);
	}
	
	public void Deactivate(){
		interMathCanvas.fadeOut(1);
		mathCanvas.fadeIn(1);
		interMathButtons.fadeOut(1);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
