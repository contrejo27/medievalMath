using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fractions : MonoBehaviour {
	private AnswerInput A_Input;

	public Text QuestionText;
	public Text QuestionText_hud;

	private int Numerator;
	private int Denominator;
	private double DecimalAnswer;
	private string StringAnswer;

	// Use this for initialization
	public void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
	}
	
	// Update is called once per frame
	public void GenerateQuestion () {
		Numerator = Random.Range (1, 13);
		Denominator = Random.Range (2, 13);

		while (Numerator >= Denominator) {
			Numerator = Random.Range (1, 13);
		}

		DecimalAnswer = Numerator / Denominator;

		StringAnswer = Numerator.ToString() + "/" + Denominator.ToString();

		GenerateChoices ();

	}

	public void GenerateChoices() {
		string [] AnswerChoices = new string [4];
		int NumeratorChoice;
		int DenominatorChoice;
		double DecimalChoice;

		//generate random choices
		for (int i = 0; i < 3; i++) {
			NumeratorChoice = Random.Range (1, 13);
			DenominatorChoice = Random.Range (2, 13);

			DecimalChoice = NumeratorChoice / DenominatorChoice;

			if (NumeratorChoice >= DenominatorChoice) {
				Debug.Log ("test");
			}

			while (NumeratorChoice >= DenominatorChoice || DecimalChoice == DecimalAnswer) {
				NumeratorChoice = Random.Range (1, 13);
				Debug.Log (NumeratorChoice + " " + DenominatorChoice);
				DecimalChoice = NumeratorChoice / DenominatorChoice;
			}
				
			AnswerChoices [i] = NumeratorChoice.ToString () + "/" + DenominatorChoice.ToString ();
		}

		AnswerChoices [3] = this.getCorrectAnswer ();

		A_Input.DisplayChoices (AnswerChoices);

	}

	public string getCorrectAnswer() {
		return this.StringAnswer;
	}

	public double getCorrectDecimalAnswer() {
		return this.DecimalAnswer;
	}

	public int  getNumerator() {
		return this.Numerator;
	}

	public int getDenominator() {
		return this.Denominator;
	}
}