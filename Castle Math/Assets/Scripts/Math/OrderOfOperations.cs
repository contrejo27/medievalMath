using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderofOperations : MonoBehaviour, Question {

    public Text questionText;
    public Text questionTextHUD;

    private int firstNum;
    private int secondNum;
    private int correctAnswer;
    public int isDivide;
    private int incorrectAnswers = 0;
    private string [] answerChoices;
    private string questionString;
    private int maxInt = 10;
    private AnswerInput answerInput;

    public MultiplyOrDivide() {

    }

    // Use this for initialization
    public void Start () {
        answerInput = GameObject.FindObjectOfType<AnswerInput> ();
        questionText = GameObject.Find ("question").GetComponent<Text>();
    }

    public void GenerateOperands(int maxDifficulty) {
        //Generate random 0 or 1 to determine whether question is to be multiplication or divison
        //isDivide == 0 -> Division problem
        isDivide = Random.Range (0, 2);

        int maxIntMulti;

        if (maxDifficulty > 20) {
            maxIntMulti = maxDifficulty / 2;
            maxInt = maxDifficulty;
        }
        else {
            maxInt = maxDifficulty;
            maxIntMulti = maxDifficulty;
        }

        //check for division
        if (isDivide == 0) {
            firstNum = Random.Range (0, maxInt + (maxInt / 2));
            secondNum = Random.Range (1, maxInt);

            //Ensure that division is even, with no remainders
            while (firstNum % secondNum != 0) {
                firstNum = Random.Range (0, maxInt + (maxInt / 2));
            }

            //Calculate correct answer
            correctAnswer = firstNum / secondNum;
        } 
        else {
            firstNum = Random.Range (0, maxIntMulti);
            secondNum = Random.Range (1, maxIntMulti);

            correctAnswer = firstNum * secondNum;
        }
    }

    public void GenerateQuestion (int maxDifficulty) {
        /// <summary>
        /// Generates either mulitplication or division question by random selection
        /// </summary>
        /// <param name="maxDifficulty">The range of numbers from which to generate the question.</param>
        GenerateOperands (maxDifficulty);

        print("MAX DIFFICULTY  = " + maxDifficulty);
        //check for division
        if (isDivide == 0) {
            //Generate formatted question string and set text box text
            char divSign = '\u00F7';

            questionString = firstNum.ToString () + " " + divSign.ToString() + " " + secondNum.ToString () + " =";
            //QuestionText.text = QuestionString;
            answerInput.SetQuestion(questionString);

            //Generate other possible answer choices
            GenerateChoices ();
        } 
        else {
            if (maxDifficulty > 20) {
                int ThirdNum = 1;
                int NewSecondNum = secondNum;

                for (int i = 5; i > 0; i++) {
                    if ( i % secondNum == 0) {
                        NewSecondNum = secondNum / i;
                        ThirdNum = i;
                        break;
                    }
                }
                NewSecondNum = 2;
                questionString = firstNum.ToString () + " " + " x " + " " + NewSecondNum.ToString () + " " + " x " + " " + ThirdNum.ToString () + " =";
            } else {
                //Generate formatted question string and set text box text
                questionString = firstNum.ToString () + " x " + secondNum.ToString () + " =";

            }
            //QuestionText.text = QuestionString;
            answerInput.SetQuestion(questionString);

            //Generate other possible answer choices
            GenerateChoices ();
        }
    }

    public void GenerateChoices() {
        /// <summary>
        /// Generates the choices.
        /// </summary>
        int Choice1;
        int Choice2;
        int Choice3;

        //Assign other choices depending on various factors
        if (isDivide == 0) {
            Choice1 = firstNum * secondNum;

            int PlusOrMinus = Random.Range (0, 2);
            if (PlusOrMinus == 0) {
                if (secondNum - 1 == 0){
                    Choice2 = firstNum / secondNum;
                }
                else {
                    Choice2 = firstNum / (secondNum - 1);
                }
                Choice3 = correctAnswer + Random.Range (1, 5);
            }
            else {
                Choice2 = firstNum / (secondNum + 1);
                Choice3 = correctAnswer - Random.Range (1, 5);
            }
        } 
        else {
            Choice1 = firstNum / secondNum;

            int PlusOrMinus = Random.Range (0, 2);

            if (PlusOrMinus == 0) {
                Choice2 = firstNum * (secondNum - 1);
                Choice3 = correctAnswer + Random.Range (1, 5);
            }
            else {
                Choice2 = firstNum * (secondNum + 1);
                Choice3 = correctAnswer - Random.Range (1, 5);
            }
        }

        //Array of all possible choices
        int[] integerChoices = new int[] { Choice1, Choice2, Choice3, correctAnswer };
        answerChoices = ChoicesToStringArray (integerChoices);
        answerInput.DisplayChoices (answerChoices);
    }

    public string[] ChoicesToStringArray(int [] integerChoices) {
        /// <summary>
        /// Converts the generated Choices integer array to array of strings for later use.
        /// Checks for duplicate values and shuffles array before returning
        /// </summary>
        /// <returns>The string array.</returns>
        /// <param name="integerChoices">Integer array of choices.</param>
        HashSet<int> choiceSet = new HashSet<int> ();
        int size = integerChoices.Length;

        //Check for duplicate values in array. If found, add a number in a random range
        for (int i = 0; i < size; i++) {
            if (choiceSet.Contains(integerChoices[i])) {
                integerChoices [i] += Random.Range(1, 4);
            }
            choiceSet.Add(integerChoices[i]);
        }

        //Shuffle array randomly
        for (int i = 0; i < integerChoices.Length; i++ ) {
            int temp = integerChoices[i];
            int r = Random.Range(i, integerChoices.Length);
            integerChoices[i] = integerChoices[r];
            integerChoices[r] = temp;

        }

        //Populate choice array with generated answer choices, converted to strings for later use
        answerChoices = new string[] {integerChoices[0].ToString(), integerChoices[1].ToString(), 
        integerChoices[2].ToString(), integerChoices[3].ToString()};

        return answerChoices;
    }

    /**Return formatted question string
    */
    public string GetQuestionString() {
        return questionString;
    }

    /**Return formatted answer string
    */
    public string GetCorrectAnswer() {
        return correctAnswer.ToString();
    }

    public void SetCorrectAnswer(string answer) {
        this.correctAnswer =  System.Int32.Parse (answer);;
    }

    public void SetQuestionString(string question) {
        this.questionString = question;
    }

    public void SetIncorrectAnswers(int incorrect) {
        incorrectAnswers = incorrect;
    }

    public int GetIncorrectAnswers() {
        return this.incorrectAnswers;
    }

    public int GetFirstNum() {
        return firstNum;
    }

    public int GetSecondNum() {
        return secondNum;
    }

    public string GetQuestionCategory() {
        if (isDivide == 0) {
            return "Division";
        }
        else {
            return "Multiplication";
        }
    }

    public string GetQuestionSubCategory() {
        int gtrValue = Mathf.Max(firstNum, secondNum);
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

    public bool GetAnsweredCorrectly() {
        return incorrectAnswers == 0;
    }

    /*
       public string GetQuestionRange()
       {
       if (isDivide == 0)
       return "0 - " + maxInt.ToString();
       else
       return "0 - " + (maxInt / 2).ToString();
       }
       */

    public void OnEndQuestion() {

    }

}
