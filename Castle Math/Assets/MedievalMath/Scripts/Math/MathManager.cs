using System.Collections.Generic;
using UnityEngine;

public class MathManager : MonoBehaviour
{
    private int mathDifficultyAorS = 6;
    private int mathDifficultyMorD = 5;
    public int totalQuestionsAnswered = 0;
    private int maxDifficultyIncrease = 3;

    AnswerInput A_Input;

    MultiplyOrDivide multOrDiv;
    AddOrSubtract addOrSub;
    // Compare Comparision;
    // TrueOrFalse True_False;
    Fractions fractions;
    OrderOfOperations orderOfOperations;
    Algebra algebraQuestion;
    FactFamilies factFamilies;
    WordProblem wordProblem;
    FractionTargets fractionTargets;

    //MathPuzzlePrefabs
    [Header("Math Puzzle Prefabs")]
    public GameObject NumberLinePrefab;

    [Header("Math Functionality")]
    public bool[] QuestionTypes;
    public List<int> intermathQTypeOptions;
    public int IncorrectAnswersPerQuestion;
    public int QuestionType;
    public Question currentQuestion;
    public GameData gameData;
    public bool interwaveMath;

    [Header("Math UI")]
    public GameObject mathCanvas;
    public UIEffects interMathCanvas;
    public UIEffects interMathButtons;
    public GameObject billboard;
    public AudioClip CorrectSound;
    public AudioClip IncorrectSound;
    public GameObject billboardCenter;
    public static MathManager instance;

    List<MathController.MathType> currentQuestionTypes = new List<MathController.MathType>();

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

    void Start()
    {
        //m_telemetry = GameObject.FindObjectOfType<TelemetryManager>();
        A_Input = GetComponent<AnswerInput>();
        multOrDiv = GetComponent<MultiplyOrDivide>();
        // Comparision = GameObject.FindObjectOfType<Compare> ();
        addOrSub = GetComponent<AddOrSubtract>();
        // True_False = GameObject.FindObjectOfType<TrueOrFalse> ();
        fractions = GetComponent<Fractions>();
        algebraQuestion = GetComponent<Algebra>();
        factFamilies = GetComponent<FactFamilies>();
        orderOfOperations = GetComponent<OrderOfOperations>();

        gameData = GameObject.FindObjectOfType<GameData>();

        wordProblem = GetComponent<WordProblem>();
        fractionTargets = GetComponent<FractionTargets>();

        multOrDiv.Start();
        addOrSub.Start();
        // Comparision.Start ();
        // True_False.Start ();
        fractions.Start();
        algebraQuestion.Start();

        QuestionTypes = new bool[4];

        GenerateProblem();
    }

    /*
    void InitializeQuestionType()
    {
        //Waits to receive API mode
        //while(gameData.gameRound.mode == "")
        //    yield return null;
        //yield return new WaitForSeconds(1.0f);

        if (MathController.instance != null)
        {
            QuestionTypes[0] = MathController.instance.add_sub;

            if (MathController.instance.add_sub)
                intermathQTypeOptions.Add(0);
            QuestionTypes[1] = MathController.instance.mult_divide;
            if (MathController.instance.mult_divide)
                intermathQTypeOptions.Add(1);
            // QuestionTypes [2] = MathController.instance.wordProblems;
            // QuestionTypes [3] = MathController.instance.wordProblems;
            QuestionTypes[2] = MathController.instance.fractions;
            if (MathController.instance.fractions)
                intermathQTypeOptions.Add(2);
            QuestionTypes[3] = MathController.instance.preAlgebra;
            if (MathController.instance.preAlgebra)
                intermathQTypeOptions.Add(3);
        }
        else if (gameData)
        {
            //The mode has to be exactly as the strings provided, or make this into an enum
            switch (gameData.gameRound.mode)
            {
                case "Add/Subtract":
                    Debug.Log("It is an add/sub problem");
                    QuestionType = 0;
                    break;
                case "Multiply/Divide":
                    Debug.Log("It is mult/div problem");
                    QuestionType = 1;
                    break;
                case "Fractions":
                    Debug.Log("It is a fraction problem");
                    QuestionType = 2;
                    break;
                case "PreAlgebra":
                    Debug.Log("It is a pre alg problem");
                    QuestionType = 3;
                    break;
                default:
                    Debug.Log("Not known which problem");
                    break;
            }

            intermathQTypeOptions.Add(QuestionType);

            for (int i = 0; i < 4; i++)
            {
                if (i != QuestionType)
                    QuestionTypes[i] = false;
                else
                    QuestionTypes[i] = true;

            }
        }
        else
        {
            QuestionTypes[0] = true;
            QuestionTypes[1] = false;
            QuestionTypes[2] = false;
            QuestionTypes[3] = false;
            intermathQTypeOptions.Add(0);
        }


        GenerateProblem(QuestionTypes);
    }*/


