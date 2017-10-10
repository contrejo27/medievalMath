using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMathStats : MonoBehaviour {


	private int CorrectAnswers;
	private int IncorrectAnswers;

	public Text PercentageText;
	public Text CorrectText;
	public Text IncorrectText;

	// Use this for initialization
	void Start () {
		
	}

	public void CorrectlyAnswered()
	{
		CorrectAnswers += 1;

		CorrectText.text = "Correct: " + CorrectAnswers.ToString ();

		PercentageText.text = "Accuracy: " + ((int)(((CorrectAnswers* 1f) / (CorrectAnswers + IncorrectAnswers)) * 100)).ToString ();

	}



	public void IncorrectlyAnswered()
	{

		IncorrectAnswers += 1;


		IncorrectText.text = "Incorrect: " + IncorrectAnswers.ToString ();

		PercentageText.text = "Accuracy: " + ((int)(((CorrectAnswers* 1f) / (CorrectAnswers + IncorrectAnswers)) * 100)).ToString ();

	}
	

}
