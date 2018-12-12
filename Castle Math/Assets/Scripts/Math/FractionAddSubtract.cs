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
    private Rational correctAnswer;
    private string[] answerChoices;
    private int difficulty;
    private Rational[] operands;
    private string[] operators;

    private int incorrectAnswers = 0;
    private int maxInt = 10;

    public FractionAddSubtract () {
        
    }

    public void Start () {
        answerInput = GameObject.FindObjectOfType<AnswerInput>();
        questionText = GameObject.Find("question").GetComponent<Text>();
    }


    public void GenerateQuestion (int maxDifficulty) {
        /// <summary>
        /// Generates order of operations question
        /// </summary>

        //this.difficulty = maxDifficulty;
		this.difficulty = 2;
        int numOfOperands = 0;

		if (maxDifficulty < 15) {
			numOfOperands = 2;
		} else if (maxDifficulty < 19) {
			numOfOperands = 3;
		} else if (maxDifficulty <= 0) {
			difficulty = 1;
		}
        else {
            numOfOperands = 4;
        }

		Rational[] operands = GenerateOperands(difficulty, numOfOperands);
		//Debug.Log ("Rational operands: " + operands[0]);
		string[] operators = GenerateOperators(difficulty, numOfOperands - 1);
		//Debug.Log ("string  operator: " + operators[0]);

        this.questionString = GenerateQuestionString(operands, operators);
        this.answerChoices = GenerateChoices(operands, operators);

        // Display to Unity
        answerInput.SetQuestion(this.questionString);
        answerInput.DisplayChoices(this.answerChoices);
		//Debug.Log("Corrected Answer: " + answerInput.GetCorrectAnswer ());
    }

    private string[] GenerateOperators (int maxDifficulty, int numOfOperands) {

        string[] possibleOperators = { "+", "-" };

		string[] operators = new string[numOfOperands];
        for (int i = 0; i < operators.Length; i++) {
            operators[i] = possibleOperators[Random.Range(0, possibleOperators.Length)];
        }

        return operators;
    }

    private Rational[] GenerateOperands (int maxDifficulty, int numOfOperands) {
		int maxInteger = difficulty;

        if (maxDifficulty > 20) {
			maxInteger = maxDifficulty / 2;
        }
        else {
			maxInteger = maxDifficulty;
        }

        Rational[] operands = new Rational[numOfOperands];
        for (int i = 0; i < operands.Length; i++) {
			int num = Random.Range(0, 5);
			int den = Random.Range(0, 5);
			while (den == 0) {
				den = Random.Range(0, 5);
			}
            
            operands[i] = new Rational(num, den);
        }

        return operands;
    }

    private string GenerateQuestionString (Rational[] operands, string[] operators) {
        string[] parts = new string[operands.Length];

        for (int i = 0; i < parts.Length; i++) {
            if (i == parts.Length-1) {
                parts[i] = string.Format("{0}", operands[i].ToString());
				break;
            }
            parts[i] = string.Format("{0} {1}", operands[i].ToString(), operators[i].ToString());
        }

        return string.Join("", parts);
    }

    public void GenerateChoices () {
        this.answerChoices = GenerateChoices(this.operands, this.operators);
    }

    public string[] GenerateChoices (Rational[] operands, string[] operators) {
        /// <summary>
        /// Generates the choices.
        /// </summary>

        correctAnswer = GenerateAnswer(operands, operators);
		correctAnswer.ans = true;

        Rational[] fakeAnswers = GenerateFakeAnswers(operands, operators, 3);

        Rational[] choices = new Rational[fakeAnswers.Length + 1];
        for (int i = 0; i < fakeAnswers.Length; i++) {
            choices[i] = fakeAnswers[i];
        }
        choices[choices.Length-1] = correctAnswer;

        string[] stringChoices = ArrayToStringArray(ShuffleChoices(choices));

        return stringChoices;
    }

    private Rational GenerateAnswer (Rational[] operandArray, string[] operatorArray) {
        /// <summary>
        /// Operates on operands in the correct order. Currently supports */+-
        /// </summary>
		Rational result = new Rational (0, 1);
        List<Rational> operands = new List<Rational>(operandArray);
        List<string> operators = new List<string>(operatorArray);

        // Addition & Subtraction
		for (int i = 0; i < operators.Count; i++) {
			if (operators [i] == "-" || operators [i] == "+") {
				result = ExecuteOperation (operators [i], operands [i], operands [i + 1]);
			}
        }

        // Return last element remaining in the operand list
        return result;

    }

    private Rational[] GenerateFakeAnswers (Rational[] operands, string[] operators, int num) {
        /// <summary>
        /// Generates vaguely plausible answers.
        /// </summary>
        
        Rational[] fakeAnswers = new Rational[num];

        for (int i = 0; i < fakeAnswers.Length; i++) {
            fakeAnswers[i] = GenerateAnswer(ShuffleArray(operands), ShuffleArray(operators));
        }

        return fakeAnswers;
    }

    private Rational ExecuteOperation(string _operator, Rational a, Rational b) {
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

    private Rational[] ShuffleChoices (Rational[] choices) {
        /// <summary>
        /// Checks for duplicate values and shuffles array before returning.
        /// </summary>
        
        HashSet<Rational> choiceSet = new HashSet<Rational> ();
        int size = choices.Length;

        // Check for duplicate values in array. If found, add a number in a random range
        for (int i = 0; i < size; i++) {
            // TODO: Change this function to a <T> generic function, with objects that shuffle themselves
			if (!choices [i].ans) {
				bool isDupe = true;
				while (isDupe) {
					
					for (int j = 0; j < choices.Length; j++) {
						if (choices [i] == choices [j] && i != j) {
							isDupe = true;
							break;
						} else {
							isDupe = false;
						}
					}
					if (i % 2 == 0) {
						choices [i].num = choices [i].num + Random.Range (1, 4);
					} else {
						choices [i].num = correctAnswer.num + Random.Range (1, 4);
						choices [i].den = correctAnswer.den + Random.Range (1, 4);
						if (choices [i].den == 0) {
							choices [i].den = correctAnswer.den + Random.Range (1, 4);
						}
					}
				}
			}
        }

        return ShuffleArray(choices);
    }

    private string[] ArrayToStringArray<T> (T[] arr) {
        string[] output = new string[arr.Length];

        for (int i = 0; i < output.Length; i++ ) {
            output[i] = arr[i].ToString();
        }

        return output;
    }

    public string GetQuestionString () {
        return questionString;
    }

    public string GetCorrectAnswer () {
		Debug.Log ("Correct Answer: " + correctAnswer.ToString ());
        return correctAnswer.ToString();
    }

    public void SetCorrectAnswer (string answer) {
        // this.correctAnswer = System.Int32.Parse(answer);
		//this.correctAnswer = answer;
    }

    public void SetQuestionString (string question) {
        this.questionString = question;
    }

    public void SetIncorrectAnswers (int incorrect) {
        incorrectAnswers = incorrect;
    }

    public int GetIncorrectAnswers () {
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
