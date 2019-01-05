using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
[System.Serializable]
public class Items
{
	public TextQuestion[] TextQuestions;
}

[System.Serializable]
public class TextProblem
{
	//public TextQuestion [] Question { get; set; }
	public string questionText;
	public string answer;
	public string hint;
	public string answerChoices;
}
*/

public class WordProblem : MonoBehaviour, Question {

	public Text questionText;
	public Text questionTextHUD;

	private string questionString;
	private int maxInt;
	private AnswerInput aInput;
	private string correctAnswer;
	private string [] answerChoices;
	private int incorrectAnswers;
    private int[] numbersInQuestion;

	TextQuestion currQuestion;
    WordProblemText currentQuestion;
    List<WordProblemText> Addition = new List<WordProblemText>();
    List<WordProblemText> Subtraction = new List<WordProblemText>();
    List<WordProblemText> Multiplication = new List<WordProblemText>();
    List<WordProblemText> Division = new List<WordProblemText>();

	public WordProblem() {
	}

	// Use this for initialization
	public void Start () {
		aInput = GameObject.FindObjectOfType<AnswerInput> ();
		//questionText = GameObject.Find ("question").GetComponent<Text>();

        StartCoroutine(GetQuestions());
	}

	/// <summary>
	/// Generates a true or false question.
	/// </summary>
	/// <param name="maxDifficulty">Placeholder value to allow for polymorphism of Questions</param>
	public void GenerateQuestion(int maxDifficulty) { //int maxDifficulty => temp fix
        List<WordProblemText> possibleQuestions = new List<WordProblemText>(Addition);
        possibleQuestions.AddRange(Subtraction);

        if (possibleQuestions.Count > 0)
        {
            currentQuestion = possibleQuestions[Random.Range(0, possibleQuestions.Count)];
            Debug.Log("Creating Question from: " + currentQuestion.ToString());
            
            switch (currentQuestion.questionType)
            {
                case "Addition":
                    GenerateAdditionWP();
                    break;
                case "Subtraction":
                    GenerateSubtractionWP();
                    break;
                case "Multiplication":
                    GenerateMultiplicaitonWP();
                    break;
                case "Division":
                    GenerateDivisionWP();
                    break;
                default:
                    break;
            }

            GenerateQuestionString();
            Debug.Log("final question: " + questionString);
            aInput.SetQuestion(questionString, 1);
        }
        
        /*
		//Load Json file from assets folder
		TextAsset text = (TextAsset)Resources.Load <TextAsset>("wordproblems");
		TextQuestion[] TextQuestions;

		if (text != null) {
			string questionJson = text.text;
			TextQuestions = JsonHelper.FromJson<TextQuestion> (questionJson);

			int rand = Random.Range (0, TextQuestions.Length);
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
        */
	}

    public void GenerateQuestionString()
    {
        questionString = "";
       

        //int ans = numbersInQuestion[0] - numbersInQuestion[1];

        int intCounter = 0;
        int varCounter = 0;
        int replacementCounter = 0;

        List<string> usedValues = new List<string>();

        for (int i = 0; i < currentQuestion.text.Length; i++)
        {
            questionString += currentQuestion.text[i];
            //Debug.Log(questionString);
            if (replacementCounter < currentQuestion.replacementType.Length)
            {
                int n;
                string c = currentQuestion.replacementType[i];
                //Debug.Log("c: " +c);
                if (c == "i")
                {
                    questionString += numbersInQuestion[intCounter].ToString();
                    usedValues.Add(numbersInQuestion[intCounter].ToString());
                    intCounter++;
                }
                else if (c == "s")
                {
                    string varString = currentQuestion.variableStrings[varCounter].array[Random.Range(0, currentQuestion.variableStrings[varCounter].array.Count)];
                    //Debug.Log("VarString: " + varString);
                    questionString += varString;
                    usedValues.Add(varString);
                    varCounter++;
                }
                else if (int.TryParse(c, out n))
                {
                    questionString += usedValues[n];
                }
                replacementCounter++;
            }
            Debug.Log(questionString);
        }
    }

