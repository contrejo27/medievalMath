using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplyOrDivide : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;

	private int FirstNum;
	private int SecondNum;
	private int CorrectAnswer;
	private int isDivide;
	private int [] AnswerChoices;
	private string QuestionString;

	private AnswerInput A_Input;

	private int IncorrectAnswersPerQuestion;


	//public QuestionGenerator QG;
	// Use this for initialization
	public MultiplyOrDivide() {
	}

	public void Start () {
		/*
		PowerUp = FindObjectOfType<ManaBar> ();

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
		QuestionText_hud = GameObject.Find ("Question_hud").GetComponent<Text> ();


		//GenerateQuestion ();
		*/
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}
	/*
	public void GenerateQuestion() {
		isDivide = Random.Range (0, 2);

		if (isDivide == 0) {
			QG.GenerateQuestion ("divide");
		} else {
			QG.GenerateQuestion ("mulitply");
		}
	}
	*/
	public void GenerateQuestion () {
		//reset incorrect answer count
		IncorrectAnswersPerQuestion = 0;

		isDivide = Random.Range (0, 2);

		//check for division
		if (isDivide == 0) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (1, 13);
			
			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, 13);
			}

			CorrectAnswer = FirstNum / SecondNum;

			QuestionString = FirstNum.ToString () + " / " + SecondNum.ToString () + " =";
			QuestionText.text = QuestionString;

			GenerateChoices ();
		} 
		else {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (1, 13);

			CorrectAnswer = FirstNum * SecondNum;

			QuestionString = FirstNum.ToString () + " x " + SecondNum.ToString () + " =";
			QuestionText.text = QuestionString;

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

		bool UniqueValues = true;
		//populate AnswerChoices array

		AnswerChoices = new int[] { Choice1, Choice2, Choice3, CorrectAnswer };

		int size = AnswerChoices.Length;

		for (int i = 0; i < size - 1; i++){
			//Debug.Log ("first for");
			for (int j = i + 1; j < size; j++) {
				if ( AnswerChoices [i] == AnswerChoices [j]) {
					//Debug.Log("Before:" + AnswerChoices [i] + ", " + AnswerChoices [j]);
					AnswerChoices [i] += Random.Range(1, 4);
					//Debug.Log("After:" + AnswerChoices [i] + ", " + AnswerChoices [j]);

				}
			}
		}

		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			int temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		A_Input.DisplayChoices (AnswerChoices);
	}

	public string GetQuestionString() {
		return QuestionString;
	}

	public int getCorrectAnswer() {
		return CorrectAnswer;
	}


}