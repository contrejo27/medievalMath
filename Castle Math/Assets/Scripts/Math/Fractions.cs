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
	private int Numerator;
	private int Denominator;
    int reducedNumerator;
    int reducedDenominator;
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
        GenerateIntermathChoices();
		DisplayItems();
	}
	/// <summary>
	/// Method to generate choices for corresponding fraction classes
	/// </summary>
	public void GenerateIntermathChoices() {
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
        reducedNumerator = Random.Range(1, maxInt);
        reducedDenominator = Random.Range(1, maxInt);
        StringAnswer = reducedNumerator + "/" + reducedDenominator;

        Numerator = reducedNumerator * fractionMultiplier;
        Denominator = reducedDenominator * fractionMultiplier;
    }

    public void GenerateQuestion(int maxDifficulty)
    {
        GenerateOperands(maxDifficulty);

        questionString = "Reduce: " + Numerator.ToString() + "/" + Denominator.ToString();

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


        int fakeNum = Numerator - Random.Range(0, Numerator-2);
        int fakeDenom = Denominator - Random.Range(0, Denominator-2);
        Choice1 = (fakeNum + "/" + fakeDenom).ToString();

        fakeDenom = Denominator - Random.Range(0, Denominator-2);
        Choice2 = (reducedNumerator + "/" + fakeDenom).ToString();

        fakeNum = Numerator - Random.Range(0, Numerator-2);
        Choice3 = (fakeNum + "/" + reducedDenominator).ToString();


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
		Debug.Log ("Num :" + Numerator);
		Debug.Log ("Denom :" + Denominator);

		//Account for if they are required to reduce the fraction
		if (reduce == 1) {
			numeratorGems = Numerator * increaseFractionAmt;
			denominatorGems = Denominator * increaseFractionAmt;
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
	private int  GetNumerator() {
		return this.Numerator;
	}

	private int GetDenominator() {
		return this.Denominator;
	}

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