    public void GenerateAdditionWP()
    {
        numbersInQuestion = new int[currentQuestion.ranges.Count];

        for (int i = 0; i < numbersInQuestion.Length; i++)
        {
            numbersInQuestion[i] = Random.Range(currentQuestion.ranges[i].array[0], currentQuestion.ranges[i].array[1]);
        }

        int localCorrect = numbersInQuestion[0] + numbersInQuestion[1];

        correctAnswer = localCorrect.ToString();

        int choice1;
        int choice2;
        int choice3;

        choice1 = numbersInQuestion[0] - numbersInQuestion[1];

        int PlusOrMinus = Random.Range(0, 2);
        if (PlusOrMinus == 0)
        {
            choice2 = localCorrect - 1;
            choice3 = localCorrect + Random.Range(1, 5);
        }
        else
        {
            choice2 = localCorrect + 1;
            choice3 = localCorrect - Random.Range(1, 5);
        }

        int[] integerChoices = new int[] { choice1, choice2, choice3, localCorrect };

        answerChoices = ChoicesToStringArray(integerChoices);
        aInput.DisplayChoices(answerChoices);
    }

    public void GenerateSubtractionWP()
    {
        /*
        Debug.Log(currentQuestion.ToString());
        Debug.Log(currentQuestion.variableStrings.Count);
        Debug.Log(currentQuestion.variableStrings[0].array.Count);
        Debug.Log(currentQuestion.ranges.Count);
        Debug.Log(currentQuestion.ranges[0].array.Count);
        */
        numbersInQuestion = new int[currentQuestion.ranges.Count];

        for (int i = 0; i < numbersInQuestion.Length; i++)
        {
            numbersInQuestion[i] = Random.Range(currentQuestion.ranges[i].array[0], currentQuestion.ranges[i].array[1]);
        }

        int localCorrect = numbersInQuestion[0] - numbersInQuestion[1];

        correctAnswer = localCorrect.ToString();

        int choice1;
        int choice2;
        int choice3;
        
        choice1 = numbersInQuestion[0] + numbersInQuestion[1];
        
        int PlusOrMinus = Random.Range(0, 2);
        if (PlusOrMinus == 0)
        {
            choice2 = localCorrect - 1;
            choice3 = localCorrect + Random.Range(1, 5);
        }
        else
        {
            choice2 = localCorrect + 1;
            choice3 = localCorrect - Random.Range(1, 5);
        }

        int[] integerChoices = new int[] { choice1, choice2, choice3, localCorrect };

        answerChoices = ChoicesToStringArray(integerChoices);
        aInput.DisplayChoices(answerChoices);
    }

    public void GenerateMultiplicaitonWP()
    {
        numbersInQuestion = new int[currentQuestion.ranges.Count];

        for (int i = 0; i < numbersInQuestion.Length; i++)
        {
            numbersInQuestion[i] = Random.Range(currentQuestion.ranges[i].array[0], currentQuestion.ranges[i].array[1]);
        }

        int localCorrect = numbersInQuestion[0] * numbersInQuestion[1];

        correctAnswer = localCorrect.ToString();

        int Choice1;
        int Choice2;
        int Choice3;

        Choice1 = numbersInQuestion[0] / numbersInQuestion[2];

        int PlusOrMinus = Random.Range(0, 2);

        if (PlusOrMinus == 0)
        {
            Choice2 = numbersInQuestion[0] * (numbersInQuestion[1] - 1);
            Choice3 = localCorrect + Random.Range(1, 5);
        }
        else
        {
            Choice2 = numbersInQuestion[0] * (numbersInQuestion[1] + 1);
            Choice3 = localCorrect - Random.Range(1, 5);
        }

        int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, localCorrect };

