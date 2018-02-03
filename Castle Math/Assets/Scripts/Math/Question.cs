using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Question  {

	void GenerateQuestion(int maxDifficulty);
	void GenerateChoices();
	void SetQuestionString (string question);
	void SetCorrectAnswer(string answer);
	string GetQuestionString();
	string getCorrectAnswer();


}
