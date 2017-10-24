using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplyOrDivide : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;
	public Text FeedbackText;
	public Text ChoiceBox;

	public Text ChoiceBox1;
	public Text ChoiceBox2;
	public Text ChoiceBox3;
	public Text ChoiceBox4;

	public Button Button1;
	public Button Button2;
	public Button Button3;
	public Button Button4;

	private int FirstNum;
	private int SecondNum;
	private int CorrectAnswer;
	private int isDivide;
	private int [] AnswerChoices;

	public AudioClip CorrectSound;
	public AudioClip IncorrectSound;

	private AnswerInput A_Input;
	private ArrowSupplier A_Supply;
	private PlayerMathStats Math_Stats;

	private AudioSource A_Source;

	private ManaBar PowerUp;
	// Use this for initialization
	void Start () {

		PowerUp = FindObjectOfType<ManaBar> ();

		GenerateQuestion ();

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

		/*
		ChoiceBox1 = GameObject.Find ("answer1").GetComponent<Text>();
		ChoiceBox2 = GameObject.Find ("answer2").GetComponent<Text>();
		ChoiceBox3 = GameObject.Find ("answer3").GetComponent<Text>();
		ChoiceBox4 = GameObject.Find ("answer4").GetComponent<Text>();
		*/
	}

	void GenerateQuestion () {

		isDivide = Random.Range (0, 2);

		//check for division
		if (isDivide == 0) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, 13);
			}

			CorrectAnswer = FirstNum / SecondNum;

			QuestionText.text = FirstNum.ToString () + " / " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " / " + SecondNum.ToString ();
			GenerateChoices ();
		} 
		else {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum * SecondNum;

			QuestionText.text = FirstNum.ToString () + " * " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " * " + SecondNum.ToString ();

			GenerateChoices ();
		}
	}


	void GenerateChoices() {

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
		//populate AnswerChoices array
		this.AnswerChoices = new int[] {Choice1, Choice2, Choice3, CorrectAnswer};

		DisplayChoices ();
	}

	void ClearChoices() {
		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//Iterate through each choice box and set text to empty string
			string boxName = "answer" + i;
			ChoiceBox = GameObject.Find (boxName).GetComponent<Text>();

			ChoiceBox.text = "";
		}
	}

	void DisplayChoices () {
		print ("Test");
		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			int temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//iterate through choices boxes, assigning each text component
			//dynamically according to AnswerChoices
			string boxName = "answer" + i;
			ChoiceBox = GameObject.Find (boxName).GetComponent<Text>();

			ChoiceBox.text = AnswerChoices [i - 1].ToString ();
		}


		/*
		ChoiceBox1.text = AnswerChoices [0].ToString();
		ChoiceBox2.text = AnswerChoices [1].ToString();
		ChoiceBox3.text = AnswerChoices [2].ToString();
		ChoiceBox4.text = AnswerChoices [3].ToString();
		*/

	}

	void CheckAnswer(Text Answer) {
		int answerAsInt = int.Parse(Answer.text.ToString());
		if (answerAsInt == CorrectAnswer) {

			FeedbackText.text = "Correct";
			FeedbackText.color = Color.green;
			FeedbackText.gameObject.SetActive (true);
			StartCoroutine (DisplayFeedback ());

			A_Input.ClearAnswer ();

			A_Supply.CreateArrow ();

			A_Source.clip = CorrectSound;
			A_Source.Play ();

			Math_Stats.CorrectlyAnswered ();

			GenerateQuestion ();

			PowerUp.QuestionAnswered ();

		} 
		//got the question wrong
		else {

			FeedbackText.text = "Incorrect";
			FeedbackText.color = Color.red;
			FeedbackText.gameObject.SetActive (true);
			StartCoroutine (DisplayFeedback ());

			A_Source.clip = IncorrectSound;
			A_Source.Play ();

			Math_Stats.IncorrectlyAnswered ();

			A_Input.ClearAnswer ();
			ClearChoices ();

		}
	}


	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);

		FeedbackText.gameObject.SetActive (false);

		if (Math_Stats.GetIncorrectAnswers() > 2) {
			//display tip graphic

			//Find random index at which to remove an answer choice
			int index = Random.Range (0, AnswerChoices.Length);

			//Check that the answer at that index is not the correct one
			while (AnswerChoices [index] == CorrectAnswer) {
				index = Random.Range (0, AnswerChoices.Length);
			}

			//Create new array, one index shorter than AnswerChoices
			int[] AnswerChoicesCopy = new int[AnswerChoices.Length - 1];

			for (int i = 0, j = 0; i < AnswerChoicesCopy.Length; i++, j++) {
				//Skip if that is the element to remove
				if (i == index) {
					j++;
				}

				//Assign answer choices to new array, minus element removed
				AnswerChoicesCopy [i] = AnswerChoices [j];
			}
			//Resassign answer choices to new array
			this.AnswerChoices = AnswerChoicesCopy;
		}
		DisplayChoices ();
	}
}