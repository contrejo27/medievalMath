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
	
	/// <summary>
	/// Generates the question.
	/// </summary>
	/// <param name="maxDifficulty">maximum end of range</param>
	public void GenerateQuestion (int maxDifficulty) { //int maxDifficulty => temp fix
		Numerator = Random.Range (1, 13);
		Denominator = Random.Range (2, 13);

		while (Numerator >= Denominator) {
			Numerator = Random.Range (1, 13);
		}

		this.reduce = Random.Range(0, 2);

		//If fraction can be reduced by half
		for (int i = 2; i <= Denominator; i++) {
			if (Numerator % i == 0 && Denominator % i == 0 && reduce == 1) {
				Numerator /= i;
				Denominator /= i;
			} else {
				reduce = 0;
			}
		}

		QuestionText.text = "What fraction do the red gems represent?";

		DecimalAnswer = (double)Numerator / (double)Denominator;
		StringAnswer = Numerator.ToString() + "/" + Denominator.ToString();
		GenerateChoices ();
		displayItems();
	}
	/// <summary>
	/// Method to generate choices for corresponding fraction classes
	/// </summary>
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

		//Debug.Log ("Correct Answer:\n\tFraction: " + getCorrectAnswer ());

		//Display choices using AnswerInput functionality
		A_Input.DisplayChoices (AnswerChoices);

	}
	/// <summary>
	/// Displays gem item graphics
	/// </summary>
	void displayItems(){
		if (gems.Count > 0) {
			this.deleteGems ();
		}

		int numeratorGems;
		int denominatorGems;
		int increaseFractionAmt = Random.Range (1, 4);
		//Debug.Log ("reduce:" + reduce);
		//Debug.Log ("Num :" + Numerator);
		//Debug.Log ("Denom :" + Denominator);

		//Account for if they are required to reduce the fraction
		if (reduce == 1) {
			numeratorGems = Numerator + increaseFractionAmt;
			denominatorGems = Denominator + increaseFractionAmt;
		}
		else {
			numeratorGems = Numerator;
			denominatorGems = Denominator;
		}

		//Find the InterMath billboard in the scene
		GameObject billboard = GameObject.Find ("MathCanvas_Billboard");

		int gemCount = 0;;
		int yPosIncrease = -1; //use for moving gems down to form grid pattern
		int xPosIncrease = 0; //use for moving gems across to form grid

		//Start creating gems to represent denominator to populate bottom of grid
		for(int i = gemCount; i < denominatorGems - numeratorGems; i++){
			
			if (i % 3 == 0) {
				yPosIncrease++;
				xPosIncrease = -1;
			}
			xPosIncrease++;
			//Debug.Log ("Denom Gems:" + denominatorGems);
			GameObject denominatorItem = Instantiate(fractionItem, billboard.transform);
			denominatorItem.transform.position = new Vector3(denominatorItem.transform.position.x + xPosIncrease, denominatorItem.transform.position.y - yPosIncrease, denominatorItem.transform.position.z);
			gems.Add (denominatorItem);
			gemCount++;
		}

		//Finish creating total number of gems to represent numerator portion
		for(int i = gemCount; i < denominatorGems;i++){
			//Debug.Log ("Num Gems:" + numeratorGems);

			if (i > 0 && i % 3 == 0) {
				yPosIncrease++;
				xPosIncrease = -1;
			}
			xPosIncrease++;
			GameObject numeratorItem = Instantiate (fractionItem, billboard.transform);
			numeratorItem.GetComponent<Image>().color = Color.red;

			numeratorItem.transform.position = new Vector3 (numeratorItem.transform.position.x + xPosIncrease, numeratorItem.transform.position.y - yPosIncrease, numeratorItem.transform.position.z);
			gems.Add (numeratorItem);

		}

		
	}

	/// <summary>
	/// Removes gems from scene
	/// </summary>
	private void deleteGems () {
		for (int i = 0; i < gems.Count; i++) {
			Destroy (gems [i]);
		}
	}

	/// <summary>
	/// Gets the question string.
	/// </summary>
	/// <returns>The question string.</returns>
	public string GetQuestionString () {
		return this.StringAnswer;
	}

	/// <summary>
	/// Gets the correct answer.
	/// </summary>
	/// <returns>The correct answer.</returns>
	public string getCorrectAnswer() {
		return this.StringAnswer;
	}

	/// <summary>
	/// Sets the correct answer.
	/// </summary>
	/// <param name="answer">Answer.</param>
	public void SetCorrectAnswer(string answer) {
		this.StringAnswer = answer;
	}

	/// <summary>
	/// Sets the question string.
	/// </summary>
	/// <param name="question">Question.</param>
	public void SetQuestionString(string question) {
		this.StringAnswer = question;
	}

	/// <summary>
	/// Gets the correct answer in decimal form.
	/// </summary>
	/// <returns>The correct decimal answer.</returns>
	private double getCorrectDecimalAnswer() {
		return this.DecimalAnswer;
	}

	/// <summary>
	/// Gets the numerator.
	/// </summary>
	/// <returns>The numerator.</returns>
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