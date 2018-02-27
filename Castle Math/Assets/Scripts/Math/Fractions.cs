using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fractions : MonoBehaviour, Question {
	private AnswerInput A_Input;

	public Text QuestionText;
	public Text QuestionText_hud;
	public GameObject fractionItem;

	private int Numerator;
	private int Denominator;
	private double DecimalAnswer;
	private string StringAnswer;
	private int incorrectAnswers; 
	private int reduce;
	private List<GameObject> gems = new List<GameObject>();

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

		this.reduce = Random.Range(0, 2);

		for (int i = 2; i <= Denominator; i++) {
			if (Numerator % i == 0 && Denominator % i == 0 && reduce == 1) {
				Numerator /= i;
				Denominator /= i;
			} else {
				reduce = 0;
			}
		}

		QuestionText.text = "What fraction do the green gems represent?";

		DecimalAnswer = (double)Numerator / (double)Denominator;
		StringAnswer = Numerator.ToString() + "/" + Denominator.ToString();
		GenerateChoices ();
		displayItems();
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

			/*
			while (DecimalChoice == DecimalAnswer) {
				DenominatorChoice = Random.Range (NumeratorChoice, 13);
				DecimalChoice = NumeratorChoice / DenominatorChoice;
			}
			*/

			while (NumeratorChoice == Numerator && DenominatorChoice == Denominator) {
				DenominatorChoice = Random.Range (NumeratorChoice, 13);
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

	void displayItems(){
		if (gems.Count > 0) {
			this.deleteGems ();
		}
		int numeratorGems;
		int denominatorGems;
		int increaseFractionAmt = Random.Range (1, 4);
		Debug.Log ("reduce:" + reduce);
		Debug.Log ("Num :" + Numerator);
		Debug.Log ("Denom :" + Denominator);


		if (reduce == 1) {
			numeratorGems = Numerator + increaseFractionAmt;
			denominatorGems = Denominator + increaseFractionAmt;
		}
		else {
			numeratorGems = Numerator;
			denominatorGems = Denominator;
		}

		GameObject billboard = GameObject.Find ("MathCanvas_Billboard");

		int gemCount = 0;;
		for(int i = gemCount; i<numeratorGems;i++){
			//Debug.Log ("Num Gems:" + numeratorGems);
			GameObject numeratorItem = Instantiate (fractionItem, billboard.transform);
			numeratorItem.transform.position = new Vector3 (numeratorItem.transform.position.x + i, numeratorItem.transform.position.y, numeratorItem.transform.position.z);
			gems.Add (numeratorItem);
			gemCount++;
		}

		for(int i = gemCount; i< denominatorGems;i++){
			//Debug.Log ("Denom Gems:" + denominatorGems);
			GameObject denominatorItem = Instantiate(fractionItem, billboard.transform);
			denominatorItem.GetComponent<Image>().color = Color.red;
			denominatorItem.transform.position = new Vector3(denominatorItem.transform.position.x + i, denominatorItem.transform.position.y, denominatorItem.transform.position.z);
			gems.Add (denominatorItem);
		}
		
	}

	private void deleteGems () {
		for (int i = 0; i < gems.Count; i++) {
			Destroy (gems [i]);
		}
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