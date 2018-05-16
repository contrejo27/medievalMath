using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compare : MonoBehaviour, Question {

	public Text questionText;
	public Text questionTextHUD;

	private double leftCompare;
	private double rightCompare;
	private string correctAnswer;
	private int isSubtract;
	private string [] answerChoices;
	private string questionString;
	private List <string> symbols;
	private AnswerInput aInput;
	private int incorrectAnswers;

	//public QuestionGenerator QG;

	public Compare() {
	} 

	// Use this for initialization
	public void Start () {
		//Debug.Log ("Start compare");
		aInput = GameObject.FindObjectOfType<AnswerInput> ();
		questionText = GameObject.Find ("question").GetComponent<Text>();

	}

	// Update is called once per frame
	public void GenerateQuestion (int maxDifficulty) { //int maxDifficulty => temp fix

        this.leftCompare = Random.Range(0.0f, 1.0f);  //.3544
		this.rightCompare = leftCompare;
        
        int decimalPlaces = Random.Range(2, 5); //3

		int step = Random.Range (0, 4); //2
        int multiple = 10;

        for (int i = 0; i < step; i++) {
            multiple *= multiple; //100,1000
        }

		//Randomly increase value of one number
		if (step < 2) {
			this.rightCompare *= multiple; //354
		} else {
			this.leftCompare *= multiple;
		}

		leftCompare = System.Math.Round(leftCompare, decimalPlaces);//.354
		rightCompare = System.Math.Round(rightCompare, decimalPlaces);

        questionString = "Which symbol makes this comparision true?\n" 
                            + leftCompare.ToString () + " __ " + rightCompare.ToString ();

		Debug.Log (questionString);
		//QuestionText.text = QuestionString;
		aInput.SetQuestion(questionString);

		GenerateChoices ();

	}

    string GetCorrectSymbol() {
        //string correctSymbol = "";
		symbols = new List<string>(){"<", ">", "=", "<=", ">=", "-", "+", "%"};

		if (leftCompare < rightCompare) {
			correctAnswer = "less";
		}
		else if (leftCompare > rightCompare){
			correctAnswer = "greater";
		}
		else if (leftCompare == rightCompare) {
			correctAnswer = "equal";
		}

        int r = Random.Range(0, 2);
		int rAdd = Random.Range (0, 2);

		if (correctAnswer == "less") {
			symbols.Remove ("<=");
			symbols.Remove ("<");

			if (r == 0) {
				correctAnswer = "<=";
			}
			else{
				correctAnswer = "<";
			}
        }
		else if (correctAnswer == "greater") {
			symbols.Remove (">=");
			symbols.Remove (">");

			if (r == 0) {
				correctAnswer = ">=";
			} else {
				correctAnswer = ">";
			}
        }
		else if (correctAnswer == "equal") {
            //Adds third variable possibility
			symbols.Remove ("<=");
			symbols.Remove (">=");
			symbols.Remove ("=");
            r += rAdd;
			if (r == 0) {
				correctAnswer = ">=";
			} else if (r == 1) {
				correctAnswer = "<=";
			} else {
				correctAnswer = "=";
			}
        }
		Debug.Log ("Correct Symbol:" + correctAnswer);
		return correctAnswer;
    }
		
	public void GenerateChoices() {
        string correctSymbol = this.GetCorrectSymbol();
		answerChoices = new string[] {"","","",""};

		for (int i = 0; i < answerChoices.Length - 1; i++) {
			int index = Random.Range(0, symbols.Count - 1);
		
           	answerChoices[i] = symbols[index];
			symbols.RemoveAt (index);
		}
        
        answerChoices[answerChoices.Length - 1] = correctSymbol;

		//Shuffle array randomly
		for (int i = 0; i < answerChoices.Length; i++ ) {
			string temp = answerChoices[i];
			int r = Random.Range(i, answerChoices.Length);
			answerChoices[i] = answerChoices[r];
			answerChoices[r] = temp;

		}

		aInput.DisplayChoices (answerChoices);
	}

	public string GetQuestionString() {
		return this.questionString;
	}

	public string GetCorrectAnswer() {
		return correctAnswer;
	}

	public void SetCorrectAnswer(string answer) {
		this.correctAnswer = answer;
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

	public string GetQuestionCategory() {
		return "Comparision";
	}

    public string GetQuestionSubCategory()
    {
        return "temp";
    }

    public bool GetAnsweredCorrectly()
    {
        return incorrectAnswers == 0;
    }

}
