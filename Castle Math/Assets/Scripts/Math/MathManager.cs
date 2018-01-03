using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MathManager : MonoBehaviour {
	public Text QuestionText;
	public Text FeedbackText;

	public AudioClip CorrectSound;
	public AudioClip IncorrectSound;

	int ProblemType;
	int mathDifficulty = 9;
	string CorrectAnswer;
  
	AnswerInput A_Input;

	MultiplyOrDivide Multi_Divide;
	AddOrSubtract Add_Sub;
	Compare Comparision;
	
	public int QuestionType;
	public int IncorrectAnswersPerQuestion;


	// Use this for initialization
	void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		Multi_Divide = GameObject.FindObjectOfType<MultiplyOrDivide> ();
		Comparision = GameObject.FindObjectOfType<Compare> ();
		Add_Sub = GameObject.FindObjectOfType<AddOrSubtract> ();


		Multi_Divide.Start ();
		Add_Sub.Start ();
		Comparision.Start ();

		A_Input.Start ();

		GenerateProblem (QuestionType);
	}


	public void GenerateProblem(int QuestionType)
	{
		IncorrectAnswersPerQuestion = 0;

		//0 = Add or subtract question
		if (QuestionType == 0) {
			Add_Sub.GenerateQuestion (mathDifficulty);
			A_Input.SetCorrectAnswer (Add_Sub.getCorrectAnswer ());
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (QuestionType == 1) {
			Multi_Divide.GenerateQuestion (mathDifficulty);
			A_Input.SetCorrectAnswer (Multi_Divide.getCorrectAnswer ());
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (QuestionType == 2) {
			//Debug.Log (Comparision.GetQuestionString ());
			Comparision.GenerateQuestion ();
			A_Input.SetCorrectAnswer (Comparision.getCorrectAnswer ());
		}

	}
	public void increaseMathDifficulty(){
		mathDifficulty++;
	}
	
	public int GetQuestionType() {
		return QuestionType;
	}

	public int GetIncorrectAnswersPerQuestion() {
		return IncorrectAnswersPerQuestion;
	}

	public void IncorrectAnswer() {
		IncorrectAnswersPerQuestion++;
	}
}
