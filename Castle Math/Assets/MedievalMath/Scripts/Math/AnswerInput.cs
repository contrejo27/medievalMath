using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class AnswerInput : MonoBehaviour {

    public GameObject[] feedbackMarks;
    public Sprite xMark;
    public Sprite checkMark;

    public TutorialBehavior tutorial;
    public UIEffects mathCanvas;

    public GameObject[] questionTexts;
    public GameObject[] feedbackTexts;
    public GameObject[] choiceBoxes;
    public string currentQuestion;
    public string selectedAnswer;

    public Text choiceBox;
    public Text QuestionText_hud;
    private string correctAnswer;
    private string[] answerChoices;

    private AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip[] interwaveCorrectSounds;

    private ArrowSupplier arrowSupplier;
    private PlayerMathStats mathStats;
    //private ManaBar manaBar;

    [HideInInspector]
    public int interwaveQuestionsForWave = 2;
    public int interwaveQuestions = 0;
    private int incorrectAnswersPerQuestion;
    private bool isCorrect = false;
   // private TelemetryManager m_telemetry;

    public void Awake() {
        // interwaveQuestionsForWave = 2;
        choiceBoxes = GameObject.FindGameObjectsWithTag("ChoiceBox");
        //m_telemetry = GameObject.FindObjectOfType<TelemetryManager>();
    }

    public void Start () {
       // manaBar = FindObjectOfType<ManaBar> ();
        arrowSupplier = GameObject.FindObjectOfType<ArrowSupplier> ();
        audioSource = GameObject.Find ("PlayerAudio").GetComponent<AudioSource> ();
        mathStats = GameObject.FindObjectOfType<PlayerMathStats> ();
        feedbackTexts = GameObject.FindGameObjectsWithTag ("Feedback");
    }

    public void SetCorrectAnswer (string answer) {
        this.correctAnswer = answer;
		//Debug.Log ("Answer is changed to: " + this.correctAnswer);
    }

	public string GetCorrectAnswer () {
		return this.correctAnswer;
	}

    public bool GetIsCorrect()
    {
        return this.isCorrect;
    }

    public void ClearAnswer () {
        // answerText.text = "";
    }

    public void ClearChoices () {
        /// <summary>
        /// Clears the choices and sets choiceBox text to empty.
        /// </summary>
        
        if (answerChoices == null) {
            answerChoices = new string[] { "" };
        }

        // choiceBoxes = GameObject.FindGameObjectsWithTag ("choiceBox");

        for (int i = 1; i <= choiceBoxes.Length; i++) {
            //Iterate through each choice box and set text to empty string
            choiceBoxes[i - 1].transform.parent.gameObject.SetActive(true);
            choiceBox = choiceBoxes[i - 1].GetComponent<Text>();
            choiceBox.text = "";

        }
    }

    public void DisplayChoices (string [] answerChoices) {
        /// <summary>
        /// Displays the choices on choiceBoxes.
        /// </summary>
        /// <param name="answerChoices">Answer choices.</param>

        this.answerChoices = answerChoices;

        for (int i = 1; i <= answerChoices.Length; i++) {
            // iterate through choices boxes, assigning each text component
            // dynamically according to answerChoices
            string boxName = "answer" + i;
            for (int j = 0; j < choiceBoxes.Length; j++) {
                if (choiceBoxes[j].name == boxName) {
                    choiceBoxes[j].transform.parent.gameObject.SetActive(true);
                    choiceBox = choiceBoxes[j].GetComponent<Text>();
					if (answerChoices[i - 1].ToString() == "") {
                        choiceBox.transform.parent.gameObject.SetActive(false);
                    }
                    else {
                        choiceBox.transform.parent.gameObject.SetActive(true);
                        choiceBox.text = answerChoices[i - 1].ToString();
                    }
                }
            }

        }

    }

    public void CheckAnswer(Text answer) {
        /// <summary>
        /// Checks the answer on the Text field against correct answer.
        /// </summary>
        /// <param name="answer">The given answer</param>

        if (!GameStateManager.instance.levelManager.isGamePaused) {
            // int answerAsInt = int.Parse(answer.text.ToString());
            // check if we're in tutorial
            if (!tutorial.tutorialDone) {
                mathCanvas.fadeOut(1.0f);
                tutorial.Next();
            }

            string answerText = answer.text.ToString();
            //Debug.Log ("You have selected: " + answer.text.ToString());
            //Debug.Log ("Really real correct 'answer': " + correctAnswer);
            // Loop through all FeedBack texts and check answers. Currently Length == 1, but in a loop to account for expansion


            GameStateManager.instance.gameObject.GetComponent<SendToGoogle>().SendCustom(SystemInfo.deviceModel.ToString() + "," + Time.time.ToString() + ", Question:" + GameObject.Find("question").GetComponent<Text>().text+  ", "  + SystemInfo.deviceName.ToString() + ",Student Answer: " + answerText + ", Right Answer: " + correctAnswer.ToString());
            
            if (answerText == correctAnswer)
            {
                OnCorrect();
                isCorrect = true;
            }
            else
            {
                OnIncorrect();
                isCorrect = false;
            }
            selectedAnswer = answerText;
            DisplayChoices(answerChoices);
            //m_telemetry.LogResponse();
        }
    }

    public void OnCorrect() {
        if (MathManager.instance.interwaveMath) {
            GameStateManager.instance.currentState = EnumManager.GameState.PotionShop;
            GameStateManager.instance.ActivatePotionShop();
            InterWaveCorrectFeedback();
            interwaveQuestions++;
        }
        else {
            CorrectFeedback(feedbackTexts);

            // ("correct answer generating new problem");
            MathManager.instance.GenerateProblem();

        }

        mathStats.CorrectlyAnswered ();

        // If answered incorrectly more than once, place in incorrect question tracker
        if (MathManager.instance.GetIncorrectAnswersPerQuestion() >= 1) {
            GameStateManager.instance.tracker.AddIncorrectQuestion(MathManager.instance.GetCurrentQuestion(), MathManager.instance.GetIncorrectAnswersPerQuestion());
        }
        else {
            GameStateManager.instance.tracker.AddCorrectQuestion(MathManager.instance.GetCurrentQuestion(), MathManager.instance.GetIncorrectAnswersPerQuestion());
        }

        //manaBar.CorrectAnswer();

        CheckNumIncorrect();
    }

    public void OnIncorrect () {
        mathStats.IncorrectlyAnswered();
        MathManager.instance.IncorrectAnswer();
        if (MathManager.instance.interwaveMath) {
            InterWaveIncorrectFeedback();
            GameStateManager.instance.waveManager.NextWave();
            //interwaveQuestions++;
        }
        else {
            IncorrectFeedback(feedbackTexts);
        }

        ClearChoices();
        //manaBar.IncorrectAnswer();

        CheckNumIncorrect();

    }

    public void CheckNumIncorrect () {
        if (MathManager.instance.GetIncorrectAnswersPerQuestion() == 2) {
            // TODO: display tip graphic

            // Find random index at which to remove an answer choice
            int index = Random.Range(0, answerChoices.Length);

            // Check that the answer at that index is not the correct one
            while (answerChoices[index] == correctAnswer) {
                index = Random.Range(0, answerChoices.Length);
            }

            // Create new array, one index shorter than answerChoices
            string[] answerChoicesCopy = new string[answerChoices.Length - 1];

            for (int i = 0, j = 0; i < answerChoicesCopy.Length; i++, j++) {
                // Skip if that is the element to remove
                if (i == index) {
                    j++;
                }

                //Assign answer choices to new array, minus element removed
                answerChoicesCopy[i] = answerChoices[j];
            }

            //Resassign answer choices to new array
            this.answerChoices = answerChoicesCopy;
        }
        else if (MathManager.instance.GetIncorrectAnswersPerQuestion() == 3) {
            GameStateManager.instance.tracker.AddIncorrectQuestion(MathManager.instance.GetCurrentQuestion(), MathManager.instance.GetIncorrectAnswersPerQuestion());
            GameStateManager.instance.tracker.ShowIncorrectQestions();

            MathManager.instance.GenerateProblem();
        }
    }

    void InterWaveCorrectFeedback() {
        Debug.Log("Interwave questions: " + interwaveQuestions + " Interwave questions for wave: " + interwaveQuestionsForWave);
        feedbackMarks[interwaveQuestions].SetActive(true);
        feedbackMarks[interwaveQuestions].GetComponent<Image>().sprite = checkMark;
        audioSource.clip = interwaveCorrectSounds[interwaveQuestions];
        audioSource.Play();
        //if (interwaveQuestions == interwaveQuestionsForWave) {
        interwaveQuestions = -1;
        interwaveQuestionsForWave = 2;
        arrowSupplier.CreateArrowIntermath(8);
        StartCoroutine(delayDeactivateMath());
        //}
        //else m_math.GenerateInterMathQuestion();


    }

    void InterWaveIncorrectFeedback() {
        //feedbackMarks[interwaveQuestions].SetActive(true);
        //feedbackMarks[interwaveQuestions].GetComponent<Image>().sprite = xMark;
        audioSource.clip = incorrectSound;
        audioSource.Play();
        /*switch (interwaveQuestions)
          {
          case 0:
          arrowSupplier.CreateArrowIntermath(2);
          break;
          case 1:
          arrowSupplier.CreateArrowIntermath(4);
          break;
          default:
          break;
          }*/
        interwaveQuestions = -1;
        StartCoroutine(delayDeactivateMath());

    }

    void CorrectFeedback(GameObject[] Feedback) {
        for (int i = 0; i < feedbackTexts.Length; i++) {
            Text FeedbackText = feedbackTexts [i].GetComponent<Text>();
            FeedbackText.text = "Correct";
            FeedbackText.color =  new Color(.188f, .44f, .1f);
            FeedbackText.gameObject.SetActive (true);
        }

        StartCoroutine (DisplayFeedback ());
        arrowSupplier.CreateArrow ();
        audioSource.clip = correctSound;
        audioSource.Play ();
    }

    void IncorrectFeedback(GameObject[] Feedback) {
        for (int i = 0; i < feedbackTexts.Length; i++) {
            Text FeedbackText = feedbackTexts [i].GetComponent<Text>();
            Debug.Log ("Incorrect");
            FeedbackText.text = "Incorrect";
            FeedbackText.color =  new Color(.756f,.278f, .29f);
            FeedbackText.gameObject.SetActive (true);
        }

        StartCoroutine (DisplayFeedback ());
        audioSource.clip = incorrectSound;
        audioSource.Play ();
    }

    public void SetQuestion(string question, int index = 0) {
        /// <summary>
        /// Sets the question display.
        /// </summary>
        /// <param name="question">Question.</param>

        //Debug.Log("SHOULD BE SETTING QUESTION. QUESTIONTEXT LENGTH: " + questionTexts.Length);
        Text QuestionText = questionTexts [index].GetComponent<Text>();
        QuestionText.text = question;   
    }

    public int GetCorrectOfType(System.Type type) {
        return GameStateManager.instance.tracker.GetCorrectOfType (type);
    }

    public int GetIncorrectOfType(System.Type type) {
        return GameStateManager.instance.tracker.GetIncorrectOfType (type);
    }

    IEnumerator DisplayFeedback() {
        yield return new WaitForSeconds (2);
        for (int i = 0; i < feedbackTexts.Length; i++) {
            Text FeedbackText = feedbackTexts [i].GetComponent<Text> ();
            FeedbackText.gameObject.SetActive (false);
        }
    }

    IEnumerator delayDeactivateMath() {
        yield return new WaitForSeconds (.7f);
        MathManager.instance.DeactivateInterMath();
        yield return new WaitForSeconds (1f);
        foreach(GameObject mark in feedbackMarks) {
            mark.SetActive(false);		
        }
    }

}
