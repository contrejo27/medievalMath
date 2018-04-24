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
	//This could be the thing to break it. IDK I can fix if need be - Hannah
	public string [] answerChoices;
}


public class TrueOrFalse : MonoBehaviour, Question {

	public Text QuestionText;
	public Text QuestionText_hud;

	private string QuestionString;
	private int maxInt;
	private AnswerInput A_Input;
	private string CorrectAnswer;
	private string [] AnswerChoices;
	private int incorrectAnswers;

	public TrueOrFalse() {
	}

	// Use this for initialization
	public void Start () {
		A_Input = GameObject.FindObjectOfType<AnswerInput> ();
		QuestionText = GameObject.Find ("question").GetComponent<Text>();
	}

	/// <summary>
	/// Generates a true or false question.
	/// </summary>
	/// <param name="maxDifficulty">Placeholder value to allow for polymorphism of Questions</param>
	public void GenerateQuestion(int maxDifficulty) { //int maxDifficulty => temp fix

		//Load Json file from assets folder
		TextAsset text = (TextAsset)Resources.Load <TextAsset>("questions");
		TextQuestion[] TextQuestions;

		if (text != null) {
			string questionJson = text.text;
			TextQuestions = JsonHelper.FromJson<TextQuestion> (questionJson);

			int rand = Random.Range (0, TextQuestions.Length);
			TextQuestion currQuestion = TextQuestions[rand];
			//QuestionText.text = currQuestion.questionText;
			A_Input.SetQuestion(currQuestion.questionText);

			Debug.Log (currQuestion.questionText);
			CorrectAnswer = currQuestion.answer;
			Debug.Log (CorrectAnswer);
			GenerateChoices ();
		} else {
			Debug.Log ("Null JSON");
		}
	}

	public void GenerateChoices() {
		AnswerChoices = new string[] {"True", "False"};
		Debug.Log(AnswerChoices[0]);
		A_Input.DisplayChoices(AnswerChoices);
	}

	public string getCorrectAnswer() {
		return CorrectAnswer;
	}

	public string GetQuestionString() {
		return this.QuestionString;
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

	public string GetQuestionCategory() {
		return "True or False";
	}

}
