using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestionTracker  {


	List <Question> incorrectQuestions;
	List <Question> correctQuestions;

	public QuestionTracker () {
		incorrectQuestions = new List <Question>();
		correctQuestions = new List <Question>();
	}

	public void AddIncorrectQuestion(Question question) {
		Question q;

		if (Object.ReferenceEquals (question.GetType (), typeof(AddOrSubtract))) {
			q = new AddOrSubtract ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(MultiplyOrDivide))) {
			q = new MultiplyOrDivide ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Fractions))) {
			q = new Fractions ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Compare))) {
			q = new Compare ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(TrueOrFalse))) {
			q = new TrueOrFalse ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else {
			q = null;
		}

		Debug.Log ("Question to add: " + q.GetQuestionString());

		incorrectQuestions.Add (q);
	}

	public void RemoveIncorrectQuestion(Question question) {
		incorrectQuestions.Remove (question);
	}

	public void AddCorrectQuestion(Question question) {
		Question q;

		if (Object.ReferenceEquals (question.GetType (), typeof(AddOrSubtract))) {
			q = new AddOrSubtract ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(MultiplyOrDivide))) {
			q = new MultiplyOrDivide ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Fractions))) {
			q = new Fractions ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Compare))) {
			q = new Compare ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else if (Object.ReferenceEquals (question.GetType (), typeof(TrueOrFalse))) {
			q = new TrueOrFalse ();
			q.SetQuestionString (question.GetQuestionString());
			q.SetCorrectAnswer (question.getCorrectAnswer ());
		} else {
			q = null;
		}

		correctQuestions.Add (q);
	}

	public void RemoveCorrectQuestion(Question question) {
		correctQuestions.Remove (question);
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
			Debug.Log(incorrectQuestions [i].GetQuestionString ());
		}
	}

}
