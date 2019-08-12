﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// this was created using _MathQuestionTemplate.cs
/// </summary>
public class FactFamilies : MonoBehaviour, Question
{
    int correctAnswer;
    string[] answerChoices;
    string questionString;
    int maxInt;
    AnswerInput aInput;
    private int incorrectAnswers = 0;
    string currentMathType;

    public Text questionText;
    public Text questionTextHUD;

    int randomQuestion;

    public static FactFamilies instance;

    //list of all factFamilies
    string[] factFamilyArray;

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
        factFamilyArray = UtilityFunctions.instance.ReadDoc("Math\\FactFamilies");

        aInput = GameObject.FindObjectOfType<AnswerInput>();
        questionText = GameObject.Find("question").GetComponent<Text>();
    }

    public void GenerateQuestion(int maxDifficulty)
    {
        //NEXT STEP: check if this prints a number family then figure out how to make the right math out of it.
        //list of numbers in family
        int[] fFamilyNums = GetFactFamily();
        foreach(int num in fFamilyNums)
        {
            print (num);
        }

        randomQuestion = Random.Range(0, 2); //1 => Add, 2 => subtract

        if (randomQuestion == 0)
        {
            string questionString = "templateQuestion";

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

    int[] GetFactFamily()
    {
        //for current testing we'll use second grade.
        int currentGrade = 2;
        List<int[]> currentGradeFams = new List<int[]>();

        //the foreach loop proceeds to move through the text file found earlier'
        foreach (string a in factFamilyArray)
        {
            //splits invidiual data based on the presence of a comma
            string[] splitLine = a.Split(',');
            if (!splitLine[0].StartsWith("*"))
            {
                //if it's the correct grade put it in a list
                if(int.Parse(splitLine[1]) == currentGrade)
                {
                    int[] currentFam = new int[] { int.Parse(splitLine[2]), int.Parse(splitLine[3]), int.Parse(splitLine[4]) };
                    currentGradeFams.Add(currentFam);
                }
                currentMathType = splitLine[0];
                
            }

        }
        //pick random family from list
        return currentGradeFams[Random.Range(0, currentGradeFams.Count)];
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