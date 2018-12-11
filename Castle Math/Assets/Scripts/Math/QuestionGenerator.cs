using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionGenerator : MonoBehaviour {

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

	private string currentQuestionType;

	private AudioSource A_Source;

	private ManaBar PowerUp;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void GenerateQuestion(string QuestionType) {
		currentQuestionType = QuestionType;
        Debug.Log("Question type: " + QuestionType);

		if (currentQuestionType.Equals ("divide")) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			//check for division by zero
			while (SecondNum == 0) {
				SecondNum = Random.Range (0, 13);
			}

			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, 13);
			}

			CorrectAnswer = FirstNum / SecondNum;

			QuestionText.text = FirstNum.ToString () + " / " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " / " + SecondNum.ToString ();
			GenerateChoices (currentQuestionType);

		} else if (currentQuestionType.Equals ("multiply")) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum * SecondNum;

			QuestionText.text = FirstNum.ToString () + " * " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " * " + SecondNum.ToString ();

			GenerateChoices (currentQuestionType);

		} else if (currentQuestionType.Equals ("add")) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum + SecondNum;

			QuestionText.text = FirstNum.ToString () + " + " + SecondNum.ToString () + " =";
			//QuestionText_hud.text = FirstNum.ToString () + " + " + SecondNum.ToString ();
			GenerateChoices (currentQuestionType);
		} else if (currentQuestionType.Equals ("subtract")) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum - SecondNum;

			QuestionText.text = FirstNum.ToString () + " - " + SecondNum.ToString () + " =";
			//QuestionText_hud.text = FirstNum.ToString () + " - " + SecondNum.ToString ();
			GenerateChoices (currentQuestionType);
		}
	}

	public void GenerateChoices(string QuestionType) {
		currentQuestionType = QuestionType;
		int Choice1;
		int Choice2;
		int Choice3;

		if (currentQuestionType.Equals ("multiply")) {
			//if SecondNum is zero, set default to 2 to avoid divison by 0
			if (SecondNum == 0)
				SecondNum = 2;

			Choice1 = FirstNum / SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else if (currentQuestionType.Equals ("divide")) {
			Choice1 = FirstNum * SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else if (currentQuestionType.Equals ("subtract")) {
			Choice1 = FirstNum + SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else if (currentQuestionType.Equals ("add")) {
			Choice1 = FirstNum - SecondNum;

			int PlusOrMinus = Random.Range (0, 2);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			Choice1 = 0;
			Choice2 = 0;
			Choice3 = 0;
			Debug.Log ("ERROR IN CHOICE ASSIGNMENT");
		}

		//bool UniqueValues = true;
		//populate AnswerChoices array

		this.AnswerChoices = new int[] { Choice1, Choice2, Choice3, CorrectAnswer };

		int size = AnswerChoices.Length;

		for (int i = 0; i < size - 1; i++){
			Debug.Log ("first for");
			for (int j = i + 1; j < size; j++) {
				if ( AnswerChoices [i] == AnswerChoices [j]) {
					Debug.Log("Before:" + AnswerChoices [i] + ", " + AnswerChoices [j]);
					AnswerChoices [i] += Random.Range(1, 4);
					Debug.Log("After:" + AnswerChoices [i] + ", " + AnswerChoices [j]);

				}
			}
		}
		DisplayChoices ();
	}

	public void ClearChoices() {

		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//Iterate through each choice box and set text to empty string
			string boxName = "answer" + i;
			ChoiceBox = GameObject.Find (boxName).GetComponent<Text>();
			ChoiceBox.text = "";
		}
	}

	public void DisplayChoices () {
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

	}

	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);
		FeedbackText.gameObject.SetActive (false);
	}
}
