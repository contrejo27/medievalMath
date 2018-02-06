using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compare : MonoBehaviour, Question {

	public Text QuestionText;
	public Text QuestionText_hud;

	private double leftCompare;
	private double rightCompare;
	private string CorrectAnswer;
	private int isSubtract;
	private string [] AnswerChoices;
	private string QuestionString;
	private List <string> Symbols;
	private AnswerInput A_Input;
	private int incorrectAnswers;

	//public QuestionGenerator QG;

	public Compare() {
	} 

	// Use this for initialization
	public void Start () {
		//Debug.Log ("Start compare");
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();

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

        QuestionString = "Which symbol makes this comparision true?\n" 
                            + leftCompare.ToString () + " __ " + rightCompare.ToString ();

		Debug.Log (QuestionString);
		//QuestionText.text = QuestionString;
		A_Input.SetQuestion(QuestionString);

		GenerateChoices ();

	}

    string getCorrectSymbol() {
        //string correctSymbol = "";
		Symbols = new List<string>(){"<", ">", "=", "<=", ">=", "-", "+", "%"};

		if (leftCompare < rightCompare) {
			CorrectAnswer = "less";
		}
		else if (leftCompare > rightCompare){
			CorrectAnswer = "greater";
		}
		else if (leftCompare == rightCompare) {
			CorrectAnswer = "equal";
		}

        int r = Random.Range(0, 2);
		int rAdd = Random.Range (0, 2);

		if (CorrectAnswer == "less") {
			Symbols.Remove ("<=");
			Symbols.Remove ("<");

			if (r == 0) {
				CorrectAnswer = "<=";
			}
			else{
				CorrectAnswer = "<";
			}
        }
		else if (CorrectAnswer == "greater") {
			Symbols.Remove (">=");
			Symbols.Remove (">");

			if (r == 0) {
				CorrectAnswer = ">=";
			} else {
				CorrectAnswer = ">";
			}
        }
		else if (CorrectAnswer == "equal") {
            //Adds third variable possibility
			Symbols.Remove ("<=");
			Symbols.Remove (">=");
			Symbols.Remove ("=");
            r += rAdd;
			if (r == 0) {
				CorrectAnswer = ">=";
			} else if (r == 1) {
				CorrectAnswer = "<=";
			} else {
				CorrectAnswer = "=";
			}
        }
		Debug.Log ("Correct Symbol:" + CorrectAnswer);
		return CorrectAnswer;
    }
		
	public void GenerateChoices() {
        string correctSymbol = this.getCorrectSymbol();
		AnswerChoices = new string[] {"","","",""};

		for (int i = 0; i < AnswerChoices.Length - 1; i++) {
			int index = Random.Range(0, Symbols.Count - 1);
		
           	AnswerChoices[i] = Symbols[index];
			Symbols.RemoveAt (index);
		}
        
        AnswerChoices[AnswerChoices.Length - 1] = correctSymbol;

		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;

		}

		A_Input.DisplayChoices (AnswerChoices);
	}

	public string GetQuestionString() {
		return this.QuestionString;
	}

	public string getCorrectAnswer() {
		return CorrectAnswer;
	}

	public void SetCorrectAnswer(string answer) {
		this.CorrectAnswer = answer;
	}

	public void SetQuestionString(string question) {
		this.QuestionString = question;
	}

	public void SetIncorrectAnswers(int incorrect) {
		incorrectAnswers = incorrect;
	}

	public int GetIncorrectAnswers() {
		return this.incorrectAnswers;
	}

}
