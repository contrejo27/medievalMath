using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour {

	public int NumberOfQuestions;
	private int CurrentNumber;
	private LaunchProjectile ProjectileLauncher;

	// Use this for initialization
	void Start () {
		ProjectileLauncher = FindObjectOfType<LaunchProjectile> ();
	}

	public void QuestionAnswered()
	{

		CurrentNumber += 1;

		if (CurrentNumber >= NumberOfQuestions) {
			CurrentNumber = 0;

			int RanMod = Random.Range (0, 4);
			ArrowModifier newMod;

			if (RanMod == 0) {
				newMod = ArrowModifier.Bomb;
			} else if (RanMod == 1) {
				newMod = ArrowModifier.Burst;
			}else if (RanMod == 2) {
				newMod = ArrowModifier.Homing;
			}else{
				newMod = ArrowModifier.Shotgun;
			}

			//give player perk
			ProjectileLauncher.AddModifier (newMod, 10);

		}

		transform.localScale = Vector3.Lerp (new Vector3 (.05f, .75f, 1), Vector3.one, (CurrentNumber * 1f) / NumberOfQuestions);


	}

}
