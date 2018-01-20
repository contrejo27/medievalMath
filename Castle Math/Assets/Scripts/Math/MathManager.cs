using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathManager : MonoBehaviour {
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
	
	private int [] QuestionTypes;
	public int IncorrectAnswersPerQuestion;
	private int QuestionType;

	public int AddSubQuestions;
	public int MultiDivideQuestions;
	public int CompareQuestions;
	public int TrueFalseQuestions;


	// Use this for initialization
	void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		Multi_Divide = GameObject.FindObjectOfType<MultiplyOrDivide> ();
		Comparision = GameObject.FindObjectOfType<Compare> ();
		Add_Sub = GameObject.FindObjectOfType<AddOrSubtract> ();
		True_False = GameObject.FindObjectOfType<TrueOrFalse> ();

		Multi_Divide.Start ();
		Add_Sub.Start ();
		Comparision.Start ();
		True_False.Start ();

		A_Input.Start ();

		QuestionTypes = new int[4];
		QuestionTypes [0] = AddSubQuestions;
		QuestionTypes [1] = MultiDivideQuestions;
		QuestionTypes [2] = CompareQuestions;
		QuestionTypes [3] = TrueFalseQuestions;
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

		if (randIndex == 0 && AddSubQuestions != 0) {
			Add_Sub.GenerateQuestion (mathDifficulty);
			A_Input.SetCorrectAnswer (Add_Sub.getCorrectAnswer ());
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 1 && MultiDivideQuestions != 0) {
			Multi_Divide.GenerateQuestion (mathDifficulty);
			A_Input.SetCorrectAnswer (Multi_Divide.getCorrectAnswer ());
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 2 && CompareQuestions != 0) {
			//Debug.Log (Comparision.GetQuestionString ());
			Comparision.GenerateQuestion ();
			A_Input.SetCorrectAnswer (Comparision.getCorrectAnswer ());
		} else if (randIndex == 3 && TrueFalseQuestions != 0) {
			True_False.GenerateQuestion ();
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
		} else {
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
}
