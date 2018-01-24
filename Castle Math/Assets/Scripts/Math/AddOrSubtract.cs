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
	int isSubtract;
	string [] AnswerChoices;
	string QuestionString;
	int maxInt;
	AnswerInput A_Input;

	public AddOrSubtract() {
		
	} 

	// Use this for initialization
	public void Start () {

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}

	/** Generates addition or subtraction question by random selection. 
	*/
	public void GenerateQuestion (int maxDifficulty) {
		//Randomly create variable to determine whether generated question is subtraction or addition
		isSubtract = Random.Range (0, 2);

		if (maxDifficulty != -1) {
			maxInt = maxDifficulty;
		}


		if (isSubtract == 0) {
			//Numbers to be used in problem equation
			FirstNum = Random.Range (0, maxInt);
			SecondNum = Random.Range (0, 13);

			//Calculate correct answer
			CorrectAnswer = FirstNum - SecondNum;

			//Create formatted question string for display
			QuestionString = FirstNum.ToString () + " - " + SecondNum.ToString () + " =";

			//Set textbox display to formatted question string
			QuestionText.text = QuestionString;

			//Generate choices for possible answers 
			this.GenerateChoices ();
		} 
		else { //Generate addition question
			//Numbers to be used in problem equation
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			//Calculate correct answer
			CorrectAnswer = FirstNum + SecondNum;

			//Create formatted question string for display
			QuestionString = FirstNum.ToString () + " + " + SecondNum.ToString () + " =";

			//Set textbox display to formatted question string
			QuestionText.text = QuestionString;

			//Generate choices for possible answers
			GenerateChoices ();

		}
	}

	/**Generate choices based on question and calculated correct answer created in GenerateQuestion()
	 */
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
		int size = IntegerChoices.Length;

		//Check for duplicate values in array. If found, add a number in a random range
		for (int i = 0; i < size - 1; i++){
			for (int j = i + 1; j < size; j++) {
				if ( IntegerChoices [i] == IntegerChoices [j]) {
					IntegerChoices [i] += Random.Range(1, 4);

				}
			}
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


		A_Input.DisplayChoices (AnswerChoices);
	}

	/** Returns the formatted question string
	 */ 
	public string GetQuestionString() {
		return this.QuestionString;
	}

	/**Returns the correct answer as string
	 */
	public string getCorrectAnswer() {
		return this.CorrectAnswer.ToString();
	}


}
