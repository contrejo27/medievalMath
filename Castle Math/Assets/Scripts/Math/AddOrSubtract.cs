using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddOrSubtract : MonoBehaviour, Question {

	public Text QuestionText;
	public Text QuestionText_hud;

	int FirstNum;
	int SecondNum;
	int CorrectAnswer;
	public int isSubtract;
	string [] AnswerChoices;
	string QuestionString;
	int maxInt;
	AnswerInput A_Input;
	private int incorrectAnswers;

	public AddOrSubtract() {
		
	} 

	// Use this for initialization
	public void Start () {

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}

	public void GenerateOperands(int maxDifficulty) {
        print("DIFFICULTY = " + maxDifficulty);
		isSubtract = Random.Range (0, 2);

		if (maxDifficulty != -1) {
			maxInt = maxDifficulty;
		}

		//Execute subtraction functionality
		if (isSubtract == 0) {
            if (maxDifficulty > 25)
            {
                FirstNum = Random.Range(0, maxInt);
                SecondNum = Random.Range(0, maxInt * 2);
            }
            else{
                //Numbers to be used in problem equation
                FirstNum = Random.Range(0, maxInt);
                SecondNum = Random.Range(0, FirstNum);
            }
            //Calculate correct answer
            CorrectAnswer = FirstNum - SecondNum;

        } else { //Generate addition question
			//Numbers to be used in problem equation
			FirstNum = Random.Range (0, maxInt);
			SecondNum = Random.Range (0, maxInt);

			//Calculate correct answer
			CorrectAnswer = FirstNum + SecondNum;
		}
	}

	///<summary>
	///Generates addition or subtraction question by random selection. 
	///</summary>
	public void GenerateQuestion (int maxDifficulty) {
		GenerateOperands (maxDifficulty);
		//Execute subtraction functionality
		if (isSubtract == 0) {	

			//Create formatted question string for display
			QuestionString = FirstNum.ToString () + " - " + SecondNum.ToString () + " =";

			//Set textbox display to formatted question string
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);


			//Generate choices for possible answers 
			this.GenerateChoices ();
		} 
		else { //Generate addition question

			if (maxDifficulty > 20) {
                
                int NewSecondNum = SecondNum;

				int diff = Random.Range (0, 5);

				while (NewSecondNum - diff <= 0) {
					NewSecondNum++;
					diff--;
				} 
				NewSecondNum -= diff;
				int ThirdNum = diff;

				QuestionString = FirstNum.ToString () + " " + " + " + " " + NewSecondNum.ToString () + " " + " + " + " " + ThirdNum.ToString () + " =";
			} else {
				//Create formatted question string for display
				QuestionString = FirstNum.ToString () + " + " + SecondNum.ToString () + " =";
			}

			//Set textbox display to formatted question string
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);


			//Generate choices for possible answers
			GenerateChoices ();

		}
	}

	/// <summary>
	/// Generate choices based on question and calculated correct answer created in GenerateQuestion
	/// </summary>
	public void GenerateChoices() {

		int Choice1;
		int Choice2;
		int Choice3;

		//Check for subtraction vs addition
		if (isSubtract == 0) {

			//First choice is result of opposite operation
			Choice1 = FirstNum + SecondNum;

			//Either add or subtract randomly to or from correct answer for remaining choices
			int PlusOrMinus = Random.Range (0, 2);
			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			//First choice is result of opposite operation
			Choice1 = FirstNum - SecondNum;

			//Either add or subtract randomly to or from correct answer for remaining choices
			int PlusOrMinus = Random.Range (0, 2);
			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		}
			
		int[] IntegerChoices = new int [] { Choice1, Choice2, Choice3, CorrectAnswer };

		AnswerChoices = ChoicesToStringArray (IntegerChoices);
		A_Input.DisplayChoices (AnswerChoices);
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
		AnswerChoices = new string[] {IntegerChoices[0].ToString(), IntegerChoices[1].ToString(), 
			IntegerChoices[2].ToString(), IntegerChoices[3].ToString()};

		return AnswerChoices;
	}

	/// <summary>
	/// Returns the formatted question string
	/// </summary>
	/// <returns>The question string.</returns>
	public string GetQuestionString() {
		return this.QuestionString;
	}

	/// <summary>
	/// Returns the correct answer as string
	/// </summary>
	/// <returns>The correct answer.</returns>
	public string getCorrectAnswer() {
		return this.CorrectAnswer.ToString();
	}

	/// <summary>
	/// Sets the correct answer.
	/// </summary>
	/// <param name="answer">Answer.</param>
	public void SetCorrectAnswer(string answer) {
		this.CorrectAnswer = System.Int32.Parse (answer);
	}

	/// <summary>
	/// Sets the question string.
	/// </summary>
	/// <param name="question">Question.</param>
	public void SetQuestionString(string question) {
		this.QuestionString = question;
	}

	/// <summary>
	/// Sets the incorrect answers.
	/// </summary>
	/// <param name="incorrect">Incorrect.</param>
	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	/// <summary>
	/// Gets the incorrect answers.
	/// </summary>
	/// <returns>The incorrect answers.</returns>
	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}

	public int GetFirstNum() {
		return FirstNum;
	}

	public int GetSecondNum() {
		return SecondNum;
	}

	public string GetQuestionCategory() {
		if (isSubtract == 0)
			return "Subtraction";
		else
			return "Addition";
	}



}
