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

	public GameObject[] powerUpDisplays;
	private int powerUpCount = 0;

	private AudioSource A_Source;
	public AudioClip PowerUpSound;

	// Use this for initialization
	void Start () {
		A_Source = GameObject.Find ("UIAudio").GetComponent<AudioSource> ();
		ProjectileLauncher = FindObjectOfType<LaunchProjectile> ();
	}

	public void ClearPowerUp(int PowerupIndex)
	{
		powerUpDisplays [PowerupIndex].SetActive (false);
	}

	public void QuestionAnswered()
	{
		CurrentNumber += 1;

		if (CurrentNumber >= NumberOfQuestions) {
			CurrentNumber = 0;

			int RanMod = Random.Range (0, 4);
			ArrowModifier newMod;
			if (RanMod == 0) {
				newMod = ArrowModifier.Burst;
				perk.text = "Burst";
			} else if (RanMod == 1) {
				newMod = ArrowModifier.Spread;
				perk.text = "Spread";
			} else if (RanMod == 2) {
				newMod = ArrowModifier.Bomb;
				perk.text = "Bomb";
			} else if (RanMod == 3) {
				newMod = ArrowModifier.Health;
				perk.text = "Health";
			} else {
				newMod = ArrowModifier.Invincible;
				perk.text = "Invincible";
			} 
			mathCanvas.alpha = 0.0f;

			powerUpDisplays [powerUpCount].SetActive (true);
			powerUpDisplays [powerUpCount].GetComponent<Image>().sprite = PowerUpIcons [RanMod];
			
			//give player perk
			ProjectileLauncher.AddModifier (newMod, powerUpCount);
			StartCoroutine(erasePerkText());
			A_Source.clip = PowerUpSound;
			A_Source.Play ();

			if (powerUpCount < powerUpDisplays.Length - 1) {
				powerUpCount += 1;
			} else {
				powerUpCount = 0;
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
