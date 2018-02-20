
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
	private int mathDifficultyAorS = 9;
	private int mathDifficultyMorD = 9;
	private string CorrectAnswer;
	private int totalQuestionsAnswered= 0;
	public bool interwaveMath;
  
	AnswerInput A_Input;

	MultiplyOrDivide Multi_Divide;
	AddOrSubtract Add_Sub;
	Compare Comparision;
	TrueOrFalse True_False;
	Fractions Fraction;

	public int [] QuestionTypes;
	public int IncorrectAnswersPerQuestion;
	private int QuestionType;
	private Question currentQuestion;
	private WaveManager W_Manager;

	public int AddSubQuestions;
	public int MultiDivideQuestions;
	public int CompareQuestions;
	public int TrueFalseQuestions;
	public int FractionQuestions;

	// Use this for initialization
	void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		Multi_Divide = GameObject.FindObjectOfType<MultiplyOrDivide> ();
		Comparision = GameObject.FindObjectOfType<Compare> ();
		Add_Sub = GameObject.FindObjectOfType<AddOrSubtract> ();
		True_False = GameObject.FindObjectOfType<TrueOrFalse> ();
		Fraction = GameObject.FindObjectOfType<Fractions> ();

		Multi_Divide.Start ();
		Add_Sub.Start ();
		Comparision.Start ();
		True_False.Start ();
		Fraction.Start ();

		A_Input.Start ();

		QuestionTypes = new int[5];
		QuestionTypes [0] = AddSubQuestions;
		QuestionTypes [1] = MultiDivideQuestions;
		QuestionTypes [2] = CompareQuestions;
		QuestionTypes [3] = TrueFalseQuestions;
		QuestionTypes [4] = FractionQuestions;
		GenerateProblem (QuestionTypes);
	}

	public void ActivateInterMath(){
		interwaveMath = true;

		for (int i = 0; i < 3; i++) {
			Debug.Log ("In for");
			Fraction.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
			currentQuestion = Fraction;
			totalQuestionsAnswered++;
		}
	}

	public void DeactivateInterMath(){
		QuestionTypes [0] = AddSubQuestions;
		QuestionTypes [1] = MultiDivideQuestions;
		QuestionTypes [2] = CompareQuestions;
		QuestionTypes [3] = TrueFalseQuestions;
		QuestionTypes [4] = FractionQuestions;
		Debug.Log("AS Difficulty: " + mathDifficultyAorS);
		Debug.Log("MD Difficulty: " + mathDifficultyMorD);
		interwaveMath = false;
		GenerateProblem (QuestionTypes);
	}

	public void SetDifficulty() {
		int aggregateScoreAorS = A_Input.GetIncorrectOfType(typeof(AddOrSubtract)) + A_Input.GetCorrectOfType(typeof(AddOrSubtract));
		int aggregateScoreMorD = A_Input.GetIncorrectOfType(typeof(MultiplyOrDivide)) + A_Input.GetCorrectOfType(typeof(MultiplyOrDivide));

		mathDifficultyAorS += aggregateScoreAorS;
		mathDifficultyMorD += aggregateScoreMorD;
	}

	public void GenerateProblem(int [] QuestionTypes)
	{
		A_Input.ClearChoices ();
		IncorrectAnswersPerQuestion = 0;

		int randIndex = Random.Range (0, QuestionTypes.Length);
		QuestionType = QuestionTypes [randIndex];

		/*
		if (totalQuestionsAnswered % 4 == 0) {
			A_Input.ClearChoices ();
			True_False.GenerateQuestion ();
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
		}
		*/

		if (randIndex == 0 && AddSubQuestions != 0) {
			Add_Sub.GenerateQuestion (mathDifficultyAorS);
			A_Input.SetCorrectAnswer (Add_Sub.getCorrectAnswer ());
			currentQuestion = Add_Sub;
			//currentQuestion = Add_Sub.GetQuestionString ();
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 1 && MultiDivideQuestions != 0) {
			Multi_Divide.GenerateQuestion (mathDifficultyMorD);
			A_Input.SetCorrectAnswer (Multi_Divide.getCorrectAnswer ());
			currentQuestion = Multi_Divide;
			//currentQuestion = Multi_Divide.GetQuestionString ();
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 2 && CompareQuestions != 0) {
			//Debug.Log (Comparision.GetQuestionString ());
			Comparision.GenerateQuestion (-1); //-1 => temp fix
			A_Input.SetCorrectAnswer (Comparision.getCorrectAnswer ());
			currentQuestion = Comparision;
			//currentQuestion = Comparision.GetQuestionString ();
		} else if (randIndex == 3 && TrueFalseQuestions != 0) {
			True_False.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
			currentQuestion = True_False;
		} else if (randIndex == 4 && FractionQuestions != 0) {
			Fraction.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
			currentQuestion = Fraction;
			//currentQuestion = Fraction.GetQuestionString ();
		}else {
			this.GenerateProblem (this.GetQuestionTypes ());
		}

		totalQuestionsAnswered++;

	}

	/*
	public void increaseMathDifficulty(){
		mathDifficulty++;
	}
	*/
	
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
