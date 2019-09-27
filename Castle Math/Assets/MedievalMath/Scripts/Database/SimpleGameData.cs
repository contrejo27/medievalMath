using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class SimpleGameData : MonoBehaviour
{
    List<QuestionAnswered> answerList = new List<QuestionAnswered>();
    QuestionAnswered currentQuestion;

    class QuestionAnswered
    {
        public List<string> userAnswers = new List<string>();
        public bool correct;
        public string question;
        public string correctAnswer;
        public string mathType;
        public int id;

        public int incorrectAttempts;

        public QuestionAnswered(string lQuestion, string lCorrectAnswer, string lMathType)
        {
            id = Random.Range(0, 256);
            question = lQuestion;
            correctAnswer = lCorrectAnswer;
            mathType = lMathType;
            incorrectAttempts = 0;
        }

        public QuestionAnswered(QuestionAnswered questionToCopy)
        {
            correct = questionToCopy.correct;
            question = questionToCopy.question;
            correctAnswer = questionToCopy.correctAnswer;
            mathType = questionToCopy.mathType;

            incorrectAttempts = questionToCopy.incorrectAttempts;
        }
    }

    public void NewQuestionInput(string newQuestion, string newAnswer, string mathType)
    {
        currentQuestion = new QuestionAnswered(newQuestion, newAnswer, mathType);
    }

    public void NewAnswer(string answer, bool correct)
    {
        currentQuestion.userAnswers.Add(answer);
        foreach(string s in currentQuestion.userAnswers)
        {
            print(s);
        }
        currentQuestion.correct = correct;
        if (correct)
        {
            answerList.Add(new QuestionAnswered(currentQuestion));
        }
        else
        {
            currentQuestion.incorrectAttempts++;
        }
    }

    public void SaveMathToCSV()
    {
        StringBuilder csv = new StringBuilder();
        csv.Append(AnswerListToString());
        StreamWriter outStr = File.AppendText(GetCSVPath());
        outStr.WriteLine(csv);
        outStr.Close();
        answerList.Clear();
    }

    string AnswerListToString()
    {
        string csvInput = "";
        foreach (QuestionAnswered q in answerList)
        {
            csvInput += q.question + "," + q.correctAnswer + "," + q.mathType;
            foreach (string s in q.userAnswers)
            {
                csvInput += "," + s;
            }
            csvInput += "\n";
        }
        return csvInput;
    }

    private string GetCSVPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/MedievalMath/CSV/" + "Question_Tracker.csv";
#elif UNITY_ANDROID
            return Application.persistentDataPath+"Question_Tracker.csv";
#elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+"Question_Tracker.csv";
#else
            return Application.dataPath +"/"+"Question_Tracker.csv";
#endif
    }
}
