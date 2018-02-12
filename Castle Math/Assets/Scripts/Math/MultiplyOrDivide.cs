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
	private int incorrectAnswers = 0;
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

		//Array of all possible choices
		int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, CorrectAnswer };
		AnswerChoices = ChoicesToStringArray (IntegerChoices);

		A_Input.DisplayChoices (AnswerChoices);
	}

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

	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}

}
