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
	
	//in-game text
	public Text correctText;
	public Text wave;
	public Text hsName;
	public Text hsWave;
	public Text hsAnswers;
	
	public void Start(){
		getHighScores();
	}
	
	public void CorrectlyAnswered()
	{
		correctAnswers += 1;
	}

	public int GetincorrectAnswers() {
		return this.incorrectAnswers;
	}
	
	void getHighScores(){
		personalHighScore = PlayerPrefs.GetInt("personalHighScore");
		globalHighScores.Add(PlayerPrefs.GetString("globalHS1"));
		globalHighScores.Add(PlayerPrefs.GetString("globalHS2"));
		globalHighScores.Add(PlayerPrefs.GetString("globalHS3"));
	}
	
	//gets called when lose screen shows up. displays and saves match stats
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
	
	//goes through each high score and checks if the current score beats it
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
	
	//displays the scores on stat screen in game
	void DisplayStats(){
		wave.text = "Wave: " + (wManager.currentWave +1).ToString();
		correctText.text = "Correct: " + correctAnswers.ToString ();
		
		foreach(string score in globalHighScores){
			string[] line = score.Split(',');
			hsName.text += line[0] + "\n";
			hsWave.text += line[1] + "\n";
			hsAnswers.text += line[2] + "\n";
		}
	}
	
	//saves highscore to playerprefs
	void SaveHighScores(){
		int i = 1;
		foreach(string score in globalHighScores){
			PlayerPrefs.SetString("globalHS"+i,score);
			i++;
		}
	}

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
