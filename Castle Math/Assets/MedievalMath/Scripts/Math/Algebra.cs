using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Algebra : MonoBehaviour, Question {
	AddOrSubtract addSub;
	MultiplyOrDivide multiDiv;
	int correctAnswer;
	string [] answerChoices;
	string questionString;
	int maxInt;
	AnswerInput aInput;
	private int incorrectAnswers = 0;

	public Text questionText;
	public Text questionTextHUD;

	int randomQuestion;

	// Use this for initialization
	public void Start () {
		addSub = GameObject.FindObjectOfType<AddOrSubtract> ();
		multiDiv = GameObject.FindObjectOfType<MultiplyOrDivide> ();
		aInput = GameObject.FindObjectOfType<AnswerInput> ();
		questionText = GameObject.Find ("question").GetComponent<Text>();
	}
	
	// Update is called once per frame
	public Algebra () {
		
	}

	public void GenerateQuestion(int maxDifficulty) {
		randomQuestion = Random.Range (0, 2); //0 => Add or subtract, 2 => multiply or divide

		if (randomQuestion == 0) {
			addSub.GenerateOperands (maxDifficulty);
			string correctAnswer = addSub.GetCorrectAnswer ().ToString ();

			if (addSub.isSubtract == 0) {
				questionString = addSub.GetFirstNum ().ToString() + " - " + "X" + " =" + correctAnswer;
			} else {
				questionString = addSub.GetFirstNum ().ToString() + " + " + "X" + " =" + correctAnswer;
			}

			this.correctAnswer = addSub.GetSecondNum ();
			//Set textbox display to formatted question string
			//QuestionText.text = QuestionString;
			aInput.SetQuestion(questionString);


			//Generate choices for possible answers 
			this.GenerateChoices ();

		} else {
			multiDiv.GenerateOperands (maxDifficulty);

			while (multiDiv.GetFirstNum() == 0) {
				multiDiv.GenerateOperands (maxDifficulty);
			}

			if (multiDiv.isDivide == 0) {
				char divSign = '\u00F7';

				questionString = multiDiv.GetFirstNum ().ToString() + " " + divSign.ToString() + " " + "X" + " =" + multiDiv.GetCorrectAnswer ().ToString ();
			} else {
				questionString = multiDiv.GetFirstNum ().ToString() + " x " + "__" + " =" + multiDiv.GetCorrectAnswer ().ToString ();
			}

			this.correctAnswer = multiDiv.GetSecondNum ();
			//Set textbox display to formatted question string
			//QuestionText.text = QuestionString;
			aInput.SetQuestion(questionString);


			//Generate choices for possible answers 
			this.GenerateChoices ();
		}

	}

	public void GenerateChoices() {

		int Choice1 = correctAnswer - 1;
		int Choice2 = correctAnswer + 1;
		int Choice3 = correctAnswer + Random.Range (2, 5);

		//Array of all possible choices
		int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, correctAnswer };
		answerChoices = ChoicesToStringArray (IntegerChoices);

		aInput.DisplayChoices (answerChoices);
	}

	/// <summary>
	/// Converts the generated Choices integer array to array of strings for later use.
	/// Checks for duplicate values and shuffles array before returning
	/// </summary>
	/// <returns>The string array.</returns>
	/// <param name="IntegerChoices">Integer array of choices.</param>
	public string[] ChoicesToStringArray(int [] IntegerChoices) {
		HashSet<int> choiceSet = new HashSet<int> ();
		int size = IntegerChoices.Length;

		//Check for duplicate values in array. If found, add a number in a random range
		for (int i = 0; i < size; i++) {
			if (choiceSet.Contains(IntegerChoices[i])) {
				IntegerChoices [i] += Random.Range(1, 4);
			}
			choiceSet.Add(IntegerChoices[i]);
		}



		//Shuffle array randomly
		for (int i = 0; i < IntegerChoices.Length; i++ ) {
			int temp = IntegerChoices[i];
			int r = Random.Range(i, IntegerChoices.Length);
			IntegerChoices[i] = IntegerChoices[r];
			IntegerChoices[r] = temp;

		}

		//Populate choice array with generated answer choices, converted to strings for later use
		answerChoices = new string[] {IntegerChoices[0].ToString(), IntegerChoices[1].ToString(), 
			IntegerChoices[2].ToString(), IntegerChoices[3].ToString()};

		return answerChoices;
	}

	/**Return formatted question string
	 */
	public string GetQuestionString() {
		return questionString;
	}

	/**Return formatted answer string
	 */
	public string GetCorrectAnswer() {
		return correctAnswer.ToString();
	}

	public void SetCorrectAnswer(string answer) {
		this.correctAnswer =  System.Int32.Parse (answer);;
	}

	public void SetQuestionString(string question) {
		this.questionString = question;
	}

	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}
		
	public string GetQuestionCategory() {
		return "Pre-Algebra";
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
