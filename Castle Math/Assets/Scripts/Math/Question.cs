using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Question  {

	void GenerateQuestion(int maxDifficulty);
	void GenerateChoices();
	void SetQuestionString (string question);
	void SetCorrectAnswer(string answer);
	void SetIncorrectAnswers (int incorrect);
	int GetIncorrectAnswers ();
	string GetQuestionString ();
	string getCorrectAnswer ();

}
