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
	
	//in-game text
	public Text correctText;
	public Text wave;

	public void Start(){
		personalHighScore = 100; //temp
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
		
		wave.text = "Wave: " + wManager.CurrentWave;
		correctText.text = "Correct: " + correctAnswers.ToString ();
		PlayerPrefs.SetInt("personalHighScore", personalHighScore);
	}
	
	
}
