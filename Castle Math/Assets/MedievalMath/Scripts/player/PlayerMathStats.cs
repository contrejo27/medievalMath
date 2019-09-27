using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMathStats : MonoBehaviour
{
    /// <summary>
    /// Controls player and global stats and displays them on scoreboard. 
    /// </summary>

    public WaveManager wManager;
    public LevelManager LvlManager;

    // stats
    public int correctAnswers;
    public int incorrectAnswers;
    public bool isCorrect;
    public int attempts;
    public int gradeNumber;
    public int personalHighScore;
    public List<string> globalHighScores = new List<string>();

    public int AddScore = 0;
    public int SubtractScore = 0;
    public int MultiOrDivideScore = 0;
    public int CompareScore = 0;
    public int TrueOrFalseScore = 0;
    public int FractionScore = 0;

    public ArrowSupplier aSupplier;

    // in-game UI
    public Text correctText;
    public Text incorrectText;

    public Text wave;
    public Text hsName;
    public Text hsWave;
    public Text hsAnswers;
    public Text grade;
    public Text towerWave;
    public GameObject tower;
    public GameObject winUI;
    public GameObject ChooseLevelEndUI;
    public Animator Anim;

    public void Start()
    {
        globalHighScores = new List<string>();
        getHighScores();
        Anim = winUI.GetComponent<Animator>();
    }

    public void CorrectlyAnswered()
    {
        correctAnswers += 1;
        isCorrect = true;
    }

    public void IncorrectlyAnswered()
    {
        incorrectAnswers += 1;
        if (isCorrect)
        {
            attempts = 0;
            isCorrect = false;
        }
        attempts += 1;

    }

    void getHighScores()
    {
        personalHighScore = PlayerPrefs.GetInt("personalHighScore");
        globalHighScores.Add(PlayerPrefs.GetString("globalHS1"));
        globalHighScores.Add(PlayerPrefs.GetString("globalHS2"));
        globalHighScores.Add(PlayerPrefs.GetString("globalHS3"));
    }

    public void SaveState()
    {
        /// <summary>
        /// gets called when lose screen shows up. displays and saves match stats
        /// </summary>

        if (correctAnswers > personalHighScore)
        {
            personalHighScore = correctAnswers;
        }

        UpdateHighScores();
        DisplayStats();
        SaveHighScores();
        PlayerPrefs.SetInt("personalHighScore", personalHighScore);
        PlayerPrefs.Save();
    }

    void UpdateHighScores()
    {
        /// <summary>
        /// goes through each high score and checks if the current score beats it
        /// </summary>

        int i = 0;
        int waveNum = wManager.currentWave + 1;
        foreach (string score in globalHighScores)
        {
            string[] line = score.Split(',');

            if (int.Parse(line[2]) < personalHighScore)
            {
                globalHighScores.Insert(i, "HBK," + waveNum.ToString() + "," + correctAnswers.ToString());
                globalHighScores.RemoveAt(3);
                break;
            }
            i++;
        }
    }

    void DisplayStats()
    {
        /// <summary>
        /// displays the scores on stat screen in game
        /// </summary>

        int towerStep = 22 * wManager.currentWave + 1;
        tower.transform.localPosition = new Vector3(tower.transform.localPosition.x + towerStep, tower.transform.localPosition.y, tower.transform.localPosition.z);
        towerWave.text = (wManager.currentWave + 1).ToString();
        wave.text = "Wave: " + (wManager.currentWave + 1).ToString();
        correctText.text = correctAnswers.ToString();
        incorrectText.text = incorrectAnswers.ToString();

        gradeNumber = (int)correctAnswers * 100 / (correctAnswers + incorrectAnswers);

        if (gradeNumber > 94)
        {
            grade.text = "A+";
        }
        else if (gradeNumber > 89)
        {
            grade.text = "A";
        }
        else if (gradeNumber > 84)
        {
            grade.text = "B+";
        }
        else if (gradeNumber > 79)
        {
            grade.text = "B";
        }
        else if (gradeNumber > 69)
        {
            grade.text = "C";
        }
        else
        {
            grade.text = "D";
        }

        foreach (string score in globalHighScores)
        {
            string[] line = score.Split(',');
            hsName.text += line[0] + "\n";
            hsWave.text += line[1] + "\n";
            hsAnswers.text += line[2] + "\n";
        }
    }

    void SaveHighScores()
    {
        /// <summary>
        /// saves highscore to playerprefs
        /// </summary>

        int i = 1;
        foreach (string score in globalHighScores)
        {
            PlayerPrefs.SetString("globalHS" + i, score);
            i++;
        }
    }

    public void UpdateScores(Question question, int attemptScore)
    {
        /// <summary>
        /// Updates the scores based on how many times the question was attempted. Based on QuestionTracker
        /// </summary>
        /// <param name="question">Question.</param>
        /// <param name="attemptScore">Number of times a given question was attempted. Negative for incorrect, positive for correct</param>

        if (Object.ReferenceEquals(question.GetType(), typeof(Add)))
        {
            AddScore += attemptScore;
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Subtract)))
        {
            SubtractScore += attemptScore;
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(MultiplyOrDivide)))
        {
            MultiOrDivideScore += attemptScore;
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Fractions)))
        {
            FractionScore += attemptScore;
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(Compare)))
        {
            CompareScore += attemptScore;
        }
        else if (Object.ReferenceEquals(question.GetType(), typeof(TrueOrFalse)))
        {
            TrueOrFalseScore += attemptScore;
        }
    }


    IEnumerator loadNextScreen()
    {
        /// <summary>
        /// Load Stat Screen
        /// </summary>

        yield return new WaitForSeconds(4f);
        winUI.SetActive(false);
        ChooseLevelEndUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        LvlManager.unlockNextGameMode();
    }

    public void showWinUI()
    {
        winUI.SetActive(true);
        Anim.Play("scaleUp");
        SaveState();
        StartCoroutine(loadNextScreen());
    }
}
