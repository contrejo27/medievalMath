using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// this was created using _MathQuestionTemplate.cs
/// </summary>
public class _MathQuestionTemplate : MonoBehaviour, Question
{
    int correctAnswer;
    string[] answerChoices;
    string questionString;
    int maxInt;
    AnswerInput aInput;
    private int incorrectAnswers = 0;

    public Text questionText;
    public Text questionTextHUD;

    int randomQuestion;

    public static _MathQuestionTemplate instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    public void Start()
    {
        aInput = GameObject.FindObjectOfType<AnswerInput>();
        questionText = GameObject.Find("question").GetComponent<Text>();
    }

    public void GenerateQuestion(int maxDifficulty)
    {
        randomQuestion = Random.Range(0, 2); //0 => Add or subtract, 2 => multiply or divide

        if (randomQuestion == 0)
        {
            string questionString = "templateQuestion";
            //Set textbox display to formatted question string
            //QuestionText.text = QuestionString;
            aInput.SetQuestion(questionString);

            correctAnswer = -1;
            //Generate choices for possible answers 
            GenerateChoices();

        }
        else
        {
            string questionString = "templateQuestion2";
            //Set textbox display to formatted question string
            //QuestionText.text = QuestionString;
            aInput.SetQuestion(questionString);

            correctAnswer = -1;
            //Generate choices for possible answers 
            GenerateChoices();
        }

    }

    public void GenerateChoices()
    {
        //insert choices here
        int Choice1 = correctAnswer - 1;
        int Choice2 = correctAnswer + 1;
        int Choice3 = correctAnswer + Random.Range(2, 5);

        //Array of all possible choices
        int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, correctAnswer };
        answerChoices = ChoicesToStringArray(IntegerChoices);

        aInput.DisplayChoices(answerChoices);
    }

    /// <summary>
    /// Converts the generated Choices integer array to array of strings for later use.
    /// Checks for duplicate values and shuffles array before returning
    /// </summary>
    /// <returns>The string array.</returns>
    /// <param name="IntegerChoices">Integer array of choices.</param>
    public string[] ChoicesToStringArray(int[] IntegerChoices)
    {
        HashSet<int> choiceSet = new HashSet<int>();
        int size = IntegerChoices.Length;

        //Check for duplicate values in array. If found, add a number in a random range
        for (int i = 0; i < size; i++)
        {
            if (choiceSet.Contains(IntegerChoices[i]))
            {
                IntegerChoices[i] += Random.Range(1, 4);
            }
            choiceSet.Add(IntegerChoices[i]);
        }

        //Shuffle array randomly
        for (int i = 0; i < IntegerChoices.Length; i++)
        {
            int temp = IntegerChoices[i];
            int r = Random.Range(i, IntegerChoices.Length);
            IntegerChoices[i] = IntegerChoices[r];
            IntegerChoices[r] = temp;

        }

        //Populate choice array with generated answer choices, converted to strings for later use
        answerChoices = new string[] {IntegerChoices[0].ToString(), IntegerChoices[1].ToString(),
            IntegerChoices[2].ToString(), IntegerChoices[3].ToString()};

        return answerChoices;
    }

    /**Return formatted question string
	 */
    public string GetQuestionString()
    {
        return questionString;
    }

    /**Return formatted answer string
	 */
    public string GetCorrectAnswer()
    {
        return correctAnswer.ToString();
    }

    public void SetCorrectAnswer(string answer)
    {
        this.correctAnswer = System.Int32.Parse(answer); ;
    }

    public void SetQuestionString(string question)
    {
        this.questionString = question;
    }

    public void SetIncorrectAnswers(int incorrect)
    {
        incorrectAnswers = incorrect;
    }

    public int GetIncorrectAnswers()
    {
        return this.incorrectAnswers;
    }

    public string GetQuestionCategory()
    {
        return "FactFamilies";
    }

    public string GetQuestionSubCategory()
    {
        return "temp";
    }

    public bool GetAnsweredCorrectly()
    {
        return incorrectAnswers == 0;
    }

    public void OnEndQuestion()
    {

    }

}
