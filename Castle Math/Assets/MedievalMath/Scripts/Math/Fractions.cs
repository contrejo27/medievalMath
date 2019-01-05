using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fractions : MonoBehaviour, Question {
	private AnswerInput A_Input;
    int maxInt = 10;

    public Text QuestionText;
	public Text QuestionText_hud;
	public GameObject fractionItem;
	private Rational fraction;
    Rational reducedFraction;
    private double DecimalAnswer;
	private string StringAnswer;
	private int incorrectAnswers; 
	private int reduce;
	private List<GameObject> gems = new List<GameObject>();
    int fractionMultiplier;
    string questionString;
    string[] answerChoices;

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
	public void GenerateIntermathQuestion (int maxDifficulty) { //int maxDifficulty => temp fix
		fraction = new Rational (Random.Range (1, 13), Random.Range (2, 13));

		while (fraction.num >= fraction.den) {
			fraction.num = Random.Range (1, 13);
		}

		this.reduce = Random.Range(0, 2);

		//If fraction can be reduced by half
		for (int i = 2; i <= fraction.den; i++) {
			if (fraction.num % i == 0 && fraction.den % i == 0 && reduce == 1) {
				fraction.num /= i;
				fraction.den /= i;
			} else {
				reduce = 0;
			}
		}

		QuestionText.text = "What fraction do the red gems represent?";

		DecimalAnswer = (double)fraction.num / (double)fraction.den;
		StringAnswer = fraction.num.ToString() + "/" + fraction.den.ToString();
        GenerateIntermathChoices();
		DisplayItems();
	}
	/// <summary>
	/// Method to generate choices for corresponding fraction classes
	/// </summary>
	public void GenerateIntermathChoices() {
		string [] AnswerChoices = new string [4];
		int NumChoice;
		int DenChoice;
		double DecimalChoice;

		//generate random choices
		for (int i = 0; i < 3; i++) {
			NumChoice = Random.Range (1, 12);
			DenChoice = Random.Range (NumChoice, 13);

			if (NumChoice == DenChoice) {
				DenChoice++;
			}

			DecimalChoice = (double)NumChoice / (double)DenChoice;

			while (NumChoice == fraction.num && DenChoice == fraction.den) {
				DenChoice = Random.Range (NumChoice, 13);
			}

			AnswerChoices [i] = NumChoice.ToString () + "/" + DenChoice.ToString ();
		}

		AnswerChoices [3] = this.GetCorrectAnswer ();

		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		Debug.Log ("Correct Answer:\n\tFraction: " + GetCorrectAnswer ());

		//Display choices using AnswerInput functionality
		A_Input.DisplayChoices (AnswerChoices);

	}

	public void GenerateOperands(int maxDifficulty)
	{
		if (maxDifficulty != -1)
		{
			maxInt = maxDifficulty;
		}

		fractionMultiplier = Random.Range(2, 4);
		reducedFraction = new Rational (Random.Range (1, maxInt), Random.Range (1, maxInt));
		StringAnswer = reducedFraction.num + "/" + reducedFraction.den;

		fraction.num = reducedFraction.num * fractionMultiplier;
		fraction.den = reducedFraction.den * fractionMultiplier;
	}

	public void GenerateQuestion(int maxDifficulty)
	{
		GenerateOperands(maxDifficulty);

		questionString = "Reduce: " + fraction.num.ToString() + "/" + fraction.den.ToString();

		//Set textbox display to formatted question string
		//QuestionText.text = QuestionString;
		Debug.Log("Setting question to: " + questionString);
		A_Input.SetQuestion(questionString);


		//Generate choices for possible answers
		GenerateChoices();
	}


	/// <summary>
	/// Generate choices based on question and calculated correct answer created in GenerateQuestion
	/// </summary>
	public void GenerateChoices()
	{

		string Choice1;
		string Choice2;
		string Choice3;

		Rational fakeFraction = new Rational (fraction.num - Random.Range (0, fraction.num - 2), fraction.den - Random.Range (0, fraction.den - 2));
		Choice1 = (fakeFraction.num + "/" + fakeFraction.den).ToString();

		fakeFraction.den = fraction.den - Random.Range(0, fraction.den-2);
		Choice2 = (reducedFraction.num + "/" + fakeFraction.den).ToString();

		fakeFraction.num = fraction.num - Random.Range(0, fraction.num-2);
		Choice3 = (fakeFraction.num + "/" + reducedFraction.den).ToString();


		string[] IntegerChoices = new string[] { Choice1, Choice2, Choice3, StringAnswer };

		//Shuffle array randomly
		for (int i = 0; i < IntegerChoices.Length; i++)
		{
			string temp = IntegerChoices[i];
			int r = Random.Range(i, IntegerChoices.Length);
			IntegerChoices[i] = IntegerChoices[r];
			IntegerChoices[r] = temp;

		}
		//Populate choice array with generated answer choices, converted to strings for later use
		answerChoices = new string[] {IntegerChoices[0].ToString(), IntegerChoices[1].ToString(),
			IntegerChoices[2].ToString(), IntegerChoices[3].ToString()};

		A_Input.DisplayChoices(answerChoices);
	}

    /// <summary>
    /// Displays gem item graphics
    /// </summary>
    void DisplayItems(){
		if (gems.Count > 0) {
			this.DeleteGems ();
		}

		int numeratorGems;
		int denominatorGems;
		int increaseFractionAmt = Random.Range (1, 3);
		Debug.Log ("reduce:" + reduce);
		Debug.Log ("Num :" + fraction.num);
		Debug.Log ("Denom :" + fraction.den);

		//Account for if they are required to reduce the fraction
		if (reduce == 1) {
			numeratorGems = fraction.num * increaseFractionAmt;
			denominatorGems = fraction.den * increaseFractionAmt;
		}
		else {
			numeratorGems = fraction.num;
			denominatorGems = fraction.den;
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
	private void DeleteGems () {
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
	public string GetCorrectAnswer() {
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
	private double GetCorrectDecimalAnswer() {
		return this.DecimalAnswer;
	}

	/// <summary>
	/// Gets the numerator.
	/// </summary>
	/// <returns>The numerator.</returns>
	/*
	 * Removed blocks below because RationalNumbers.cs already contains get/set for n/d
	private int  Getfraction.num() {
		return this.fraction.num;
	}

	private int Getfraction.den() {
		return this.fraction.den;
	}
	*/
	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}

	public string GetQuestionCategory() {
		return "Fraction";
	}

    public string GetQuestionSubCategory()
    {
        return "temp";
    }

    public bool GetAnsweredCorrectly()
    {
        return incorrectAnswers == 0;
    }

    public void OnEndQuestion()
    {

    }

}