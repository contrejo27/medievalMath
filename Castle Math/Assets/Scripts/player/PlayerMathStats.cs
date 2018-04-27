using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls player and global stats and displays them on scoreboard. 
public class PlayerMathStats : MonoBehaviour {
	public WaveManager wManager;
	
	//stats
	int correctAnswers;
    int incorrectAnswers;

    int personalHighScore;
	List<string> globalHighScores = new List<string>();

	private int AddOrSubtractScore = 0;
	private int MultiOrDivideScore = 0;
	private int CompareScore = 0;
	private int TrueOrFalseScore = 0;
	private int FractionScore = 0;
	
	//in-game UI
	public Text correctText;
	public Text wave;
	public Text hsName;
	public Text hsWave;
	public Text hsAnswers;
    public Text grade;
    public Text towerWave;
    public GameObject tower;

    public void Start(){
		getHighScores();
	}
	
	public void CorrectlyAnswered()
	{
		correctAnswers += 1;
	}

    public void IncorrectlyAnswered()
    {
        incorrectAnswers += 1;
    }
    void getHighScores(){
		personalHighScore = PlayerPrefs.GetInt("personalHighScore");
		globalHighScores.Add(PlayerPrefs.GetString("globalHS1"));
		globalHighScores.Add(PlayerPrefs.GetString("globalHS2"));
		globalHighScores.Add(PlayerPrefs.GetString("globalHS3"));
	}
	
	/// <summary>
	/// gets called when lose screen shows up. displays and saves match stats
	/// </summary>
	public void SaveState() {
		//set personal high score
		if(correctAnswers > personalHighScore) {
			personalHighScore = correctAnswers;
		}
		UpdateHighScores();
		DisplayStats();
		SaveHighScores();
		PlayerPrefs.SetInt("personalHighScore", personalHighScore);
		PlayerPrefs.Save();
	}
	
	/// <summary>
	/// goes through each high score and checks if the current score beats it
	/// </summary>
	void UpdateHighScores(){
		int i = 0;
		int waveNum = wManager.currentWave +1;
		foreach(string score in globalHighScores){
			string[] line = score.Split(',');

			if(int.Parse(line[2]) < personalHighScore){
				globalHighScores.Insert(i,"HBK," + waveNum.ToString() + "," + correctAnswers.ToString ());
				globalHighScores.RemoveAt(3);
				break;
			}				
			i++;
		}
	}
	
	/// <summary>
	/// displays the scores on stat screen in game
	/// </summary>
	void DisplayStats(){
        int towerStep = 22 * wManager.currentWave + 1;
        tower.transform.localPosition = new Vector3(tower.transform.localPosition.x + towerStep, tower.transform.localPosition.y, tower.transform.localPosition.z);
        towerWave.text = (wManager.currentWave + 1).ToString();
        wave.text = "Wave: " + (wManager.currentWave +1).ToString();
		correctText.text = "Correct: " + correctAnswers.ToString ();
        int gradeNumber = (int)correctAnswers * 100 / (correctAnswers + incorrectAnswers);
        grade.text = gradeNumber.ToString() + "%";

        foreach (string score in globalHighScores){
			string[] line = score.Split(',');
			hsName.text += line[0] + "\n";
			hsWave.text += line[1] + "\n";
			hsAnswers.text += line[2] + "\n";
		}
	}
	
	/// <summary>
	/// saves highscore to playerprefs
	/// </summary>
	void SaveHighScores(){
		int i = 1;
		foreach(string score in globalHighScores){
			PlayerPrefs.SetString("globalHS"+i,score);
			i++;
		}
	}

	/// <summary>
	/// Updates the scores based on how many times the question was attempted. Based on QuestionTracker
	/// </summary>
	/// <param name="question">Question.</param>
	/// <param name="attemptScore">Number of times a given question was attempted. Negative for incorrect, positive for correct</param>
	public void UpdateScores(Question question, int attemptScore) {
		if (Object.ReferenceEquals (question.GetType (), typeof(AddOrSubtract))) {
			AddOrSubtractScore += attemptScore;
		} else if (Object.ReferenceEquals (question.GetType (), typeof(MultiplyOrDivide))) {
			MultiOrDivideScore += attemptScore;
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Fractions))) {
			FractionScore += attemptScore;
		} else if (Object.ReferenceEquals (question.GetType (), typeof(Compare))) {
			CompareScore += attemptScore;
		} else if (Object.ReferenceEquals (question.GetType (), typeof(TrueOrFalse))) {
			TrueOrFalseScore += attemptScore;	
		}
	}
	
}
