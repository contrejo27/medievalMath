using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderofOperations : MonoBehaviour, Question {
    public Text questionText;
    public Text questionTextHUD;

    private AnswerInput answerInput;
    private string questionString;
    private int correctAnswer;
    private string[] answerChoices;
    // private int[] operands;
    // private string[] operators;

    private int incorrectAnswers = 0;
    private int maxInt = 10;

    public void Start () {
        answerInput = GameObject.FindObjectOfType<AnswerInput>();
        questionText = GameObject.Find("question").GetComponent<Text>();
    }

    public void GenerateQuestion (int maxDifficulty) {
        /// <summary>
        /// Generates order of operations question
        /// </summary>
        
        // Increase number of operands based on difficulty
        int numOfOperands = 0;
        if (maxDifficulty < 13) {
            numOfOperands = 3;
        }
        else if (maxDifficulty < 19) {
            numOfOperands = 4;
        }
        else {
            numOfOperands = 5;
        }
        
        int[] operands = GenerateOperands(maxDifficulty, numOfOperands);
        string[] operators = GenerateOperators(maxDifficulty, numOfOperands);
        string question = GenerateQuestionString(operands, operators);

        this.answerChoices = GenerateChoices(question);

        // Display to Unity
        answerInput.SetQuestion(question);
        answerInput.DisplayChoices(answerChoices);
    }

    public string[] GenerateChoices (string question) {
        /// <summary>
        /// Generates the choices.
        /// </summary>

        correctAnswer = GenerateAnswer(question);
        int[] incorrectAnswers = GenerateFakeAnswers(question, 3);

        int[] integerChoices = new int[4];
        integerChoices[0] = incorrectAnswers[0];
        integerChoices[1] = incorrectAnswers[1];
        integerChoices[2] = incorrectAnswers[2];
        integerChoices[4] = correctAnswer;

        string[] choices = ChoicesToStringArray(ShuffleChoices(integerChoices));

        return choices;
    }

    private int GenerateAnswer (int[] operands, string[] operators) {
        // TODO: Implements Function
    }

    private int GenerateFakeAnswers (int[] operands, string[] operators, int num) {
        // TODO: Implements Function
    }

    private string[] GenerateOperators (int maxDifficulty, int numOfOperands) {
        int numOfOperators = numOfOperands - 1;

        string[] possibleOperators = { "+", "-", "*", "/" };

        string[] operators = new string[numOfOperators];
        foreach (int _operator in operators) {
            _operator = RandomChoice(possibleOperators);
        }

        return operators;
    }

    private int[] GenerateOperands (int maxDifficulty, int numOfOperands) {
        int maxIntMulti;
        int[] operands;

        maxInteger = maxDifficulty;

        if (maxDifficulty > 20) {
            maxInteger = maxDifficulty / 2;
        }
        else {
            maxInteger = maxDifficulty;
        }

        foreach (int operand in operands) {
            operand = Random.Range(0, maxInteger);
        }

        return operands;
    }


    private string[] GenerateQuestionString (int[] operands, string[] operators) {
        List<string> parts = new List<string>();

        for (int i = 0; i < operands.Length; i++) {
            if (i == operands.Length - 1) {
                parts[i] = string.Format("{0}", operands[i].ToString());
            }
            parts[i] = string.Format("{0} {1}", operands[i].ToString(), operators[i].ToString());
        }

        return string.Join("", parts);
    }


    private int[] ShuffleChoices (int[] choices) {
        /// <summary>
        /// Checks for duplicate values and shuffles array before returning.
        /// </summary>

        HashSet<int> choiceSet = new HashSet<int> ();
        int size = choices.Length;

        // Check for duplicate values in array. If found, add a number in a random range
        for (int i = 0; i < size; i++) {
            if (choiceSet.Contains(choices[i])) {
                choices[i] += Random.Range(1, 4);
            }
            choiceSet.Add(choices[i]);
        }

        // Shuffle array randomly
        for (int i = 0; i < choices.Length; i++ ) {
            int temp = choices[i];
            int r = Random.Range(i, choices.Length);
            choices[i] = choices[r];
            choices[r] = temp;
        }

        return choices;
    }

    private T RandomChoice<T> (IEnumerable<T> source) {
        Random rnd = new Random();
        T result = default(T);

        int cnt = 0;
        foreach (T item in source) {
          cnt++;
          if (rnd.Next(cnt) == 0) {
            result = item;
          }
        }

        return result;
    }

    private string[] ChoicesToStringArray(int[] integerChoices) {
        string[] choices = new string[] { integerChoices[0].ToString(), integerChoices[1].ToString(), integerChoices[2].ToString(), integerChoices[3].ToString() };

        return choices;
    }

    public string GetQuestionString () {
        return questionString;
    }

    public string GetCorrectAnswer () {
        return correctAnswer.ToString();
    }

    public void SetCorrectAnswer (string answer) {
        this.correctAnswer = System.Int32.Parse(answer);
    }

    public void SetQuestionString (string question) {
        this.questionString = question;
    }

    public void SetIncorrectAnswers (int incorrect) {
        // TODO: Where is this called from?
        incorrectAnswers = incorrect;
    }

    public int GetIncorrectAnswers () {
        // TODO: Where is this called from?
        return this.incorrectAnswers;
    }

    public string GetQuestionCategory () {
        return "OrderOfOperations";
    }

    public string GetQuestionSubCategory () {
        // TODO: Set difficulty level from largest number in the string
        int gtrValue = Mathf.Max(operands);
        if(gtrValue < 7) {
            return "1-6";
        }
        else if (gtrValue < 13) {
            return "7-12";
        }
        else if (gtrValue < 21) {
            return "13-20";
        }
        else {
            return "20+";
        }
    }

    public bool GetAnsweredCorrectly () {
        return incorrectAnswers == 0;
    }

    public void OnEndQuestion() {

    }

}
