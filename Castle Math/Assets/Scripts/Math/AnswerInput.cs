using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnswerInput : MonoBehaviour {


	private string CurrentAnswer;

	public Text AnswerText;

	private MathManager M_Manager;

	// Use this for initialization
	void Start () {
	
		CurrentAnswer = "";
		M_Manager = GameObject.FindObjectOfType<MathManager> ();
	}

	public void ClearAnswer()
	{
		CurrentAnswer = "";

		AnswerText.text = "";

	}

	public void AddTo(string Digit)
	{
		CurrentAnswer += Digit;

		AnswerText.text = CurrentAnswer;
	}


	public void BackSpace()
	{
		if (CurrentAnswer.Length > 0) {
			CurrentAnswer = CurrentAnswer.Substring (0, CurrentAnswer.Length - 1);

			AnswerText.text = CurrentAnswer;
		}

	}


	public void SubmitAnswer()
	{
		if (CurrentAnswer.Length > 0) {
			int FinalAnswer = Convert.ToInt32 (CurrentAnswer);

			M_Manager.CheckAnswer (FinalAnswer);
		}
	}
		

}
