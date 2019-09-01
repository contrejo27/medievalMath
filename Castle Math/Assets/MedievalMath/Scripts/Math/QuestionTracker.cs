using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
//using System;


public class QuestionTracker
{
    List<Question> incorrectQuestions;
    List<Question> correctQuestions;
    PlayerMathStats PlayerStats;
    const int isIncorrect = -1;
    const int isCorrect = 1;

    public Dictionary<string, QuestionData> questionData = new Dictionary<string, QuestionData>();

    public QuestionTracker()
    {
        incorrectQuestions = new List<Question>();
        correctQuestions = new List<Question>();
        PlayerStats = new PlayerMathStats();
    }


    /// <summary>
    /// Adds the incorrect question to a list of all incorect questions.
    /// </summary>
    /// <param name="question">The question that was answered</param>
    /// <param name="incorrectAnswers">number of times the question has been answered incorrectly.</param>
    public void AddIncorrectQuestion(Question question, int incorrectAnswers)
    {
        Question q;

        //Check object type and instanitate a Question object accordingly
        if (Object.ReferenceEquals(question.GetType(), typeof(Add)))
        {
            q = new Add();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Subtract)))
        {
            q = new Subtract();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(MultiplyOrDivide)))
        {
            q = new MultiplyOrDivide();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Fractions)))
        {
            q = new Fractions();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Compare)))
        {
            q = new Compare();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(TrueOrFalse)))
        {
            q = new TrueOrFalse();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Algebra)))
        {
            q = new Algebra();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(WordProblem)))
        {
            q = new WordProblem();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(FractionTargets)))
        {
            q = new FractionTargets();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(NumberLineQuestion)))
        {
            q = new NumberLineQuestion();
        }
        else
        {
            q = null;
        }

        //Set the question to add's values based on Question passed to the function
        q.SetQuestionString(question.GetQuestionString());
        q.SetCorrectAnswer(question.GetCorrectAnswer());
        q.SetIncorrectAnswers(incorrectAnswers);
        PlayerStats.UpdateScores(q, isIncorrect);
        //Debug.Log ("Question to add: " + q.GetQuestionString());

        // Update Library
        string questionType = question.GetQuestionCategory();
        string questionSubType = question.GetQuestionSubCategory();
        //Debug.Log("Incorrect. Question type: " + questionType + ", question subtype: " + questionSubType);
        if (questionData.ContainsKey(questionType))
        {
            if (questionData[questionType].data.ContainsKey(questionSubType))
            {
                questionData[questionType].IncIncorrect(questionSubType, 1);
            }
            else
            {
                questionData[questionType].data.Add(questionSubType, new QuestionData.DataLine(questionSubType, 0, 1, 0));
            }

        }
        else
        {
            questionData.Add(questionType, new QuestionData(questionType));
            questionData[questionType].data.Add(questionSubType, new QuestionData.DataLine(questionSubType, 0, 1, 0));
        }

        // CHANGE THIS IN THE FUTURE (way too frequent here)
        SaveToCSV();

        incorrectQuestions.Add(q);
    }

    public void RemoveIncorrectQuestion(Question question)
    {
        incorrectQuestions.Remove(question);
    }

    /// <summary>
    /// Adds the correct question to a list of all correctly answered questions.
    /// </summary>
    /// <param name="question">The question that was answered</param>
    /// <param name="incorrectAnswers">number of times the question has been answered incorrectly.</param>
    public void AddCorrectQuestion(Question question, int incorrectAnswers)
    {
        Question q;

        if (Object.ReferenceEquals(question.GetType(), typeof(Add)))
        {
            q = new Add();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Subtract)))
        {
            q = new Subtract();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(MultiplyOrDivide)))
        {
            q = new MultiplyOrDivide();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Fractions)))
        {
            q = new Fractions();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Compare)))
        {
            q = new Compare();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(TrueOrFalse)))
        {
            q = new TrueOrFalse();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Algebra)))
        {
            q = new Algebra();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(WordProblem)))
        {
            q = new WordProblem();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(FractionTargets)))
        {
            q = new FractionTargets();
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(NumberLineQuestion)))
        {
            q = new NumberLineQuestion();
        }
        else
        {
            q = null;
        }

        q.SetQuestionString(question.GetQuestionString());
        q.SetCorrectAnswer(question.GetCorrectAnswer());
        q.SetIncorrectAnswers(incorrectAnswers);
        PlayerStats.UpdateScores(q, isCorrect);

        // Update Library
        string questionType = question.GetQuestionCategory();
        string questionSubType = question.GetQuestionSubCategory();
        //Debug.Log("Correct! Question type: " + questionType+ ", question subtype: " + questionSubType);
        if (questionData.ContainsKey(questionType))
        {
            if (questionData[questionType].data.ContainsKey(questionSubType))
            {
                questionData[questionType].IncCorrect(questionSubType, 1);
            }
            else
            {
                questionData[questionType].data.Add(questionSubType, new QuestionData.DataLine(questionSubType, 1, 0, 100));
            }

        }
        else
        {
            questionData.Add(questionType, new QuestionData(questionType));
            questionData[questionType].data.Add(questionSubType, new QuestionData.DataLine(questionSubType, 1, 0, 100));
        }

        // CHANGE THIS IN THE FUTURE (way too frequent here)
        SaveToCSV();

        correctQuestions.Add(q);

    }

    #region CSV functions

    // Reference:
    // https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

    public void SaveToCSV()
    {

        var csv = new StringBuilder();
        // Using linq to divide up categories + count relevant values
        /*
        foreach (var qType in allQuestions.GroupBy(x => x.GetType()).Select(group => new { Variant = group.Key, items = group.ToList()}))
        {
            csv.AppendLine(qType.Variant.ToString() + "," + qType.items.Count.ToString());
            string line = "";
            foreach(var q in qType.items.GroupBy( y => y.GetQuestionSubCategory()).Select(sGroup => new { SubType = sGroup.Key, items = sGroup.ToList() }))
            {
                line += q.SubType.ToString() + ",";
                int totalQ = q.items.Count();
                int numCorrect = (from n in q.items where n.GetAnsweredCorrectly() == true select n).Count();
                line += numCorrect.ToString() + "," + (totalQ - numCorrect).ToString() + "," + (100*(float)numCorrect / (float)totalQ).ToString();
                csv.AppendLine(line);
                line = "";
            }
        }
        */
        foreach (string s in questionData.Keys)
        {
            csv.AppendLine(s + "," + questionData[s].data.Count);
            foreach (QuestionData.DataLine dl in questionData[s].data.Values)
            {
                csv.AppendLine(dl.subCat + "," + dl.numCorrect + "," + dl.numIncorrect);
                //Debug.Log(csv.ToString());
            }
        }

        StreamWriter outStr = File.CreateText(GetCSVPath());
        outStr.WriteLine(csv);
        outStr.Close();
    }

    public void ReadCSV()
    {
        //Debug.Log("Pre-CSV");
        if (!File.Exists(GetCSVPath()))
        {
            return;
        }
        //Debug.Log("Reading CSV");

        using (var reader = new StreamReader(GetCSVPath()))
        {

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                //Debug.Log("Line: " + line);
                var values = line.Split(',');
                if (values.Count() <= 1)
                {
                    break;
                }

                string category = values[0];
                int numSubCats = int.Parse(values[1]);

                //Debug.Log("Cat: " + category + " subcats: " + numSubCats);

                QuestionData qd;
                if (questionData.ContainsKey(category))
                {
                    qd = questionData[category];
                }
                else
                {
                    qd = new QuestionData(category);
                }

                for (int i = 0; i < numSubCats; i++)
                {
                    var sLine = reader.ReadLine();
                    var sValues = sLine.Split(',');

                    string subCat = sValues[0];

                    int numCorrect = int.Parse(sValues[1]);
                    int numIncorrect = int.Parse(sValues[2]);

                    if (qd.data.ContainsKey(subCat))
                    {
                        QuestionData.DataLine dl = qd.data[subCat];
                        dl.numCorrect += numIncorrect;
                        dl.numIncorrect += numCorrect;
                        dl.percentCorrect = (int)Mathf.Round(100 * (float)dl.numCorrect / (dl.numCorrect + dl.numIncorrect));
                        qd.data[subCat] = dl;

                    }
                    else
                    {
                        qd.data.Add(subCat, new QuestionData.DataLine(subCat, numCorrect, numIncorrect, (int)Mathf.Round(100 * (float)numCorrect / (numCorrect + numIncorrect))));
                    }

                }

                if (!questionData.ContainsKey(category))
                {
                    questionData.Add(category, qd);
                }
            }
        }

        // bear with me here:
        /*
        Debug.Log("DOFASODMASOSVM");
        foreach(string s in questionData.Keys)
        {
            Debug.Log(s);
            foreach(QuestionData.DataLine dl in questionData[s].data.Values)
            {
                Debug.Log(dl.ToString());
            }
        }
        */
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

    #endregion

    public void RemoveCorrectQuestion(Question question)
    {
        correctQuestions.Remove(question);
    }

    public List<Question> GetIncorrectQuestions()
    {
        return this.incorrectQuestions;
    }


    public List<Question> GetCorrectQuestions()
    {
        return this.correctQuestions;
    }

    public void ShowIncorrectQestions()
    {
        //Debug.Log ("Question List: " );
        /*
                for (int i = 0; i < incorrectQuestions.Count; i++) {
                    Debug.Log(incorrectQuestions [i].GetQuestionString ());
                    Debug.Log ("Incorrect attempts: " + incorrectQuestions [i].GetIncorrectAnswers ());
                }*/
    }

    public int GetIncorrectQuestionCount()
    {
        return this.incorrectQuestions.Count;
    }

    public int GetCorrectQuestionCount()
    {
        return this.correctQuestions.Count;
    }

    public int GetIncorrectOfType(System.Type type)
    {
        int count = 0;
        foreach (Question question in incorrectQuestions)
        {
            if (Object.ReferenceEquals(question.GetType(), type))
            {
                count++;
            }
        }
        return count;
    }

    public int GetCorrectOfType(System.Type type)
    {
        int count = 0;
        foreach (Question question in correctQuestions)
        {
            if (Object.ReferenceEquals(question.GetType(), type))
            {
                count++;
            }
        }
        return count;
    }

    public void updateQuestionHistory()
    {
        foreach (Question question in correctQuestions)
        {
            Debug.Log("Saving: " + question.GetQuestionString());
        }
    }
}

