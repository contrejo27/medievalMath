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
	
	//in-game text
	public Text correctText;
	public Text wave;
	public Text hsName;
	public Text hsWave;
	public Text hsAnswers;
	
	public void Start(){
		personalHighScore = PlayerPrefs.GetInt("personalHighScore");

		globalHighScores.Add("JGC,3,8");
		globalHighScores.Add("HBK,2,5");
		globalHighScores.Add("JGC,2,3");

		
	}
	
	public void CorrectlyAnswered()
	{
		correctAnswers += 1;
	}

	public int GetincorrectAnswers() {
		return this.incorrectAnswers;
	}
	
	//gets called when lose screen shows up. displays and saves match stats
	public void SaveState() {
		//set personal high score
		if(correctAnswers > personalHighScore) {
			personalHighScore = correctAnswers;
		}
		CheckHighScores();
		DisplayStats();
		PlayerPrefs.SetInt("personalHighScore", personalHighScore);
	}
	
	void DisplayStats(){
		wave.text = "Wave: " + wManager.CurrentWave +1;
		correctText.text = "Correct: " + correctAnswers.ToString ();
		
		foreach(string score in globalHighScores){
			string[] line = score.Split(',');
			hsName.text += line[0] + "\n";
			hsWave.text += line[1] + "\n";
			hsAnswers.text += line[2] + "\n";
		}
	}
	
	void CheckHighScores(){

	}
}
