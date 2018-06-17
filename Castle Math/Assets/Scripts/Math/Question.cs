using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Question  {

	void GenerateQuestion(int maxDifficulty);
	void GenerateChoices();
	void SetQuestionString (string question);
	void SetCorrectAnswer(string answer);
	void SetIncorrectAnswers (int incorrect);
    void OnEndQuestion();
	int GetIncorrectAnswers ();
	string GetQuestionString ();
	string GetCorrectAnswer ();
	string GetQuestionCategory();
    string GetQuestionSubCategory();
    bool GetAnsweredCorrectly();
    //int GetLargestOperator();
    //string GetQuestionRange();

}