    public void ActivateInterMath()
    {
        /// <summary>
        /// Activates the in-between math functionality. Called between waves
        /// </summary>

        interwaveMath = true;
        GenerateInterMathQuestion();
        ActivateBillboard();

    }

    public void GenerateInterMathQuestion()
    {

        MathController.MathType selectedMath = currentQuestionTypes[Random.Range(0, currentQuestionTypes.Count)];
        switch (selectedMath.questionCategory)
        {
            case EnumManager.QuestionCategories.AddOrSubtract:
                Instantiate(NumberLinePrefab, billboardCenter.transform);
                break;

            case EnumManager.QuestionCategories.MultiplyOrDivide:
                Instantiate(NumberLinePrefab, billboardCenter.transform);
                break;

            case EnumManager.QuestionCategories.Fractions:
                Instantiate(NumberLinePrefab, billboardCenter.transform);
                break;

            case EnumManager.QuestionCategories.Algebra:
                Instantiate(NumberLinePrefab, billboardCenter.transform);
                break;

            case EnumManager.QuestionCategories.FactFamilies:
                Instantiate(NumberLinePrefab, billboardCenter.transform);
                break;

            default:
                Debug.LogError("Error: No MathType Found");
                break;
        }
        

    }

    private void GenerateQuestionForInterMath(Question q)
    {
        /// <summary>
        /// Generates a fraction question for the interwave math question
        /// </summary>

        // TODO: Remove hotfix
        q.GenerateQuestion(-1); //-1 => temp fix
        A_Input.SetCorrectAnswer(q.GetCorrectAnswer());
        currentQuestion = q;
    }

    public void DeactivateInterMath()
    {
        /// <summary>
        /// Deactivates the in-between math functionality.
        /// </summary>

        interwaveMath = false;

        GenerateProblem();
        DeactivateBillboard();
    }

    public void ActivateBillboard()
    {
        mathCanvas.GetComponent<UIEffects>().fadeOut(1);
        billboard.SetActive(true);
        billboard.GetComponent<Animator>().Play("show");
        interMathCanvas.fadeIn(1);
        interMathButtons.fadeIn(1);
    }

    public void DeactivateBillboard()
    {
        billboard.GetComponent<Animator>().Play("hide");
        interMathCanvas.fadeOut(1);
        mathCanvas.GetComponent<UIEffects>().fadeIn(1);
        interMathButtons.fadeOut(1);
    }

    public void SetDifficulty()
    {
        /// <summary>
        /// Sets the math difficulty based on previous performance. Adds correct and incorrect
        /// answers to generate aggregate score to be used in order to increase difficulty
        /// </summary>

        int aggregateScoreAorS = A_Input.GetIncorrectOfType(typeof(AddOrSubtract)) + A_Input.GetCorrectOfType(typeof(AddOrSubtract));
        int aggregateScoreMorD = A_Input.GetIncorrectOfType(typeof(MultiplyOrDivide)) + A_Input.GetCorrectOfType(typeof(MultiplyOrDivide));

        int increaseAorS;
        int increaseMorD;

        // Don't increase difficulty beyond set point
        if (aggregateScoreAorS > maxDifficultyIncrease)
        {
            increaseAorS = maxDifficultyIncrease;
        }
        else
        {
            increaseAorS = aggregateScoreAorS;
        }

        // Don't increase difficulty beyond set point
        if (aggregateScoreMorD > maxDifficultyIncrease - 2)
        {
            increaseMorD = maxDifficultyIncrease - 2;
        }
        else
        {
            increaseMorD = aggregateScoreMorD;
        }

        // Check to see if difficulty will fall below zero, else reset to default value
        if (mathDifficultyAorS + increaseAorS > 0)
        {
            mathDifficultyAorS += increaseAorS;
        }
        else
        {
            mathDifficultyAorS = 5;
        }

        if (mathDifficultyMorD + increaseMorD > 0)
        {
            mathDifficultyMorD += increaseMorD;
        }
        else
        {
            mathDifficultyMorD = 5;
        }
    }

