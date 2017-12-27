using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compare : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;

	private double leftCompare;
	private double rightCompare;
	private int CorrectAnswer;
	private int isSubtract;
	private string [] AnswerChoices;
	private string QuestionString;

	private AnswerInput A_Input;

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
	public void GenerateQuestion () {

        this.leftCompare = Random.Range(0.0f, 1.0f);  //.3544
        
        int decimalPlaces = Random.Range(2, 5); //3

		int step = Random.Range (0, 4); //2
        int multiple = 10;

        for (int i = 0; i < step; i++) {
            multiple *= multiple; //100,1000
        }

		leftCompare = System.Math.Round(leftCompare, decimalPlaces);//.354

        this.rightCompare = leftCompare * multiple; //354

        QuestionString = "Which symbol makes this comparision true?\n" 
                            + leftCompare.ToString () + " __ " + rightCompare.ToString ();

		QuestionText.text = QuestionString;
		GenerateChoices ();

        //Debug.Log(QuestionString);
	}

    string getCorrectSymbol() {
        string correctSymbol = "";


		if (leftCompare < rightCompare) {
            correctSymbol = "less";
			//Debug.Log ("Sym " + correctSymbol);
		}
		else if (leftCompare > rightCompare){
            correctSymbol = "greater";
			//Debug.Log ("Sym " + correctSymbol);
		}
		else if (leftCompare == rightCompare) {
            correctSymbol = "equal";
			//Debug.Log ("Sym " + correctSymbol);
		}

        int r = Random.Range(0, 2);
		int rAdd = Random.Range (0, 2);

		if (correctSymbol == "less") {
			if (r == 0) {
                correctSymbol = "<=";
				//Debug.Log ("Sym " + correctSymbol);
			}
			else{
				correctSymbol = "<";
				//Debug.Log ("Sym " + correctSymbol);
			}
        }
        else if (correctSymbol == "greater") {
			if (r == 0) {
				correctSymbol = ">=";
				//Debug.Log ("Sym " + correctSymbol);
			} else {
				correctSymbol = ">";
				//Debug.Log ("Sym "+ correctSymbol);
			}
        }
		else if (correctSymbol == "equal") {
            //Adds third variable possibility
            r += rAdd;
			if (r == 0) {
				correctSymbol = ">=";
				//Debug.Log ("Sym " + correctSymbol);
			} else if (r == 1) {
				correctSymbol = "<=";
				//Debug.Log ("Sym " + correctSymbol);
			} else {
				correctSymbol = "=";
				//Debug.Log ("Sym " + correctSymbol);
			}
        }

        return correctSymbol;
    }
		
	void GenerateChoices() {
		////Debug.Log ("Gen Choices");
        string correctSymbol = this.getCorrectSymbol();
		//Debug.Log ("Correct: " + correctSymbol);

		//string Choice1;
		//string Choice2;
		//string Choice3;

        string [] compareSymbols = {"<", ">", "=", "<=", ">=", "-", "+", "%"};
		AnswerChoices = new string[] {"","","",""};
		List<int> usedValues = new List<int>();
        for (int i = 0; i < AnswerChoices.Length - 1; i++){
            int index = Random.Range(0, compareSymbols.Length);
            
            while(usedValues.Contains(index)) {
                index = Random.Range(0, compareSymbols.Length);
            }
            
            AnswerChoices[i] = compareSymbols[index];
			//Debug.Log(i + AnswerChoices[i]);

			usedValues.Add(index);
		}
        
        AnswerChoices[AnswerChoices.Length - 1] = correctSymbol;

		/*
		//Shuffle array randomly
		for (int i = 0; i < AnswerChoices.Length; i++ ) {
			string temp = AnswerChoices[i];
			int r = Random.Range(i, AnswerChoices.Length);
			AnswerChoices[i] = AnswerChoices[r];
			AnswerChoices[r] = temp;
            //Debug.Log(i + AnswerChoices[r]);
		}
		*/


		A_Input.DisplayChoices (AnswerChoices);
	}

	public string GetQuestionString() {
		return this.QuestionString;
	}

	public string getCorrectAnswer() {
		return this.getCorrectSymbol();
	}

}
