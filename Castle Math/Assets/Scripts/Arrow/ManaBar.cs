using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {
	public Text perk;
	public int NumberOfQuestions;
	public GameObject manaBarEnd;
	public CanvasGroup mathCanvas;
	public Sprite[] PowerUpIcons;

	private int CurrentNumber;
	private LaunchProjectile ProjectileLauncher;

	public GameObject[] PowerUpDisplays;
	private int PowerUpCount;

	private AudioSource A_Source;
	public AudioClip PowerUpSound;

	// Use this for initialization
	void Start () {
		A_Source = GameObject.Find ("UIAudio").GetComponent<AudioSource> ();
		ProjectileLauncher = FindObjectOfType<LaunchProjectile> ();
	}

	public void ClearPowerUp(int PowerupIndex)
	{
		PowerUpDisplays [PowerupIndex].SetActive (false);
	}

	public void QuestionAnswered()
	{
		CurrentNumber += 1;

		if (CurrentNumber >= NumberOfQuestions) {
			CurrentNumber = 0;

			int RanMod = Random.Range (0, 3);
			ArrowModifier newMod;
			if (RanMod == 0) {
				newMod = ArrowModifier.Shotgun;
				perk.text = "Bomb";
			} else if (RanMod == 1) {
				newMod = ArrowModifier.Shotgun;
				perk.text = "Burst";
			} else {
				newMod = ArrowModifier.Shotgun;
				perk.text = "Spread";
			} 
			mathCanvas.alpha = 0.0f;

			PowerUpDisplays [PowerUpCount].SetActive (true);
			PowerUpDisplays [PowerUpCount].GetComponent<Image>().sprite = PowerUpIcons [RanMod];
			
			//making ui show on top of everything else
			RectTransform theRectTransform;
			theRectTransform = PowerUpDisplays[PowerUpCount].transform as RectTransform; // Cast it to RectTransform
			theRectTransform.SetAsLastSibling(); // Make the panel show on top.
			
			
			//give player perk
			ProjectileLauncher.AddModifier (newMod, PowerUpCount);
			StartCoroutine(erasePerkText());
			A_Source.clip = PowerUpSound;
			A_Source.Play ();

			if (PowerUpCount < PowerUpDisplays.Length - 1) {
				PowerUpCount += 1;
			} else {
				PowerUpCount = 0;
			}

		}

		float percent = (CurrentNumber * 1f) / NumberOfQuestions;
		transform.localScale = Vector3.Lerp (new Vector3 (.05f, .5f, 1f), new Vector3 (.8f, .5f, 1f), percent);
		//manaBarEnd.transform.position = ;
	}
	IEnumerator erasePerkText()
	{
		yield return new WaitForSeconds (3f);
		mathCanvas.alpha = 1.0f;
		perk.text = "";

	}
}
