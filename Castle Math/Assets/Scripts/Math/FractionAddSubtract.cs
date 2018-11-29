using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random=UnityEngine.Random;

// TODO: Convert from OrderOfOperations to FractionAddSubtract

public class FractionAddSubtract : MonoBehaviour, Question {
    public Text questionText;
    public Text questionTextHUD;

    private AnswerInput answerInput;
    private string questionString;
    private int correctAnswer;
    private string[] answerChoices;
    private int difficulty;
    private int[] operands;
    private string[] operators;

    private int incorrectAnswers = 0;
    private int maxInt = 10;

    public FractionAddSubtract() {
        
    }

    public void Start () {
        answerInput = GameObject.FindObjectOfType<AnswerInput>();
        questionText = GameObject.Find("question").GetComponent<Text>();
    }


    public void GenerateQuestion (int maxDifficulty) {
        /// <summary>
        /// Generates order of operations question
        /// </summary>

        this.difficulty = maxDifficulty;
        
        // TODO: Increase number of operands based on difficulty
        int numOfOperands = 0;
        if (maxDifficulty < 13) {
            numOfOperands = 2;
        }
        else if (maxDifficulty < 19) {
            numOfOperands = 3;
        }
        else {
            numOfOperands = 4;
        }
        
        int[] operands = GenerateOperands(maxDifficulty, numOfOperands);
        string[] operators = GenerateOperators(maxDifficulty, numOfOperands);
        string question = GenerateQuestionString(operands, operators);

        this.operands = operands;
        this.operators = operators;
        this.answerChoices = GenerateChoices(operands, operators);

        // Display to Unity
        answerInput.SetQuestion(question);
        answerInput.DisplayChoices(answerChoices);
    }

    private string[] GenerateOperators (int maxDifficulty, int numOfOperands) {
        int numOfOperators = numOfOperands - 1;

        string[] possibleOperators = { "+", "-", "*", "/" };

        string[] operators = new string[numOfOperators];
        for (int i = 0; i < operators.Length; i++) {
            operators[i] = possibleOperators[Random.Range(0, possibleOperators.Length)];
        }

        return operators;
    }

    private int[] GenerateOperands (int maxDifficulty, int numOfOperands) {
        int maxInteger = maxDifficulty;

        if (maxDifficulty > 20) {
            maxInteger = maxDifficulty / 2;
        }
        else {
            maxInteger = maxDifficulty;
        }

        int[] operands = new int[numOfOperands];
        for (int i = 0; i < operands.Length; i++) {
            operands[i] = Random.Range(0, maxInteger);
        }

        return operands;
    }

    private string GenerateQuestionString (int[] operands, string[] operators) {
        string[] parts = new string[operands.Length];

        for (int i = 0; i < parts.Length; i++) {
            if (i == parts.Length - 1) {
                parts[i] = string.Format("{0}", operands[i].ToString());
            }
            parts[i] = string.Format("{0} {1}", operands[i].ToString(), operators[i].ToString());
        }

        return string.Join("", parts);
    }

    public void GenerateChoices () {
        this.answerChoices = GenerateChoices(this.operands, this.operators);
    }

    public string[] GenerateChoices (int[] operands, string[] operators) {
        /// <summary>
        /// Generates the choices.
        /// </summary>

        this.correctAnswer = GenerateAnswer(operands, operators);
        int[] incorrectAnswers = GenerateFakeAnswers(operands, operators, 3);

        int[] integerChoices = new int[4];
        integerChoices[0] = incorrectAnswers[0];
        integerChoices[1] = incorrectAnswers[1];
        integerChoices[2] = incorrectAnswers[2];
        integerChoices[4] = correctAnswer;

        string[] choices = ChoicesToStringArray(ShuffleChoices(integerChoices));

        return choices;
    }

