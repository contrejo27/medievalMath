using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Items
{
	public TextQuestion[] TextQuestions;
}

[System.Serializable]
public class TextQuestion
{
	//public TextQuestion [] Question { get; set; }
	public string questionText;
	public string answer;
	public string hint;
}


public class TrueOrFalse : MonoBehaviour {

	public Text QuestionText;
	public Text QuestionText_hud;

	private string QuestionString;
	private int maxInt;
	private AnswerInput A_Input;
	private string CorrectAnswer;
	private string [] AnswerChoices;

	public TrueOrFalse() {
	}

	// Use this for initialization
	public void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
	}

	public void GenerateQuestion() {
		/*
		string questionLine = @"{""question"":""A fraction is the same thing as a ratio."",""answer"":""True"",""hint"":""""}";
		TextQuestion questionJSON = JsonUtility.FromJson<TextQuestion> (questionLine);
		QuestionText.text = questionJSON.question;
		CorrectAnswer = questionJSON.answer;
		A_Input.SetCorrectAnswer (CorrectAnswer);
		Debug.Log (CorrectAnswer);
		GenerateChoices ();
		*/


		TextAsset text = (TextAsset)Resources.Load <TextAsset>("questions");
		TextQuestion[] TextQuestions;

		if (text != null) {
			string questionJson = text.text;
			TextQuestions = JsonHelper.FromJson<TextQuestion> (questionJson);

			int rand = Random.Range (0, TextQuestions.Length);
			TextQuestion currQuestion = TextQuestions[rand];
			QuestionText.text = currQuestion.questionText;
			Debug.Log (currQuestion.questionText);
			CorrectAnswer = currQuestion.answer;
			Debug.Log (CorrectAnswer);
			GenerateChoices ();
		} else {
			Debug.Log ("Null JSON");
		}

		/*
		for (int i = 0; i < TextQuestions.Length; i++) {
			TextQuestion currQuestion = TextQuestions[i];
			QuestionText.text = currQuestion.questionText;
			Debug.Log (currQuestion.questionText);
			CorrectAnswer = currQuestion.answer;
			Debug.Log (CorrectAnswer);
			GenerateChoices ();
		}
		*/


		/*
		string[] questionLines = questionText.Split ('\n');

		for (int i = 0; i < questionLines.Length; i++) {
			TextQuestion questionJSON = JsonUtility.FromJson<TextQuestion> (questionLines[i]);
			QuestionText.text = questionJSON.question;
			CorrectAnswer = questionJSON.answer;
			A_Input.SetCorrectAnswer (CorrectAnswer);
			Debug.Log (CorrectAnswer);
			GenerateChoices ();
		}
		*/

			
		/*
		System.IO.StreamReader file = new System.IO.StreamReader(text.text);

		string line = file.ReadLine ();

		ArrayList TrueOrFalseQuestions = new ArrayList ();
		for (int i = 0; line != null; i++) {
			TextQuestion questionJSON = JsonUtility.FromJson<TextQuestion> (line);

			TrueOrFalseQuestions.Add(questionJSON);
		}

		int rand = Random.Range (0, TrueOrFalseQuestions.Count);
		TextQuestion questionToDisplay = (TextQuestion)TrueOrFalseQuestions [rand];
		QuestionText.text = questionToDisplay.question;
		CorrectAnswer = questionToDisplay.answer;
		A_Input.SetCorrectAnswer (CorrectAnswer);
		Debug.Log (CorrectAnswer);
		GenerateChoices ();
		*/

	}

	public void GenerateChoices() {
		AnswerChoices = new string[] {"True", "False"};
		Debug.Log(AnswerChoices[0]);
		A_Input.DisplayChoices(AnswerChoices);
	}

	public string getCorrectAnswer() {
		return CorrectAnswer;
	}
		
}
