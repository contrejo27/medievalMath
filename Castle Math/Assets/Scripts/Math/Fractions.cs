using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fractions : MonoBehaviour, Question {
	private AnswerInput A_Input;

	public Text QuestionText;
	public Text QuestionText_hud;

	private int Numerator;
	private int Denominator;
	private double DecimalAnswer;
	private string StringAnswer;
	private int incorrectAnswers; 

	public Fractions () {
	}

	// Use this for initialization
	public void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
	}
	
	// Update is called once per frame
	public void GenerateQuestion (int maxDifficulty) { //int maxDifficulty => temp fix
		Numerator = Random.Range (1, 13);
		Denominator = Random.Range (2, 13);

		while (Numerator >= Denominator) {
			Numerator = Random.Range (1, 13);
		}

		DecimalAnswer = (double)Numerator / (double)Denominator;

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
			NumeratorChoice = Random.Range (1, 12);
			DenominatorChoice = Random.Range (NumeratorChoice, 13);

			if (NumeratorChoice == DenominatorChoice) {
				DenominatorChoice++;
			}

			DecimalChoice = (double)NumeratorChoice / (double)DenominatorChoice;

			while (DecimalChoice == DecimalAnswer) {
				DenominatorChoice = Random.Range (NumeratorChoice, 13);
				DecimalChoice = NumeratorChoice / DenominatorChoice;
			}
				
			AnswerChoices [i] = NumeratorChoice.ToString () + "/" + DenominatorChoice.ToString ();
		}

		AnswerChoices [3] = this.getCorrectAnswer ();

		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		Debug.Log ("Correct Answer:\n\tFraction: " + getCorrectAnswer ());

		A_Input.DisplayChoices (AnswerChoices);

	}

	public string GetQuestionString () {
		return this.StringAnswer;
	}

	public string getCorrectAnswer() {
		return this.StringAnswer;
	}

	public void SetCorrectAnswer(string answer) {
		this.StringAnswer = answer;
	}

	public void SetQuestionString(string question) {
		this.StringAnswer = question;
	}

	private double getCorrectDecimalAnswer() {
		return this.DecimalAnswer;
	}

	private int  getNumerator() {
		return this.Numerator;
	}

	private int getDenominator() {
		return this.Denominator;
	}

	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}

}