    private int GenerateAnswer (int[] operandArray, string[] operatorArray) {
        /// <summary>
        /// Operates on operands in the correct order. Currently supports */+-
        /// </summary>

        List<int> operands = new List<int>(operandArray);
        List<string> operators = new List<string>(operatorArray);

        // Multiplication and Division
        for (int i = 0; i < operators.Count; i++) {
            if (operators[i] == "*" || operators[i] == "/") {
                int result = ExecuteOperation(operators[i], operands[i], operands[i+1]);
                // Debug.Log(string.Format("i={0}, result={1}", i, result));
                operands[i] = result;
                operators.RemoveAt(i);
                operands.RemoveAt(i+1);
            }
        }

        // Addition and Subtraction
        for (int i = 0; i < operators.Count; i++) {
            if (operators[i] == "+" || operators[i] == "-") {
                int result = ExecuteOperation(operators[i], operands[i], operands[i+1]);
                // Debug.Log(string.Format("i={0}, result={1}", i, result));
                operands[i] = result;
                operators.RemoveAt(i);
                operands.RemoveAt(i+1);
            }
        }

        // Return last element remaining in the operand list
        return operands[0];
    }

    private int[] GenerateFakeAnswers (int[] operands, string[] operators, int num) {
        /// <summary>
        /// Generates vaguely plausible answers.
        /// </summary>
        
        int[] fakeAnswers = new int[num];

        for (int i = 0; i < fakeAnswers.Length; i++) {
            fakeAnswers[i] = GenerateAnswer(ShuffleArray(operands), ShuffleArray(operators));
        }

        return fakeAnswers;
    }

    private int ExecuteOperation(string _operator, int a, int b) {
        /// <summary>
        /// Returns the mathematical result of "a operator b".
        /// </summary>

        switch (_operator) {
            case "+":
                return a + b;
            case "-":
                return a - b;
            case "*":
                return a * b;
            case "/":
                return a / b;
            default:
                Debug.Log(_operator);
                throw new ArgumentException("_operator");
        }
    }

    private class Problem {
        public int[] operands { get; set; }
        public string[] operators { get; set; }
        public int answer { get; set; }

        public Problem (int[] operands, string[] operators, int answer) {
            this.operands = operands;
            this.operators = operators;
            this.answer = answer;
        }

        public string ToString () {
            string problem = "";
            for (int i = 0; i < operands.Length; i++) {
                problem += operands[i].ToString();
                if (i == operands.Length - 1) {
                    break;
                }
                problem += operators[i].ToString();
            }
            return problem;
        }
    }

    private bool TestOrderOfOperations() {
        // testProblems = {"12+3*3", "3*2+4"};
        List<Problem> testProblems = new List<Problem>();
        testProblems.Add(new Problem(new int[] {12,3,3}, new string[] {"+", "*"}, 21));
        testProblems.Add(new Problem(new int[] {3,2,4}, new string[] {"*", "+"}, 10));

        int errors = 0;

        foreach (Problem problem in testProblems) {
            foreach (int operand in problem.operands) {
                // Debug.Log(operand);
            }

            foreach (string _operator in problem.operators) {
                // Debug.Log(_operator);
            }

            int answer = GenerateAnswer(problem.operands, problem.operators);

            if (answer != problem.answer) {
                Debug.Log("WARNING: Test Failed: Order of Operations");
                Debug.Log(string.Format("{0} = {1}, but returned {2}", problem.ToString(), problem.answer, answer));
                errors++;
            }
        }

        if (errors == 0) {
            return true;
        }
        else {
            return false;
        }
    }


    private T[] ShuffleArray<T> (T[] arr) {
        /// <summary>
        /// Shuffles array randomly
        /// </summary>
        
        for (int i = 0; i < arr.Length; i++ ) {
            T temp = arr[i];
            int r = Random.Range(i, arr.Length);
            arr[i] = arr[r];
            arr[r] = temp;
        }

        return arr;
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

        return ShuffleArray(choices);
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
        int gtrValue = this.difficulty;
        if(gtrValue < 3) {
            return "1-6";
        }
        else if (gtrValue < 4) {
            return "7-15";
        }
        else if (gtrValue < 5) {
            return "16-20";
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
