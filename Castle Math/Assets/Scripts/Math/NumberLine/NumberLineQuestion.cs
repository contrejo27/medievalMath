using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberLineQuestion : MonoBehaviour, Question {

    private AnswerInput A_Input;

    public Text QuestionText;
    public Text QuestionText_hud;
    // This is messy, but it's honestly easier than calculating the 
    // Billboard's off-center position.
    public Transform billboardCenter;
    public NumberLineManager nlm;
    
    private int lowerBound;
    private int upperBound;

    public int maxAttempts;
    int currentAttempts;

    private int intAnswer;
    private string StringAnswer;
    private int incorrectAnswers;

    // Use this for initialization
    public void Start()
    {
        A_Input = GameObject.FindObjectOfType<AnswerInput>();
        //QuestionText = GameObject.Find("question").GetComponent<Text>();
    }

    /// <summary>
    /// Generates the question.
    /// </summary>
    /// <param name="maxDifficulty">maximum end of range</param>
    public void GenerateQuestion(int maxDifficulty)
    { //int maxDifficulty => temp fix

        lowerBound = Random.Range(-20, 1);
        upperBound = lowerBound + 20;
        intAnswer = Random.Range(lowerBound, upperBound + 1);
        A_Input.interwaveQuestionsForWave = 0;

        int initPos;
        if (intAnswer - 5 <= lowerBound)
            initPos = Random.Range(intAnswer + 5, upperBound);
        else if (intAnswer + 5 >= upperBound)
            initPos = Random.Range(lowerBound, intAnswer - 5);
        else
        {
            if(Random.Range(0,1f) > .5f) 
                initPos = Random.Range(lowerBound, intAnswer - 5);
            else
                initPos = Random.Range(intAnswer + 5, upperBound);
        }


        QuestionText.text = "Move the slider to " + intAnswer + " by shooting targets to add the value!";

        nlm.gameObject.SetActive(true);
        nlm.SpawnTargets(lowerBound, initPos - lowerBound);
        nlm.maxAttempts = maxAttempts;
        nlm.targetValue = intAnswer;


        // StringAnswer = numerator.ToString() + "/" + denominator.ToString();
        GenerateChoices();
    }

    public void CheckAnswer(int currentPos, bool maxAttemptsSurpased)
    {
        Debug.Log("target: " + intAnswer + ", current: " + currentPos);
        if(currentPos == intAnswer)
        {
            A_Input.OnCorrect();
            return;
        }

        if(maxAttemptsSurpased)
        {
            SetIncorrectAnswers(3);
            A_Input.OnIncorrect();
        }

        
    }

    /// <summary>
    /// Method to generate choices for corresponding fraction classes
    /// </summary>
    public void GenerateChoices()
    {
        string[] AnswerChoices = {"","","",""};

        A_Input.DisplayChoices(AnswerChoices);

    }
    

    /// <summary>
    /// Gets the question string.
    /// </summary>
    /// <returns>The question string.</returns>
    public string GetQuestionString()
    {

        return QuestionText.text;
    }

    /// <summary>
    /// Gets the correct answer.
    /// </summary>
    /// <returns>The correct answer.</returns>
    public string GetCorrectAnswer()
    {
        return "doot";
    }

    /// <summary>
    /// Sets the correct answer.
    /// </summary>
    /// <param name="answer">Answer.</param>
    public void SetCorrectAnswer(string answer)
    {
        this.StringAnswer = answer;
    }

    /// <summary>
    /// Sets the question string.
    /// </summary>
    /// <param name="question">Question.</param>
    public void SetQuestionString(string question)
    {
        this.StringAnswer = question;
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
        return "Number Line";
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
        nlm.gameObject.SetActive(false);
        incorrectAnswers = 0;
    }
}
