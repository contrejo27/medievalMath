using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FractionTargets : MonoBehaviour, Question
{
    private AnswerInput A_Input;

    public Text QuestionText;
    public Text QuestionText_hud;
    public GameObject flipperPrefab;

    public float flipperSpacing = .2f;
    public int flipperSize = 1;
    private int numerator;
    private int denominator;
    private int targetFlips;
    [HideInInspector]
    public int currentFlips = 0;

    private double DecimalAnswer;
    private string StringAnswer;
    private int incorrectAnswers;

    private List<MathFlipper> flippers = new List<MathFlipper>();
    int xDim;
    int yDim;

    // Use this for initialization
    public void Start()
    {
        A_Input = GameObject.FindObjectOfType<AnswerInput>();
        QuestionText = GameObject.Find("question").GetComponent<Text>();
    }

    /// <summary>
    /// Generates the question.
    /// </summary>
    /// <param name="maxDifficulty">maximum end of range</param>
    public void GenerateQuestion(int maxDifficulty)
    { //int maxDifficulty => temp fix
        xDim = Random.Range(2, 6);
        yDim = Random.Range(2, 6);

        // Maybe consider trying to avoid the 35/36 situation
        int initDenom = xDim * yDim;
        int initNum = Random.Range(1, (initDenom > 16) ? 16 : initDenom);
        int maxFactor = 1;

        List<int> mutualFactors = new List<int>();
        mutualFactors.Add(1);
        // there are better ways to do this, but we're maxing out at like
        // O(35) here, so this should be fine.
        for(int i = 2; i<initNum+1; i++)
        {
            if (initDenom % i == 0 && initNum % i == 0)
            {
                mutualFactors.Add(i);
                maxFactor = i;
            }
        }

        int factor = mutualFactors[Random.Range(0, mutualFactors.Count)];
        numerator = initNum / factor;
        denominator = initDenom / factor;
        targetFlips = initNum;
        
        QuestionText.text = (Random.Range(0f,1f) > .5f && initDenom / maxFactor <= 10) 
                ? "Fill in " + ((float)numerator / denominator).ToString() + " of the squares by shooting them!"
                : "Fill in " + numerator.ToString() + "/" + denominator.ToString() + " of the squares by shooting them!";

        DecimalAnswer = (double)numerator / (double)denominator;
        StringAnswer = numerator.ToString() + "/" + denominator.ToString();
        Debug.Log("StringAnswer: " + StringAnswer);
        GenerateChoices();
        DisplayItems();
    }
    /// <summary>
    /// Method to generate choices for corresponding fraction classes
    /// </summary>
    public void GenerateChoices()
    {
        string[] AnswerChoices = new string[4];
        AnswerChoices[0] = "Submit";
        AnswerChoices[1] = "";
        AnswerChoices[2] = "";
        AnswerChoices[3] = "";
        
        A_Input.DisplayChoices(AnswerChoices);

    }
    /// <summary>
    /// Displays gem item graphics
    /// </summary>
    void DisplayItems()
    {
        if (flippers.Count > 0)
        {
            this.DeleteFlippers();
        }
        
        //Find the InterMath billboard in the scene
        GameObject billboard = GameObject.Find("MathCanvas_Billboard");

        Vector3 billboardPos = billboard.transform.position;
        Vector3 flipperStartPoint = billboardPos + new Vector3(-((flipperSpacing + flipperSize) * (xDim - 1) / 2), ((flipperSpacing + flipperSize) * (yDim - 1) / 2) + 17, 0.5f);

        for (int i = 0; i<yDim; i++)
        {
            for(int j = 0; j<xDim; j++)
            {
                GameObject f = Instantiate<GameObject>(flipperPrefab, flipperStartPoint + new Vector3(j * (flipperSize + flipperSpacing), -i * (flipperSize + flipperSpacing), 0), Quaternion.identity);
                flippers.Add(f.GetComponent<MathFlipper>());
                f.GetComponent<MathFlipper>().fractionTargets = this;
            }
        }

    }

    public void IncrementFlips()
    {
        ++currentFlips;
        A_Input.SetCorrectAnswer(GetCorrectAnswer());
    }

    public void DecrementFlips()
    {
        --currentFlips;
        A_Input.SetCorrectAnswer(GetCorrectAnswer());
    }

    /// <summary>
    /// Removes gems from scene
    /// </summary>
    private void DeleteFlippers()
    {
        for (int i = 0; i < flippers.Count; i++)
        {
            Destroy(flippers[i].gameObject);
        }
        flippers.Clear();
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
        return (currentFlips == targetFlips) ? "Submit" : "doot";
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

    /// <summary>
    /// Gets the correct answer in decimal form.
    /// </summary>
    /// <returns>The correct decimal answer.</returns>
    private double GetCorrectDecimalAnswer()
    {
        return this.DecimalAnswer;
    }

    /// <summary>
    /// Gets the numerator.
    /// </summary>
    /// <returns>The numerator.</returns>
    private int GetNumerator()
    {
        return this.numerator;
    }

    private int GetDenominator()
    {
        return this.denominator;
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
        return "Flippers";
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
        DeleteFlippers();
    }

}