using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hudManager : MonoBehaviour {

	public Text arrowText;
	public ArrowSupplier arrows;
	public GameObject leftArrow;
	public GameObject rightArrow;
	public GameObject turnAroundText;
	public Sprite[] PowerUpIcons;
	public GameObject[] powerUpDisplays;
	public GameObject mainCamera; // used to figure out what angle they're facing
		
	public int powerUpCount = 0;
	
	void Update () {
		arrowText.text = arrows.NumberOfArrows.ToString();
		
		//give direction if player is looking the wrong way
		float cameraYAngle = mainCamera.transform.eulerAngles.y;
		if(cameraYAngle < 115f || cameraYAngle > 290f){
			rightArrow.SetActive(true);		
			leftArrow.SetActive(true);
			turnAroundText.SetActive(true);
		}
		else{
			rightArrow.SetActive(false);
			leftArrow.SetActive(false);
			turnAroundText.SetActive(false);
		}
	}

	//puts new perk icon in corresponding hud powerup display
	public void AddPoweUpIcon(string perkText){
		foreach(Sprite icon in PowerUpIcons){
			if(icon.name.ToString() == perkText){
				powerUpDisplays[powerUpCount].SetActive (true);
				powerUpDisplays[powerUpCount].GetComponent<Image>().sprite = icon;
				StartCoroutine(fadeInPanelCoroutine(15f,powerUpDisplays[powerUpCount]));
				powerUpCount++;
			}
		}
	}
	
	IEnumerator RemovePowerUpDisplay(float time, GameObject powerUp){
		yield return new WaitForSeconds (time);
		print("setting " + powerUp.ToString() + " to false");
		
		powerUp.SetActive (false);	
	}
	
	IEnumerator fadeInPanelCoroutine(float time, GameObject powerUp)    {
		Image emptyPanel = powerUp.transform.GetChild(0).GetComponent<Image>();
		float transition = 0;
		while(emptyPanel.fillAmount < 1){
			transition += Time.deltaTime/time;
			emptyPanel.fillAmount = Mathf.Lerp(0,1, transition);			
			yield return null;
		}
		
		powerUp.SetActive (false);	

	}
}