        answerChoices = ChoicesToStringArray(IntegerChoices);
        aInput.DisplayChoices(answerChoices);
    }

    // TODO: Make sure answer is an integer
    public void GenerateDivisionWP()
    {
        numbersInQuestion = new int[currentQuestion.ranges.Count];

        for (int i = 0; i < numbersInQuestion.Length; i++)
        {
            numbersInQuestion[i] = Random.Range(currentQuestion.ranges[i].array[0], currentQuestion.ranges[i].array[1]);
        }

        int localCorrect = numbersInQuestion[0] / numbersInQuestion[1];

        correctAnswer = localCorrect.ToString();

        int Choice1;
        int Choice2;
        int Choice3;

        Choice1 = numbersInQuestion[0] * numbersInQuestion[2];

        int PlusOrMinus = Random.Range(0, 2);

        if (PlusOrMinus == 0)
        {
            if (numbersInQuestion[1] - 1 == 0)
            {
                Choice2 = numbersInQuestion[0] / numbersInQuestion[1];
            }
            else
            {
                Choice2 = numbersInQuestion[0] / (numbersInQuestion[1] - 1);
            }
            Choice3 = localCorrect + Random.Range(1, 5);
        }
        else
        {
            Choice2 = numbersInQuestion[0] / (numbersInQuestion[1] + 1);
            Choice3 = localCorrect - Random.Range(1, 5);
        }

        int[] IntegerChoices = new int[] { Choice1, Choice2, Choice3, localCorrect };

        answerChoices = ChoicesToStringArray(IntegerChoices);
        aInput.DisplayChoices(answerChoices);
    }

    public void GenerateChoices() {
		answerChoices = currQuestion.answerChoices;
		Debug.Log(answerChoices[0]);
		aInput.DisplayChoices(answerChoices);
	}

    // from AddOrSubtract.cs
	public string[] ChoicesToStringArray(int[] IntegerChoices)
    {
        HashSet<int> choiceSet = new HashSet<int>();
        int size = IntegerChoices.Length;

        //Check for duplicate values in array. If found, add a number in a random range
        for (int i = 0; i < size; i++)
        {
            if (choiceSet.Contains(IntegerChoices[i]))
            {
                IntegerChoices[i] += Random.Range(1, 4);
            }
            choiceSet.Add(IntegerChoices[i]);
        }

        //Shuffle array randomly
        for (int i = 0; i < IntegerChoices.Length; i++)
        {
            int temp = IntegerChoices[i];
            int r = Random.Range(i, IntegerChoices.Length);
            IntegerChoices[i] = IntegerChoices[r];
            IntegerChoices[r] = temp;

        }

        //Populate choice array with generated answer choices, converted to strings for later use
        answerChoices = new string[] {IntegerChoices[0].ToString(), IntegerChoices[1].ToString(),
            IntegerChoices[2].ToString(), IntegerChoices[3].ToString()};

        return answerChoices;
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
		return "Word Problem";
	}

    public string GetQuestionSubCategory()
    {
        return currentQuestion.questionType;
    }

    public bool GetAnsweredCorrectly()
    {
        return incorrectAnswers == 0;
    }

    IEnumerator GetQuestions()
    {
        TextAsset text = (TextAsset)Resources.Load<TextAsset>("wordproblemtext");
        if (text != null)
        {
            //Debug.Log(text.text);

            List<WordProblemText> wpt = JsonHelper.FromJson<WordProblemText>(text.text);

            for (int i = 0; i < wpt.Count; i++)
            {
                //Debug.Log(wpt[i].ToString());
                switch (wpt[i].questionType)
                {
                    case "Addition":
                        Addition.Add(wpt[i]);
                        break;
                    case "Subtraction":
                        Subtraction.Add(wpt[i]);
                        break;
                    case "Multiplication":
                        Multiplication.Add(wpt[i]);
                        break;
                    case "Division":
                        Division.Add(wpt[i]);
                        break;
                    default:
                        break;
                }
            }

            yield return null;
        }
        else
        {
            Debug.Log("NULL QUESTION TXT JSON");
        }
        //GenerateQuestion(0);
        Debug.Log(questionString);
        yield return null;
    }

    public void OnEndQuestion()
    {

    }
}

[System.Serializable]
public class WordProblemText
{
    public string questionType;
    public string[] text;
    public string[] replacementType;
    public List<StringArray> variableStrings;
    public List<IntArray> ranges;

    public override string ToString()
    {
        string question = "";
        for(int i = 0; i < text.Length; i++)
        {
            if (i == 0)
            {
                question += text[i];
            }
            else
            {
                question += "<var>" + text[i];
            }
        }
        return question;
    }
}

// temporary structs for jank:
[System.Serializable]
public struct IntArray
{
    public List<int> array;
}

[System.Serializable]
public struct StringArray
{
    public List<string> array;
}