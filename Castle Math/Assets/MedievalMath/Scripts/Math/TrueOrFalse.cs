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

	public Text questionText;
	public Text questionTextHUD;

	private string questionString;
	private int maxInt;
	private AnswerInput aInput;
	private string correctAnswer;
	private string [] answerChoices;
	private int incorrectAnswers;

	public TrueOrFalse() {
	}

	// Use this for initialization
	public void Start () {
		aInput = GameObject.FindObjectOfType<AnswerInput> ();
		questionText = GameObject.Find ("question").GetComponent<Text>();
	}

	/// <summary>
	/// Generates a true or false question.
	/// </summary>
	/// <param name="maxDifficulty">Placeholder value to allow for polymorphism of Questions</param>
	public void GenerateQuestion(int maxDifficulty) { //int maxDifficulty => temp fix

		//Load Json file from assets folder
		TextAsset text = (TextAsset)Resources.Load <TextAsset>("questions");
		List<TextQuestion> TextQuestions;

		if (text != null) {
			string questionJson = text.text;
			TextQuestions = JsonHelper.FromJson<TextQuestion> (questionJson);

			int rand = Random.Range (0, TextQuestions.Count);
			TextQuestion currQuestion = TextQuestions[rand];
			//QuestionText.text = currQuestion.questionText;
			aInput.SetQuestion(currQuestion.questionText);

			Debug.Log (currQuestion.questionText);
			correctAnswer = currQuestion.answer;
			Debug.Log (correctAnswer);
			GenerateChoices ();
		} else {
			Debug.Log ("Null JSON");
		}
	}

	public void GenerateChoices() {
		answerChoices = new string[] {"True", "False"};
		Debug.Log(answerChoices[0]);
		aInput.DisplayChoices(answerChoices);
	}

	public string GetCorrectAnswer() {
		return correctAnswer;
	}

	public string GetQuestionString() {
		return this.questionString;
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
		return "True or False";
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

    }
}
