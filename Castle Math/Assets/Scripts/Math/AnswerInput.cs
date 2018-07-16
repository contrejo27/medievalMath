using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class AnswerInput : MonoBehaviour {

	//public Text AnswerText;
	private MathManager M_Manager;
	public GameObject[] feedbackMarks;
	public Sprite xMark;
	public Sprite checkMark;

	//start
	public GameObject [] QuestionTexts;
	public Text QuestionText_hud;
	public GameObject [] FeedbackTexts;
	public GameObject [] ChoiceBoxes;
	public Text ChoiceBox;
	public TutorialBehavior tutorial;
	public UIEffects mathCanvas;

	private string CorrectAnswer;
	private string [] AnswerChoices;

	public AudioClip CorrectSound;
	public AudioClip[] interwaveCorrectSounds;

	public AudioClip IncorrectSound;

	private ArrowSupplier A_Supply;
	private PlayerMathStats Math_Stats;

	private int IncorrectAnswersPerQuestion;

	private AudioSource A_Source;

	private ManaBar PowerUp;

	public int interwaveQuestions = 0;

	// Use this for initialization
    public void Awake()
    {
        //QuestionTexts = GameObject.FindGameObjectsWithTag("Question");

    }

	public void Start () {
	
		PowerUp = FindObjectOfType<ManaBar> ();
		M_Manager = GameObject.FindObjectOfType<MathManager> ();
		A_Supply = GameObject.FindObjectOfType<ArrowSupplier> ();
		A_Source = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
		
		Math_Stats = GameObject.FindObjectOfType<PlayerMathStats> ();
		FeedbackTexts = GameObject.FindGameObjectsWithTag ("Feedback");
        
    }

	public void SetCorrectAnswer(string Answer) {
		this.CorrectAnswer = Answer;
	}

	public void ClearAnswer()
	{
		//AnswerText.text = "";
	}

	/// <summary>
	/// Clears the choices and sets ChoiceBox text to empty.
	/// </summary>
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

	/// <summary>
	/// Displays the choices on ChoiceBoxes.
	/// </summary>
	/// <param name="AnswerChoices">Answer choices.</param>
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
                    if (AnswerChoices[i - 1].ToString() == "")
                    {
                        ChoiceBox.gameObject.SetActive(false);
                    }
                    else
                    {
                        ChoiceBox.gameObject.SetActive(true);
                        ChoiceBox.text = AnswerChoices[i - 1].ToString();
                    }
				}
			}

		}

	}

	/// <summary>
	/// Checks the answer on the Text field against correct answer.
	/// </summary>
	/// <param name="Answer">The given answer</param>
	public void CheckAnswer(Text Answer) {
		//int answerAsInt = int.Parse(Answer.text.ToString());
		//check if we're in tutorial
		if(!tutorial.tutorialDone){
			mathCanvas.fadeOut(1.0f);
			tutorial.Next();
		}

		String answerText = Answer.text.ToString();


		//Loop through all FeedBack texts and check answers. Currently Length == 1, but in a loop to account for expansion

			if (answerText == CorrectAnswer) {
				if(M_Manager.interwaveMath){
					interWaveCorrectFeedack();
					interwaveQuestions++;
				}
				else{
					correctFeedack(FeedbackTexts);
					//("correct answer generating new problem");
					M_Manager.GenerateProblem (M_Manager.GetQuestionTypes());

				}
				Math_Stats.CorrectlyAnswered ();

				//If answered incorrectly more than once, place in incorrect question tracker
				if (M_Manager.GetIncorrectAnswersPerQuestion () >= 1) {
					GameStateManager.instance.tracker.AddIncorrectQuestion (M_Manager.GetCurrentQuestion (), M_Manager.GetIncorrectAnswersPerQuestion ());
				} else {
					GameStateManager.instance.tracker.AddCorrectQuestion (M_Manager.GetCurrentQuestion (), M_Manager.GetIncorrectAnswersPerQuestion ());
				}

				PowerUp.CorrectAnswer ();

			} 
			//got the question wrong
			else {
                Math_Stats.IncorrectlyAnswered();
                M_Manager.IncorrectAnswer ();
				if(M_Manager.interwaveMath){
					interWaveIncorrectFeedack();
					interwaveQuestions++;
				}
				else{
					incorrectFeedack(FeedbackTexts);
				}

				//ClearAnswer ();
				ClearChoices ();
				PowerUp.IncorrectAnswer ();
			}
				
		

		if (M_Manager.GetIncorrectAnswersPerQuestion () == 2) {
			//TODO: display tip graphic

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
			GameStateManager.instance.tracker.AddIncorrectQuestion (M_Manager.GetCurrentQuestion(), M_Manager.GetIncorrectAnswersPerQuestion());
			Debug.Log ("Current Question: " + M_Manager.GetCurrentQuestion ().GetQuestionString ());
			GameStateManager.instance.tracker.ShowIncorrectQestions ();
					print("incorrect answers generating new problem");

			M_Manager.GenerateProblem (M_Manager.GetQuestionTypes());
		}

		DisplayChoices (AnswerChoices);
	}

	void interWaveCorrectFeedack(){
		feedbackMarks[interwaveQuestions].SetActive(true);
		feedbackMarks[interwaveQuestions].GetComponent<Image>().sprite = checkMark;
		A_Source.clip = interwaveCorrectSounds[interwaveQuestions];
		A_Source.Play ();
		if (interwaveQuestions == 2) {
			interwaveQuestions = -1;

			A_Supply.CreateArrowIntermath (8);
			StartCoroutine(delayDeactivateMath());
		}
		else M_Manager.GenerateInterMathQuestion();


	}

	void interWaveIncorrectFeedack(){
		feedbackMarks[interwaveQuestions].SetActive(true);
		feedbackMarks[interwaveQuestions].GetComponent<Image>().sprite = xMark;
		A_Source.clip = IncorrectSound;
		A_Source.Play ();
        switch (interwaveQuestions)
        {
            case 0:
                A_Supply.CreateArrowIntermath(2);
                break;
            case 1:
                A_Supply.CreateArrowIntermath(4);
                break;
            default:
                break;
        }
		interwaveQuestions = -1;
		StartCoroutine(delayDeactivateMath());
		
	}

	void correctFeedack(GameObject[] Feedback){

		for (int i = 0; i < FeedbackTexts.Length; i++) {
			Text FeedbackText = FeedbackTexts [i].GetComponent<Text>();
			FeedbackText.text = "Correct";
			FeedbackText.color =  new Color(.188f, .44f, .1f);
			FeedbackText.gameObject.SetActive (true);
		}


		StartCoroutine (DisplayFeedback ());
		A_Supply.CreateArrow ();
		A_Source.clip = CorrectSound;
		A_Source.Play ();
	}

	void incorrectFeedack(GameObject[] Feedback){

		for (int i = 0; i < FeedbackTexts.Length; i++) {
			Text FeedbackText = FeedbackTexts [i].GetComponent<Text>();
			Debug.Log ("Incorrect");
			FeedbackText.text = "Incorrect";
			FeedbackText.color =  new Color(.756f,.278f, .29f);
			FeedbackText.gameObject.SetActive (true);
		}

		StartCoroutine (DisplayFeedback ());
		A_Source.clip = IncorrectSound;
		A_Source.Play ();
	}

	/// <summary>
	/// Sets the question display.
	/// </summary>
	/// <param name="question">Question.</param>
	public void SetQuestion(string question, int index = 0) {
        //Debug.Log("SHOULD BE SETTING QUESTION. QUESTIONTEXT LENGTH: " + QuestionTexts.Length);
		
		Text QuestionText = QuestionTexts [index].GetComponent<Text>();
        //Debug.Log("(AInput) Setting Question to: " + question + " in " + QuestionText.name );
		QuestionText.text = question;
	}

	public int GetCorrectOfType(System.Type type) {
		return GameStateManager.instance.tracker.GetCorrectOfType (type);
	}

	public int GetIncorrectOfType(System.Type type) {
		return GameStateManager.instance.tracker.GetIncorrectOfType (type);
	}

	IEnumerator DisplayFeedback()
	{
		yield return new WaitForSeconds (2);
		for (int i = 0; i < FeedbackTexts.Length; i++) {
			Text FeedbackText = FeedbackTexts [i].GetComponent<Text> ();
			FeedbackText.gameObject.SetActive (false);
		}

	}

	IEnumerator delayDeactivateMath()
	{
		yield return new WaitForSeconds (.7f);
		M_Manager.DeactivateInterMath();
		yield return new WaitForSeconds (1f);
		foreach(GameObject mark in feedbackMarks){
			mark.SetActive(false);		
		}

	}

}
