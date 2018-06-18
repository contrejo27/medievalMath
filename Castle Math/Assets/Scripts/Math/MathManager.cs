
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

	//private int ProblemType;
	private int mathDifficultyAorS = 6;
	private int mathDifficultyMorD = 5;
	//private string CorrectAnswer;
	private int totalQuestionsAnswered= 0;
	private int maxDifficultyIncrease = 3;
	public bool interwaveMath;
  
	AnswerInput A_Input;

	MultiplyOrDivide multOrDiv;
	AddOrSubtract addOrSub;
	//Compare Comparision;
	//TrueOrFalse True_False;
	Fractions fractions;
	Algebra algebraQuestion;
    WordProblem wordProblem;
    FractionTargets fractionTargets;

	public bool [] QuestionTypes;
    public List<int> intermathQTypeOptions;
	public int IncorrectAnswersPerQuestion;
	private int QuestionType;
	private Question currentQuestion;


	public GameObject mathCanvas;
	public MathController m_Controller;
	public UIEffects interMathCanvas;
	public UIEffects interMathButtons;

    public static MathManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        GameStateManager.instance.mathManager = this;

		A_Input = GetComponent<AnswerInput> ();
		multOrDiv = GetComponent<MultiplyOrDivide> ();
		//Comparision = GameObject.FindObjectOfType<Compare> ();
		addOrSub = GetComponent<AddOrSubtract> ();
	//	True_False = GameObject.FindObjectOfType<TrueOrFalse> ();
		fractions = GetComponent<Fractions> ();
		algebraQuestion = GetComponent<Algebra> ();
		m_Controller = GetComponent<MathController> ();
        wordProblem = GetComponent<WordProblem>();
        fractionTargets = GetComponent<FractionTargets>();

		multOrDiv.Start ();
		addOrSub.Start ();
	//	Comparision.Start ();
	//	True_False.Start ();
		fractions.Start ();
		algebraQuestion.Start ();

		//A_Input.Start ();

		QuestionTypes = new bool[4];
		if(m_Controller != null){
			QuestionTypes [0] = m_Controller.add_sub;
            if (m_Controller.add_sub) intermathQTypeOptions.Add(0);
			QuestionTypes [1] = m_Controller.mult_divide;
            if (m_Controller.mult_divide) intermathQTypeOptions.Add(1);
            //QuestionTypes [2] = m_Controller.wordProblems;
            //QuestionTypes [3] = m_Controller.wordProblems;
            QuestionTypes [2] = m_Controller.fractions;
			QuestionTypes [3] = m_Controller.preAlgebra;
		}
		else{
			QuestionTypes [0] = true;
            intermathQTypeOptions.Add(0);
            QuestionTypes [1] = false;
			QuestionTypes [2] = false;
			QuestionTypes [3] = false;
		}
        
		GenerateProblem (QuestionTypes);
	}

    /// <summary>
    /// Activates the in-between math functionality. Called between waves
    /// </summary>
    public void ActivateInterMath()
    {
        interwaveMath = true;

        GenerateInterMathQuestion();

		ActivateBillboard();
		//Check to see if all three questions have been asked
	}

	public void GenerateInterMathQuestion(){

        switch (intermathQTypeOptions[Random.Range(0, intermathQTypeOptions.Count)])
        {
            case 0:
                Debug.Log("Generating word problem");
                GenerateQuestionForInterMath(fractionTargets);
                //GenerateQuestionForInterMath(wordProblem);
                break;
            case 1:
                //GenerateQuestionForInterMath(fractionTargets);
                GenerateQuestionForInterMath(multOrDiv);
                break;
            default:
                break;
        }
        

	}

    private void GenerateQuestionForInterMath(Question q)
    {
        //Generates a fraction question for the interwave math question
        q.GenerateQuestion(-1);//-1 => temp fix
        A_Input.SetCorrectAnswer(q.GetCorrectAnswer());
        currentQuestion = q;
    }

	/// <summary>
	/// Deactivates the in-between math functionality.
	/// </summary>
	public void DeactivateInterMath(){
		interwaveMath = false;
		//Reset math settings
		if(m_Controller != null){
			QuestionTypes [0] = m_Controller.add_sub;
			QuestionTypes [1] = m_Controller.mult_divide;
			//QuestionTypes [2] = m_Controller.wordProblems;
			//QuestionTypes [3] = m_Controller.wordProblems;
			QuestionTypes [2] = m_Controller.fractions;
			QuestionTypes [3] = m_Controller.preAlgebra;
		}
		else{
			QuestionTypes [0] = true;
			QuestionTypes [1] = false;
			QuestionTypes [2] = false;
			QuestionTypes [3] = false;
		}
        currentQuestion.OnEndQuestion();
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

		int increaseAorS;
		int increaseMorD;

		//Don't increase difficulty beyond set point
		if (aggregateScoreAorS > maxDifficultyIncrease)
			increaseAorS = maxDifficultyIncrease;
		else
			increaseAorS = aggregateScoreAorS;

		//Don't increase difficulty beyond set point
		if (aggregateScoreMorD > maxDifficultyIncrease-2)
			increaseMorD = maxDifficultyIncrease-2;
		else
			increaseMorD = aggregateScoreMorD;

		//Check to see if difficulty will fall below zero, else reset to default value
		if (mathDifficultyAorS + increaseAorS > 0)
			mathDifficultyAorS += increaseAorS;
		else
			mathDifficultyAorS = 5;

		if (mathDifficultyMorD + increaseMorD > 0)
			mathDifficultyMorD += increaseMorD;
		else
			mathDifficultyMorD = 5;
	}

	/// <summary>
	/// Generates the corresponding problem based on selected question options and a random variable.
	/// </summary>
	/// <param name="QuestionTypes">Question types.</param>
	public void GenerateProblem(bool [] QuestionTypes){
        print("questionTypesActivated:");
        foreach(bool questionT in QuestionTypes){
            print(questionT);
        }
		A_Input.ClearChoices ();
		IncorrectAnswersPerQuestion = 0;

        List<int> currentQuestionTypes = new List<int>();

        //finds the currently selected question types and puts indices in list
        int i = 0;
        foreach(bool question in QuestionTypes){
        	if(question){
        		currentQuestionTypes.Add(i);
        	}
        	i++;
        }
 		int selectedMath = currentQuestionTypes[Random.Range (0, currentQuestionTypes.Count)];
		if (selectedMath == 0) {
			addOrSub.GenerateQuestion (mathDifficultyAorS);
			A_Input.SetCorrectAnswer (addOrSub.GetCorrectAnswer ());
			currentQuestion = addOrSub;
		} else if (selectedMath == 1) {
			multOrDiv.GenerateQuestion (mathDifficultyMorD);
			A_Input.SetCorrectAnswer (multOrDiv.GetCorrectAnswer ());
			currentQuestion = multOrDiv;
		}/* else if (selectedMath == 2) {
			Comparision.GenerateQuestion (-1); //-1 => temp fix
			A_Input.SetCorrectAnswer (Comparision.getCorrectAnswer ());
			currentQuestion = Comparision;
		} else if (selectedMath == 3) {
			True_False.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (True_False.getCorrectAnswer ());
			currentQuestion = True_False;
		}*/ else if (selectedMath == 2) {
			fractions.GenerateQuestion (-1);//-1 => temp fix
			A_Input.SetCorrectAnswer (fractions.GetCorrectAnswer ());
			currentQuestion = fractions;
		} else if (selectedMath == 3) {
			algebraQuestion.GenerateQuestion (mathDifficultyAorS);
			A_Input.SetCorrectAnswer (algebraQuestion.GetCorrectAnswer ());
			currentQuestion = algebraQuestion;
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
        Debug.Log("IsCurrentQuestion Null?: " + (currentQuestion == null));
		return this.currentQuestion;
	}

}
