using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestionTracker  {


	List <Question> incorrectQuestions;
	List <Question> correctQuestions;
	PlayerMathStats PlayerStats;
	const int isIncorrect = -1;
	const int isCorrect = 1;

	public QuestionTracker () {
		incorrectQuestions = new List <Question>();
		correctQuestions = new List <Question>();
		PlayerStats = new PlayerMathStats ();
	}

	public void AddIncorrectQuestion(Question question, int incorrectAnswers) {
		Question q;

		if (Object.ReferenceEquals (question.GetType (), typeof(AddOrSubtract))) {
			q = new AddOrSubtract ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(MultiplyOrDivide))) {
			q = new MultiplyOrDivide ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Fractions))) {
			q = new Fractions ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Compare))) {
			q = new Compare ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(TrueOrFalse))) {
			q = new TrueOrFalse ();	
		} else {
			q = null;
		}

		q.SetQuestionString (question.GetQuestionString());
		q.SetCorrectAnswer (question.getCorrectAnswer());
		q.SetIncorrectAnswers (incorrectAnswers);
		PlayerStats.UpdateScores (q, isIncorrect);
		Debug.Log ("Question to add: " + q.GetQuestionString());

		incorrectQuestions.Add (q);
	}

	public void RemoveIncorrectQuestion(Question question) {
		incorrectQuestions.Remove (question);
	}

	public void AddCorrectQuestion(Question question, int incorrectAnswers) {
		Question q;

		if (Object.ReferenceEquals (question.GetType (), typeof(AddOrSubtract))) {
			q = new AddOrSubtract ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(MultiplyOrDivide))) {
			q = new MultiplyOrDivide ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Fractions))) {
			q = new Fractions ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Compare))) {
			q = new Compare ();
		} else if (Object.ReferenceEquals (question.GetType (), typeof(TrueOrFalse))) {
			q = new TrueOrFalse ();
		} else {
			q = null;
		}

		q.SetQuestionString (question.GetQuestionString());
		q.SetCorrectAnswer (question.getCorrectAnswer());
		q.SetIncorrectAnswers (incorrectAnswers);

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
			Debug.Log ("Incorrect attempts: " + incorrectQuestions [i].GetIncorrectAnswers ());
		}
	}

	public int GetIncorrectQuestionCount() {
		return this.incorrectQuestions.Count;
	}

	public int GetCorrectQuestionCount() {
		return this.correctQuestions.Count;
	}

	public int GetIncorrectOfType(System.Type type) {
		int count = 0;
		foreach (Question question in incorrectQuestions) {
			if (Object.ReferenceEquals(question.GetType(), type)) {
				count++;
			}
		}
		return count;
	}

	public int GetCorrectOfType(System.Type type) {
		int count = 0;
		foreach (Question question in correctQuestions) {
			if (Object.ReferenceEquals(question.GetType(), type)) {
				count++;
			}
		}
		return count;
	}
}