    public void GenerateProblem()
    {
        /// <summary>
        /// Generates the corresponding problem based on selected question options and a random variable.
        /// </summary>
        /// <param name="QuestionTypes">Question types.</param>

        //print("questionTypesActivated:");
        /* foreach(bool questionT in QuestionTypes) {
            print(questionT);
        }*/

        A_Input.ClearChoices();
        IncorrectAnswersPerQuestion = 0;


        // find the currently selected question types and put indices in list

        foreach (MathController.MathType question in MathController.instance.mathList)
        {
            if (question.isEnabled)
            {
                currentQuestionTypes.Add(question);
            }
        }

        //if we haven't initialized the mathlist because we start mid program add a mathType to list
        if (currentQuestionTypes == null || currentQuestionTypes.Count == 0)
        {
            currentQuestionTypes.Add( new MathController.MathType("factFamilies", true, EnumManager.QuestionCategories.FactFamilies));
        }

        MathController.MathType selectedMath = currentQuestionTypes[Random.Range(0, currentQuestionTypes.Count)];
        switch (selectedMath.questionCategory)
        {
            case EnumManager.QuestionCategories.AddOrSubtract:
                addOrSub.GenerateQuestion(mathDifficultyAorS);
                A_Input.SetCorrectAnswer(addOrSub.GetCorrectAnswer());
                currentQuestion = addOrSub;
                break;

            case EnumManager.QuestionCategories.MultiplyOrDivide:
                multOrDiv.GenerateQuestion(mathDifficultyMorD);
                A_Input.SetCorrectAnswer(multOrDiv.GetCorrectAnswer());
                currentQuestion = multOrDiv;
                break;

            case EnumManager.QuestionCategories.Fractions:
                // TODO: Remove hot fix
                fractions.GenerateQuestion(-1);//-1 => temp fix
                A_Input.SetCorrectAnswer(fractions.GetCorrectAnswer());
                currentQuestion = fractions;
                break;
            case EnumManager.QuestionCategories.Algebra:
                algebraQuestion.GenerateQuestion(mathDifficultyAorS);
                A_Input.SetCorrectAnswer(algebraQuestion.GetCorrectAnswer());
                currentQuestion = algebraQuestion;
                break;
            case EnumManager.QuestionCategories.FactFamilies:
                FactFamilies.instance.GenerateQuestion(mathDifficultyAorS);
                A_Input.SetCorrectAnswer(FactFamilies.instance.GetCorrectAnswer());
                currentQuestion = algebraQuestion;
                break;

            default:
                Debug.LogError("Error: No MathType Found");
                break;
        }

        Debug.Log(currentQuestion.GetQuestionString());
        //gameData.gameResponse.solution = A_Input.GetCorrectAnswer();
        //gameData.gameResponse.question = A_Input.currentQuestion;
        //m_telemetry.LogResponse();

        totalQuestionsAnswered++;
        //m_telemetry.LogRound();
    }

    /*
    public void increaseMathDifficulty() {
        mathDifficulty++;
    }
    */

    /*
    public int GetQuestionType() {
        return QuestionType;
    }
    */

    public bool[] GetQuestionTypes()
    {
        return QuestionTypes;
    }

    public int GetIncorrectAnswersPerQuestion()
    {
        return IncorrectAnswersPerQuestion;
    }

    public void IncorrectAnswer()
    {
        IncorrectAnswersPerQuestion++;
    }

    public Question GetCurrentQuestion()
    {
        Debug.Log("IsCurrentQuestion Null?: " + (currentQuestion == null));
        return this.currentQuestion;
    }
}