public class QuestionData
{
    public string type;
    public Dictionary<string, DataLine> data = new Dictionary<string, DataLine>();

    public struct DataLine
    {
        public string subCat;
        public int numCorrect;
        public int numIncorrect;
        public int percentCorrect;

        public DataLine(string _subCat, int _numCorrect, int _numIncorrect, int _percentCorrect)
        {
            subCat = _subCat;
            numCorrect = _numCorrect;
            numIncorrect = _numIncorrect;
            percentCorrect = _percentCorrect;
        }

        public override string ToString()
        {
            return string.Format("{0,6}{1,20}{2,16}{3,10}", subCat, numCorrect.ToString(), numIncorrect.ToString(), (percentCorrect.ToString() + "%"));
        }
    }

    public QuestionData(string _type)
    {
        type = _type;
    }

    public void IncCorrect(string subCat, int value)
    {
        DataLine dl = data[subCat];
        dl.numCorrect += value;
        dl.percentCorrect = (int)Mathf.Round(100 * (float)dl.numCorrect / (dl.numCorrect + dl.numIncorrect));
        data[subCat] = dl;
    }

    public void IncIncorrect(string subCat, int value)
    {
        DataLine dl = data[subCat];
        dl.numIncorrect += value;
        dl.percentCorrect = (int)Mathf.Round(100 * (float)dl.numCorrect / (dl.numCorrect + dl.numIncorrect));
        data[subCat] = dl;
    }
}
