using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplyOrDivide : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;
	public Text FeedbackText;

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


	// Use this for initialization
	void Start () {
		GenerateQuestion ();

		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();

		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
	}
	
	void GenerateQuestion () {

		isDivide = Random.Range (0, 1);

		if (isDivide == 0) {
			FirstNum = Random.Range (0, 11);
			SecondNum = Random.Range (0, 11);

			while (FirstNum % SecondNum != 0) {
				FirstNum = Random.Range (0, 11);
			}

			CorrectAnswer = FirstNum / SecondNum;

			QuestionText.text = FirstNum.ToString () + " / " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " / " + SecondNum.ToString ();
		} 
		else {
			FirstNum = Random.Range (0, 11);
			SecondNum = Random.Range (0, 11);

			CorrectAnswer = FirstNum * SecondNum;

			QuestionText.text = FirstNum.ToString () + " * " + SecondNum.ToString () + " =";
			QuestionText_hud.text = FirstNum.ToString () + " * " + SecondNum.ToString ();

		}
	}


	void GenerateChoices() {
		
		int Choice1;
		int Choice2;
		int Choice3;

		if (isDivide == 0) {
			Choice1 = FirstNum * SecondNum;

			int PlusOrMinus = Random.Range (0, 1);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		} else {
			Choice1 = FirstNum / SecondNum;

			int PlusOrMinus = Random.Range (0, 1);

			if (PlusOrMinus == 0) {
				Choice2 = CorrectAnswer - 1;
				Choice3 = CorrectAnswer + Random.Range (1, 5);
			} else {
				Choice2 = CorrectAnswer + 1;
				Choice3 = CorrectAnswer - Random.Range (1, 5);
			}
		}
		AnswerChoices = new int[] {Choice1, Choice2, Choice3, CorrectAnswer};
	}
		
	void CheckAnswer(int Answer) {
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

			A_Input.ClearAnswer ();

		}
	}


	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);

		FeedbackText.gameObject.SetActive (false);

	}
}
