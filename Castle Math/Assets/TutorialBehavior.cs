using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour {

	Image tutorialImage;
	public Sprite[] tutorialImages;
	bool tutorialDone = false;
	public GameObject startGame;
	public GameObject billboard;
	public UIEffects mathCanvas;

	int currentImage = 0;

	// Use this for initialization
	void Start () {
		tutorialImage = GetComponent<Image>();
	}

	public void Next () {
		if(tutorialDone) {
			startGame.GetComponent<Button>().onClick.Invoke();
			billboard.SetActive(false);	
			return;
		}
		currentImage++;
		tutorialImage.sprite = tutorialImages[currentImage];

		if(currentImage == 1){
			mathCanvas.fadeIn(1);
		}

		if(currentImage == tutorialImages.Length-1){
			tutorialDone = true;
			startGame.SetActive(true);	
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
