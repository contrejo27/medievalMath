using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionTracker  {


	List <Question> incorrectQuestions = new List <Question>();
	List <Question> correctQuestions = new List <Question>();

	public QuestionTracker () {
		
	}
	public void AddIncorrectQuestion(Question question) {
		Question q = question;
		q = question;
		Debug.Log ("Question to add: " + q.GetQuestionString());

		incorrectQuestions.Add (q);
	}

	public void RemoveIncorrectQuestion(Question question) {
		incorrectQuestions.Remove (question);
	}

	public void AddCorrectQuestion(Question question) {
		incorrectQuestions.Add (question);
	}

	public void RemoveCorrectQuestion(Question question) {
		incorrectQuestions.Remove (question);
	}

	public List<Question> GetIncorrectQuestions() {
		return this.incorrectQuestions;
	}

	public List <Question> GetCorrectQuestions() {
		return this.correctQuestions;
	}

	public void ShowIncorrectQestions() {
		Debug.Log ("Question List: " );
		for (int i = 0; i < incorrectQuestions.Count; i++) {
			Debug.Log (incorrectQuestions [i].GetQuestionString ());
		}
	}

}
