using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class AnswerInput : MonoBehaviour {

	//public Text AnswerText;
	private MathManager M_Manager;

	//start
	public GameObject [] QuestionTexts;
	public Text QuestionText_hud;
	public GameObject [] FeedbackTexts;
	public GameObject [] ChoiceBoxes;
	public Text ChoiceBox;
	public TutorialBehavior tutorial;
	public UIEffects mathCanvas;

	QuestionTracker Tracker;

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
	
		PowerUp = FindObjectOfType<ManaBar> ();
		M_Manager = GameObject.FindObjectOfType<MathManager> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
		QuestionTexts = GameObject.FindGameObjectsWithTag ("Question");
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		FeedbackTexts = GameObject.FindGameObjectsWithTag ("Feedback");

		Tracker = new QuestionTracker ();
	}

	public void SetCorrectAnswer(string Answer) {
		this.CorrectAnswer = Answer;
	}

	public void ClearAnswer()
	{
		//AnswerText.text = "";

	}

	//start
	public void ClearChoices() {
		if (AnswerChoices == null) {
			AnswerChoices = new string [] { "" };
		}

		ChoiceBoxes = GameObject.FindGameObjectsWithTag ("ChoiceBox");

		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//Iterate through each choice box and set text to empty string
			ChoiceBox = ChoiceBoxes [i-1].GetComponent<Text>();
			ChoiceBox.text = "";


		}
	}

	public void DisplayChoices (String [] AnswerChoices) {
		this.AnswerChoices = AnswerChoices;

		ChoiceBoxes = GameObject.FindGameObjectsWithTag ("ChoiceBox");

		for (int i = 1; i <= AnswerChoices.Length; i++) {
			//iterate through choices boxes, assigning each text component
			//dynamically according to AnswerChoices
			string boxName = "answer" + i;
			for (int j = 0; j < ChoiceBoxes.Length; j++){
				ChoiceBox = ChoiceBoxes [j].GetComponent<Text>();

				if (ChoiceBox.name == boxName) {
					ChoiceBox.text = AnswerChoices [i - 1].ToString ();
				}
			}

		}

	}

	public void CheckAnswer(Text Answer) {
		//int answerAsInt = int.Parse(Answer.text.ToString());

		//check if we're in tutorial
		if(!tutorial.tutorialDone){
			mathCanvas.fadeOut(1.0f);
			tutorial.Next();
		}

		String answerText = Answer.text.ToString();

		for (int i = 0; i < 1; i++) {
			Text FeedbackText = FeedbackTexts [i].GetComponent<Text>();

			if (answerText == CorrectAnswer) {
				Debug.Log ("Correct");

				FeedbackText.text = "Correct";
				FeedbackText.color =  new Color(.188f, .44f, .1f);
				FeedbackText.gameObject.SetActive (true);
				StartCoroutine (DisplayFeedback ());

				//ClearAnswer ();

				A_Supply.CreateArrow ();
				A_Source.clip = CorrectSound;
				A_Source.Play ();
	
				Math_Stats.CorrectlyAnswered ();


				if (M_Manager.GetIncorrectAnswersPerQuestion () >= 1) {
					Tracker.AddIncorrectQuestion (M_Manager.GetCurrentQuestion (), M_Manager.GetIncorrectAnswersPerQuestion ());
				} else {
					Tracker.AddCorrectQuestion (M_Manager.GetCurrentQuestion (), M_Manager.GetIncorrectAnswersPerQuestion ());
				}

				M_Manager.GenerateProblem (M_Manager.GetQuestionTypes());

				PowerUp.CorrectAnswer ();

			} 
			//got the question wrong
			else {
				M_Manager.IncorrectAnswer ();
				Debug.Log ("Incorrect");
				FeedbackText.text = "Incorrect";
				FeedbackText.color =  new Color(.756f,.278f, .29f);
				FeedbackText.gameObject.SetActive (true);
				StartCoroutine (DisplayFeedback ());

				A_Source.clip = IncorrectSound;
				A_Source.Play ();

				//ClearAnswer ();
				ClearChoices ();
				PowerUp.IncorrectAnswer ();
			}
				
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
			Tracker.AddIncorrectQuestion (M_Manager.GetCurrentQuestion(), M_Manager.GetIncorrectAnswersPerQuestion());
			Debug.Log ("Current Question: " + M_Manager.GetCurrentQuestion ().GetQuestionString ());
			Tracker.ShowIncorrectQestions ();
			M_Manager.GenerateProblem (M_Manager.GetQuestionTypes());
		}

		DisplayChoices (AnswerChoices);
	}

	public void SetQuestion(string question) {
		for (int i = 0; i < QuestionTexts.Length; i++) {
			Text QuestionText = QuestionTexts [i].GetComponent<Text>();
			QuestionText.text = question;
		}
	}

	public int GetCorrectOfType(System.Type type) {
		return Tracker.GetCorrectOfType (type);
	}

	public int GetIncorrectOfType(System.Type type) {
		return Tracker.GetIncorrectOfType (type);
	}

	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);
		for (int i = 0; i < FeedbackTexts.Length; i++) {
			Text FeedbackText = FeedbackTexts [i].GetComponent<Text> ();
			FeedbackText.gameObject.SetActive (false);
		}

	}



}
