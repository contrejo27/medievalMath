using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Question  {

	void GenerateQuestion(int maxDifficulty);
	void GenerateChoices();
	string GetQuestionString();
	string getCorrectAnswer();

}
