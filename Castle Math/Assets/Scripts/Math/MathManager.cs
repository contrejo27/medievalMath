
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathManager : MonoBehaviour {

	public Text QuestionText;
	public Text FeedbackText;
	public GameObject billboard;
	public WaveManager W_man;
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
	Algebra AlgebraQuestion;

	public bool [] QuestionTypes;
	public int IncorrectAnswersPerQuestion;
	private int QuestionType;
	private Question currentQuestion;


	public GameObject mathCanvas;
	public mathController m_Controller;
	public UIEffects interMathCanvas;
	public UIEffects interMathButtons;

	// Use this for initialization
	void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		Multi_Divide = GameObject.FindObjectOfType<MultiplyOrDivide> ();
		Comparision = GameObject.FindObjectOfType<Compare> ();
		Add_Sub = GameObject.FindObjectOfType<AddOrSubtract> ();
		True_False = GameObject.FindObjectOfType<TrueOrFalse> ();
		Fraction = GameObject.FindObjectOfType<Fractions> ();
		AlgebraQuestion = GameObject.FindObjectOfType<Algebra> ();
		m_Controller = GameObject.FindObjectOfType<mathController> ();

		Multi_Divide.Start ();
		Add_Sub.Start ();
		Comparision.Start ();
		True_False.Start ();
		Fraction.Start ();
		AlgebraQuestion.Start ();

		A_Input.Start ();

		QuestionTypes = new bool[6];
		QuestionTypes [0] = m_Controller.add_sub;
		QuestionTypes [1] = m_Controller.mult_divide;
		QuestionTypes [2] = m_Controller.wordProblems;
		QuestionTypes [3] = m_Controller.wordProblems;
		QuestionTypes [4] = m_Controller.fractions;
		QuestionTypes [5] = m_Controller.preAlgebra;
		GenerateProblem (QuestionTypes);
	}

	/// <summary>
	/// Activates the in-between math functionality. Called between waves
	/// </summary>
	public void ActivateInterMath(){
		interwaveMath = true;

		//Generates a fraction question for the interwave math question
		Fraction.GenerateQuestion (-1);//-1 => temp fix
		A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
		currentQuestion = Fraction;

		ActivateBillboard();
		//Check to see if all three questions have been asked
	}

	public void GenerateInterMathQuestion(){

		//Generates a fraction question for the interwave math question
		Fraction.GenerateQuestion (-1);//-1 => temp fix
		A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
		currentQuestion = Fraction;

	}

	/// <summary>
	/// Deactivates the in-between math functionality.
	/// </summary>
	public void DeactivateInterMath(){
		interwaveMath = false;
		//Reset math settings
		QuestionTypes [0] = m_Controller.add_sub;
		QuestionTypes [1] = m_Controller.mult_divide;
		QuestionTypes [2] = m_Controller.wordProblems;
		QuestionTypes [3] = m_Controller.wordProblems;
		QuestionTypes [4] = m_Controller.fractions;
		QuestionTypes [5] = m_Controller.preAlgebra;

		//Debug.Log("AS Difficulty: " + mathDifficultyAorS);
		//Debug.Log("MD Difficulty: " + mathDifficultyMorD);

		GenerateProblem (QuestionTypes);
		DeactivateBillboard();
		W_man.NextWave();
	}

	public void ActivateBillboard(){
		mathCanvas.GetComponent<UIEffects>().fadeOut(1);
		billboard.SetActive(true);
		billboard.GetComponent<Animator> ().Play("show");
		interMathCanvas.fadeIn(1);
		interMathButtons.fadeIn(1);
	}
	
	public void DeactivateBillboard(){
		billboard.GetComponent<Animator> ().Play("hide");
		interMathCanvas.fadeOut(1);
		mathCanvas.GetComponent<UIEffects>().fadeIn(1);
		interMathButtons.fadeOut(1);
	}


	/// <summary>
	/// Sets the math difficulty based on previous performance. Adds correct and incorrect 
	/// answers to generate aggregate score to be used in order to increase difficulty
	/// </summary>
	public void SetDifficulty() {
		int aggregateScoreAorS = A_Input.GetIncorrectOfType(typeof(AddOrSubtract)) + A_Input.GetCorrectOfType(typeof(AddOrSubtract));
		int aggregateScoreMorD = A_Input.GetIncorrectOfType(typeof(MultiplyOrDivide)) + A_Input.GetCorrectOfType(typeof(MultiplyOrDivide));

		mathDifficultyAorS += aggregateScoreAorS;
		mathDifficultyMorD += aggregateScoreMorD;
	}

	/// <summary>
	/// Generates the corresponding problem based on selected question options and a random variable.
	/// </summary>
	/// <param name="QuestionTypes">Question types.</param>
	public void GenerateProblem(bool [] QuestionTypes)
	{
		A_Input.ClearChoices ();
		IncorrectAnswersPerQuestion = 0;

		int randIndex = Random.Range (0, QuestionTypes.Length);
		//QuestionType = QuestionTypes [randIndex];

		/*
		if (totalQuestionsAnswered % 4 == 0) {
			A_Input.ClearChoices ();
			True_False.GenerateQuestion ();
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
		}
		*/

		if (randIndex == 0 && QuestionTypes[0]) {
			Add_Sub.GenerateQuestion (mathDifficultyAorS);
			A_Input.SetCorrectAnswer (Add_Sub.getCorrectAnswer ());
			currentQuestion = Add_Sub;
			//currentQuestion = Add_Sub.GetQuestionString ();
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 1 && QuestionTypes[1]) {
			Multi_Divide.GenerateQuestion (mathDifficultyMorD);
			A_Input.SetCorrectAnswer (Multi_Divide.getCorrectAnswer ());
			currentQuestion = Multi_Divide;
			//currentQuestion = Multi_Divide.GetQuestionString ();
			//Debug.Log (Multi_Divide.GetQuestionString ());
		} else if (randIndex == 2 && QuestionTypes[2]) {
			//Debug.Log (Comparision.GetQuestionString ());
			Comparision.GenerateQuestion (-1); //-1 => temp fix
			A_Input.SetCorrectAnswer (Comparision.getCorrectAnswer ());
			currentQuestion = Comparision;
			//currentQuestion = Comparision.GetQuestionString ();
		} else if (randIndex == 3 && QuestionTypes[3]) {
			True_False.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
			currentQuestion = True_False;
		} else if (randIndex == 4 && QuestionTypes[4]) {
			Fraction.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (Fraction.getCorrectAnswer ());
			currentQuestion = Fraction;
			//currentQuestion = Fraction.GetQuestionString ();
		} else if (randIndex == 5 && QuestionTypes[5]) {
			AlgebraQuestion.GenerateQuestion (mathDifficultyAorS);//-1 => temp fix
			A_Input.SetCorrectAnswer (AlgebraQuestion.getCorrectAnswer ());
			currentQuestion = AlgebraQuestion;
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

	/*
	public int GetQuestionType() {
		return QuestionType;
	}
	*/

	public bool [] GetQuestionTypes() {
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
