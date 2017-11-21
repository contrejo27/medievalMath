using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddOrSubtract : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;

	private int FirstNum;
	private int SecondNum;
	private int CorrectAnswer;
	private int isSubtract;
	private string [] AnswerChoices;
	private string QuestionString;

	private AnswerInput A_Input;

	//public QuestionGenerator QG;

	public AddOrSubtract() {
	} 

	// Use this for initialization
	public void Start () {
		/*
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		//QuestionText_hud = GameObject.Find ("Question_hud").GetComponent<Text> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

		//GenerateQuestion ();
		*/
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}
	/*
	public void GenerateQuestion() {
		isSubtract = Random.Range (0, 2);

		if (isSubtract == 0) {
			QG.GenerateQuestion ("subtract");
		} else {
			QG.GenerateQuestion ("add");
		}
	}
    */

	// Update is called once per frame
	public void GenerateQuestion () {
		//Debug.Log ("Gen Questions");
		isSubtract = Random.Range (0, 2);

		if (isSubtract == 0) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum - SecondNum;
			QuestionString = FirstNum.ToString () + " - " + SecondNum.ToString () + " =";

			QuestionText.text = QuestionString;

			GenerateChoices ();
		} 
		else {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum + SecondNum;

			QuestionString = FirstNum.ToString () + " + " + SecondNum.ToString () + " =";
			QuestionText.text = QuestionString;
			GenerateChoices ();

		}
		//Debug.Log ("End Gen Choices");
	}
		
	void GenerateChoices() {
		//Debug.Log ("Gen Choices");

		int Choice1;
		int Choice2;
		int Choice3;

		if (isSubtract == 0) {
			Choice1 = FirstNum + SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			Choice1 = FirstNum - SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		}
		AnswerChoices = new string[] {Choice1.ToString(), Choice2.ToString(), 
										Choice3.ToString(), CorrectAnswer.ToString()};

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
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		A_Input.DisplayChoices (AnswerChoices);
	}

	public string GetQuestionString() {
		return this.QuestionString;
	}

	public string getCorrectAnswer() {
		return this.CorrectAnswer.ToString();
	}


}
