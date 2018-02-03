using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplyOrDivide : MonoBehaviour, Question {

	public Text QuestionText;
	public Text QuestionText_hud;

	private int FirstNum;
	private int SecondNum;
	private int CorrectAnswer;
	private int isDivide;
	private string [] AnswerChoices;
	private string QuestionString;
	int maxInt = 10;
	private AnswerInput A_Input;

	public MultiplyOrDivide() {
		
	}

	// Use this for initialization
	public void Start () {

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}

	/** Generates either mulitplication or division question by random selection
	 */
	public void GenerateQuestion (int maxDifficulty) {
		//Generate random 0 or 1 to determine whether question is to be multiplication or divison
		//isDivide == 0 -> Division problem
		isDivide = Random.Range (0, 2);

		maxInt = maxDifficulty;

		//check for division
		if (isDivide == 0) {
			FirstNum = Random.Range (0, maxInt);
			SecondNum = Random.Range (1, maxInt);

			//Ensure that division is even, with no remainders
			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, 13);
			}

			//Calculate correct answer
			CorrectAnswer = FirstNum / SecondNum;

			//Generate formatted question string and set text box text
			QuestionString = FirstNum.ToString () + " / " + SecondNum.ToString () + " =";
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);

			//Generate other possible answer choices
			GenerateChoices ();
		} 
		else {
			FirstNum = Random.Range (0, maxInt);
			SecondNum = Random.Range (1, maxInt);

			CorrectAnswer = FirstNum * SecondNum;

			//Generate formatted question string and set text box text
			QuestionString = FirstNum.ToString () + " x " + SecondNum.ToString () + " =";
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);

			//Generate other possible answer choices
			GenerateChoices ();
		}
	}

	public void GenerateChoices() {
		int Choice1;
		int Choice2;
		int Choice3;

		//Assign other choices depending on various factors
		if (isDivide == 0) {
			Choice1 = FirstNum * SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			
			Choice1 = FirstNum / SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		}

		//TODO change to Hashmap with lookup for duplicate values
		//Array of all possible choices
		int[] integerChoices = new int[] { Choice1, Choice2, Choice3, CorrectAnswer };
		int size = integerChoices.Length;

		//Check for duplicate values and add random integer to them if necessary
		for (int i = 0; i < size - 1; i++){
			for (int j = i + 1; j < size; j++) {
				if ( integerChoices [i] == integerChoices [j]) {
					integerChoices [i] += Random.Range(1, 4);

				}
			}
		}

		//Convert integer choices to strings for later implementation
		AnswerChoices = new string[] { Choice1.ToString (), Choice2.ToString (), 
			Choice3.ToString (), CorrectAnswer.ToString ()
		};

		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		A_Input.DisplayChoices (AnswerChoices);
	}

	/**Return formatted question string
	 */
	public string GetQuestionString() {
		return QuestionString;
	}

	/**Return formatted answer string
	 */
	public string getCorrectAnswer() {
		return CorrectAnswer.ToString();
	}

	public void SetCorrectAnswer(string answer) {
		this.CorrectAnswer =  System.Int32.Parse (answer);;
	}

	public void SetQuestionString(string question) {
		this.QuestionString = question;
	}
		


}
