using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddOrSubtract : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;
	public Text FeedbackText;
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
	private int isSubtract;
	private int [] AnswerChoices;

	public AudioClip CorrectSound;
	public AudioClip IncorrectSound;

	private AnswerInput A_Input;
	private ArrowSupplier A_Supply;
	private PlayerMathStats Math_Stats;

	private AudioSource A_Source;


	// Use this for initialization
	void Start () {
		GenerateQuestion ();

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();

		QuestionText = GameObject.Find ("question").GetComponent<Text>();

	}

	// Update is called once per frame
	void GenerateQuestion () {
		Debug.Log ("Gen Questions");
		isSubtract = Random.Range (0, 2);

		if (isSubtract == 0) {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum - SecondNum;

			QuestionText.text = FirstNum.ToString () + " - " + SecondNum.ToString () + " =";
			//QuestionText_hud.text = FirstNum.ToString () + " - " + SecondNum.ToString ();
			GenerateChoices ();
		} 
		else {
			FirstNum = Random.Range (0, 13);
			SecondNum = Random.Range (0, 13);

			CorrectAnswer = FirstNum + SecondNum;

			QuestionText.text = FirstNum.ToString () + " + " + SecondNum.ToString () + " =";
			//QuestionText_hud.text = FirstNum.ToString () + " + " + SecondNum.ToString ();
			GenerateChoices ();

		}
		Debug.Log ("End Gen Choices");
	}

	void GenerateChoices() {
		Debug.Log ("Gen Choices");

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
		AnswerChoices = new int[] {Choice1, Choice2, Choice3, CorrectAnswer};

		DisplayChoices ();
	}

	void DisplayChoices () {
		Debug.Log ("Display");
		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			int temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;
		}

		ChoiceBox1.text = AnswerChoices [0].ToString();
		ChoiceBox2.text = AnswerChoices [1].ToString();
		ChoiceBox3.text = AnswerChoices [2].ToString();
		ChoiceBox4.text = AnswerChoices [3].ToString();

	}

	public void CheckAnswer(int Answer) {

		if (Answer == CorrectAnswer) {

			FeedbackText.text = "Correct";
			FeedbackText.color = Color.green;
			FeedbackText.gameObject.SetActive (true);
			StartCoroutine (DisplayFeedback ());

			A_Input.ClearAnswer ();

			//A_Supply.CreateArrow (ProblemType);

			A_Source.clip = CorrectSound;
			A_Source.Play ();

			Math_Stats.CorrectlyAnswered ();

			GenerateQuestion ();
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

		}
	}


	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);

		FeedbackText.gameObject.SetActive (false);

	}
}
