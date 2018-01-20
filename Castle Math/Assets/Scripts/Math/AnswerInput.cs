using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class AnswerInput : MonoBehaviour {

	private string CurrentAnswer;

	public Text AnswerText;

	private MathManager M_Manager;

	//start
	public Text QuestionText;
	public Text QuestionText_hud;
	public Text FeedbackText;
	public Text ChoiceBox;

	private string CorrectAnswer;
	private string [] AnswerChoices;

	public AudioClip CorrectSound;
	public AudioClip IncorrectSound;

	private ArrowSupplier A_Supply;
	private PlayerMathStats Math_Stats;

	private int IncorrectAnswersPerQuestion;

	private AudioSource A_Source;

	private ManaBar PowerUp;

	// Use this for initialization
	public void Start () {
	
		CurrentAnswer = "";
		PowerUp = FindObjectOfType<ManaBar> ();
		M_Manager = GameObject.FindObjectOfType<MathManager> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		FeedbackText = GameObject.Find ("feedback").GetComponent<Text>();

	}

	public void SetCorrectAnswer(string Answer) {
		this.CorrectAnswer = Answer;
	}

	public void ClearAnswer()
	{
		CurrentAnswer = "";
		AnswerText.text = "";

	}

	public void AddTo(string Digit)
	{
		CurrentAnswer += Digit;

		AnswerText.text = CurrentAnswer;
	}


	public void BackSpace()
	{
		if (CurrentAnswer.Length > 0) {
			CurrentAnswer = CurrentAnswer.Substring (0, CurrentAnswer.Length - 1);

			AnswerText.text = CurrentAnswer;
		}

	}

	//start
	public void ClearChoices() {
		if (AnswerChoices == null) {
			AnswerChoices = new string [] { "" };
		}

		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//Iterate through each choice box and set text to empty string
			string boxName = "answer" + i;
			ChoiceBox = GameObject.Find (boxName).GetComponent<Text>();

			ChoiceBox.text = "";
		}
	}

	public void DisplayChoices (String [] AnswerChoices) {
		this.AnswerChoices = AnswerChoices;
		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//iterate through choices boxes, assigning each text component
			//dynamically according to AnswerChoices
			string boxName = "answer" + i;
			ChoiceBox = GameObject.Find (boxName).GetComponent<Text>();

			ChoiceBox.text = AnswerChoices [i - 1].ToString ();
			Debug.Log(AnswerChoices [i - 1].ToString ());
		}

	}

	public void CheckAnswer(Text Answer) {
		//int answerAsInt = int.Parse(Answer.text.ToString());
		String answerText = Answer.text.ToString();
		if (answerText == CorrectAnswer) {

			FeedbackText.text = "Correct";
			FeedbackText.color =  new Color(.188f, .44f, .1f);
			FeedbackText.gameObject.SetActive (true);
			StartCoroutine (DisplayFeedback ());

			ClearAnswer ();

			A_Supply.CreateArrow ();

			A_Source.clip = CorrectSound;
			A_Source.Play ();

			Math_Stats.CorrectlyAnswered ();

			M_Manager.GenerateProblem (M_Manager.GetQuestionType());

			PowerUp.QuestionAnswered ();

		} 
		//got the question wrong
		else {
			M_Manager.IncorrectAnswer ();
			FeedbackText.text = "Incorrect";
			FeedbackText.color =  new Color(.756f,.278f, .29f);
			FeedbackText.gameObject.SetActive (true);
			StartCoroutine (DisplayFeedback ());

			A_Source.clip = IncorrectSound;
			A_Source.Play ();

			ClearAnswer ();
			ClearChoices ();

		}

		if (M_Manager.GetIncorrectAnswersPerQuestion () == 2) {
			//display tip graphic

			//Find random index at which to remove an answer choice
			int index = Random.Range (0, AnswerChoices.Length);

			//Check that the answer at that index is not the correct one
			while (AnswerChoices [index] == CorrectAnswer) {
				index = Random.Range (0, AnswerChoices.Length);
			}

			//Create new array, one index shorter than AnswerChoices
			string[] AnswerChoicesCopy = new string[AnswerChoices.Length - 1];

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
		} else if (M_Manager.GetIncorrectAnswersPerQuestion() == 3) {
			M_Manager.GenerateProblem (M_Manager.GetQuestionType());
		}

		DisplayChoices (AnswerChoices);
	}


	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);

		FeedbackText.gameObject.SetActive (false);

	}

		

}
