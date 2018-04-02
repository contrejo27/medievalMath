using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBehavior : MonoBehaviour {

	Image tutorialImage;
	public Sprite[] tutorialImages;
	public Sprite storyImage;
	public bool tutorialDone;
	public GameObject startGame;
	public GameObject mathCanvas;
	public GameObject target;
	int currentImage = 0;

	public AudioClip[] tutorialSounds;
	public AudioSource UIAudio;

	// Use this for initialization
	void Start () {
		tutorialImage = GetComponent<Image>();

		if(tutorialDone){
			//if(bossLevel){
				tutorialImage.sprite = storyImage;
				target.SetActive(false);
				startGame.SetActive(true);	
			//} 
		}else{
			StartCoroutine(ShootTheTargetSounds());
		}
	}

	//goes through tutorial images when next gets clicked
	public void Next () {
		currentImage++;
		//load next tutorial image
		tutorialImage.sprite = tutorialImages [currentImage];


		if(currentImage == 1){
			mathCanvas.SetActive(true);
			mathCanvas.GetComponent<UIEffects>().fadeIn(1);
			target.SetActive(false);
			UIAudio.clip = tutorialSounds[2];
			UIAudio.Play ();
		}

		if(currentImage == 2){
			tutorialDone = true;
			startGame.SetActive(true);	
			// UIAudio.clip = tutorialSounds[2]; // protect the castle you're our only hope
			//UIAudio.Play ();
		}
	}


	public IEnumerator ShootTheTargetSounds(){
		yield return new WaitForSeconds(3f);
		if(currentImage == 0){
			UIAudio.clip = tutorialSounds[0];
			UIAudio.Play ();
		}

		yield return new WaitForSeconds(11f);
		if(currentImage == 0){
			UIAudio.clip = tutorialSounds[1];
			UIAudio.Play ();
		}

		yield return new WaitForSeconds(9f);
		if(currentImage == 0){
			UIAudio.clip = tutorialSounds[0];
			UIAudio.Play ();
		}
	}
}
