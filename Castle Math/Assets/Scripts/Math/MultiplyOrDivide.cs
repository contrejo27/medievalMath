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
	public int isDivide;
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

	public void GenerateOperands(int maxDifficulty) {
		//Generate random 0 or 1 to determine whether question is to be multiplication or divison
		//isDivide == 0 -> Division problem
		isDivide = Random.Range (0, 2);

		int maxIntMulti;

		if (maxDifficulty > 20) {
			maxIntMulti = maxDifficulty / 2;
			maxInt = maxDifficulty;
		} else {
			maxInt = maxDifficulty;
			maxIntMulti = maxDifficulty;
		}

		//check for division
		if (isDivide == 0) {
			FirstNum = Random.Range (0, maxInt + (maxInt / 2));
			SecondNum = Random.Range (1, maxInt);

			//Ensure that division is even, with no remainders
			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, maxInt + (maxInt / 2));
			}

			//Calculate correct answer
			CorrectAnswer = FirstNum / SecondNum;

		} 
		else {
			FirstNum = Random.Range (0, maxIntMulti);
			SecondNum = Random.Range (1, maxIntMulti);

			CorrectAnswer = FirstNum * SecondNum;
		}
	}

	/// <summary>
	/// Generates either mulitplication or division question by random selection
	/// </summary>
	/// <param name="maxDifficulty">The range of numbers from which to generate the question.</param>
	public void GenerateQuestion (int maxDifficulty) {
		GenerateOperands (maxDifficulty);

		//check for division
		if (isDivide == 0) {
			//Generate formatted question string and set text box text
			char divSign = '\u00F7';

			QuestionString = FirstNum.ToString () + " " + divSign.ToString() + " " + SecondNum.ToString () + " =";
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);

			//Generate other possible answer choices
			GenerateChoices ();
		} 
		else {

			if (maxDifficulty > 20) {
				int ThirdNum = 1;
				int NewSecondNum = SecondNum;

				for (int i = 5; i > 0; i++) {
					if (SecondNum % i == 0) {
						NewSecondNum = SecondNum / i;
						ThirdNum = i;
						break;
					}
				}

				QuestionString = FirstNum.ToString () + " " + " x " + " " + NewSecondNum.ToString () + " " + " x " + " " + ThirdNum.ToString () + " =";
			} else {
				//Generate formatted question string and set text box text
				QuestionString = FirstNum.ToString () + " x " + SecondNum.ToString () + " =";

			}
			//QuestionText.text = QuestionString;
			A_Input.SetQuestion(QuestionString);

			//Generate other possible answer choices
			GenerateChoices ();
		}
	}

	/// <summary>
	/// Generates the choices.
	/// </summary>
	public void GenerateChoices() {
		int Choice1;
		int Choice2;
		int Choice3;

		//Assign other choices depending on various factors
		if (isDivide == 0) {
			Choice1 = FirstNum * SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer / (CorrectAnswer - 1);
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer / (CorrectAnswer + 1);
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			
			Choice1 = FirstNum / SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer * (CorrectAnswer - 1);
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer * (CorrectAnswer + 1);
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		}

		//Array of all possible choices
		int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, CorrectAnswer };
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

	public int GetFirstNum() {
		return FirstNum;
	}

	public int GetSecondNum() {
		return SecondNum;
	}

	public string GetQuestionCategory() {
		if (isDivide == 0)
			return "Division";
		else
			return "Multiplication";
	}

}