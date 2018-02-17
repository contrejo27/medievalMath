using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveMathManager : MonoBehaviour {
	public Text QuestionText;
	public Text FeedbackText;

	public AudioClip CorrectSound;
	public AudioClip IncorrectSound;

	private int ProblemType;
	private int mathDifficulty = 9;
	private string CorrectAnswer;
	private int totalQuestionsAnswered= 0;

	AnswerInput A_Input;

	MultiplyOrDivide Multi_Divide;
	AddOrSubtract Add_Sub;
	Compare Comparision;
	TrueOrFalse True_False;
	Fractions Fraction;

	private int [] QuestionTypes;
	public int IncorrectAnswersPerQuestion;
	private int QuestionType;
	private Question currentQuestion;

	public int FractionQuestions;

	public WaveMathManager () {
	}

	// Use this for initialization
	public void Start () {
		Debug.Log ("Start WM");
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();

		Fraction = GameObject.FindObjectOfType<Fractions> ();

	
		Fraction.Start ();

		A_Input.Start ();

		QuestionTypes = new int[1];
		QuestionTypes [0] = FractionQuestions;
		GenerateProblem (QuestionTypes);
	}

	public void Init() {
		QuestionTypes = new int[1];
		QuestionTypes [0] = FractionQuestions;
		GenerateProblem (QuestionTypes);
	}

	public void GenerateProblem(int [] QuestionTypes)
	{
		A_Input.ClearChoices ();

		int randIndex = Random.Range (0, QuestionTypes.Length);
		QuestionType = QuestionTypes [randIndex];

		IncorrectAnswersPerQuestion = 0;

		/*
		if (totalQuestionsAnswered % 4 == 0) {
			A_Input.ClearChoices ();
			True_False.GenerateQuestion ();
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
		}
		*/

		if (FractionQuestions == 0) {
			Fraction.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
			currentQuestion = Fraction;
			//currentQuestion = Fraction.GetQuestionString ();
		}else {
			this.GenerateProblem (this.GetQuestionTypes ());
		}

		totalQuestionsAnswered++;

	}
	public void increaseMathDifficulty(){
		mathDifficulty++;
	}

	public int GetQuestionType() {
		return QuestionType;
	}

	public int [] GetQuestionTypes() {
		return QuestionTypes;
	}

	public int GetIncorrectAnswersPerQuestion() {
		return IncorrectAnswersPerQuestion;
	}

	public void IncorrectAnswer() {
		IncorrectAnswersPerQuestion++;
	}

	public Question GetCurrentQuestion() {
		return this.currentQuestion;
	}

}
