using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//This script should contain anything that activates or deactivates things between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {
	
	//UI
	public UIEffects mainMenuEffects;
	public UIEffects notificationEffects;
	public GameObject LoseScreen;
	public GameObject MathScreen;
	public GameObject StatScreen;
	private string playerName = "JGC";

	//enemy behavior
	public GameObject InsidePoint;
	private int CurrentEnemies;
	private WaveManager W_Manager;

	//audio
	public AudioClip WaveCleared;
	public AudioClip LostTheCastle;
	public AudioClip[] CastleScreams;
	private AudioSource A_Source;
	public AudioSource music;
	public AudioClip gameplaySong;
	public PlayerMathStats playerMathStats;
	
	//Environment
	private LaunchProjectile Player;
	public Light directionalLight;
	private bool loseState = false;
	public doorHealth fence1,fence2,fence3;
	
	// Use this for initialization
	void Start () {
		RenderSettings.skybox.SetFloat("_Exposure", 1.0f); //reset exposure
		Player = GameObject.FindObjectOfType<LaunchProjectile> (); 
		W_Manager = GameObject.FindObjectOfType<WaveManager> ();
		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();
		mainMenuEffects.fadeIn (.4f);
		
		//first time game is opened sets up initial playerPref values
		if(!PlayerPrefs.HasKey("isFirstTime"))
		{
			// Set and save all your PlayerPrefs here.
			// Now set the value of isFirstTime to be false in the PlayerPrefs.
			PlayerPrefs.SetInt("isFirstTime", 1);
			PlayerPrefs.SetString("globalHS1","JGC,3,8");
			PlayerPrefs.SetString("globalHS2","HBK,2,5");
			PlayerPrefs.SetString("globalHS3","JGC,2,3");
			PlayerPrefs.Save();
		}
	}

	public void StartGame(){
		mainMenuEffects.fadeOut(1.5f);
		notificationEffects.fadeIn(1.5f);
		music.clip = gameplaySong;
		music.Play ();
	}

	public void EnemyKilled()
	{
		CurrentEnemies -= 1;
		//if all enemies were killed
		if (CurrentEnemies <= 0) {
			W_Manager.NextWave();

			A_Source.clip = WaveCleared;
			A_Source.Play ();
		}
	}

	public void LoseState(){
	
/*
		music.Stop ();
		music.clip = LostTheCastle;
		music.Play ();*/
		if(!loseState){
			loseState = true;
		}
		SaveGame();

		Player.isAlive = false;
		
		//set UI
		LoseScreen.SetActive (true);
		MathScreen.SetActive (false);
		StatScreen.SetActive (true);

		//change enemy target so they start running
		EnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnemyBehavior> ();
		for (int i = 0; i < Enemies.Length; i++) {
			Enemies [i].Target = InsidePoint;
			Enemies [i].gameObject.transform.parent = null;
		}
		fadeWorldOut();
	}

	public void Retry()
	{
		loseState = false;
		RenderSettings.skybox.SetFloat("_Exposure", 1.0f);
		SceneManager.LoadScene (0);

	}

	public void Quit()
	{
		Application.Quit ();
	}
	
	void fadeWorldOut(){
		StartCoroutine(fadeSky(1.0f,0.0f));
		StartCoroutine(fadeLight(false));

	}
	
	void fadeWorldIn(){
		StartCoroutine(fadeSky(0.0f,1.0f));
		StartCoroutine(fadeLight(true));
	}
	
	IEnumerator fadeSky(float initialValue, float endValue)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
		{
			RenderSettings.skybox.SetFloat("_Exposure",  Mathf.Lerp(initialValue,endValue,t));
			yield return null;
		}
	}
	
	//fades light when you lose
	IEnumerator fadeLight(bool fadeIn)
	{
		Color sunLightColor =  directionalLight.color;
		Color ambientLightColor = RenderSettings.ambientSkyColor;
		Color currentColor =  RenderSettings.fogColor;

		if(fadeIn){
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				directionalLight.color = Color.Lerp(sunLightColor,new Color(.93f,.87f,.76f), t);
				RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor,new Color(.93f,.87f,.76f), t);
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.98f,.918f,.7f), t);
				yield return null;
			}
		}
		else{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.01f,.02f,.03f), t);
				RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor,new Color(.05f,.06f,.08f), t);
				directionalLight.color = Color.Lerp(sunLightColor,new Color(.05f,.06f,.08f), t);
				yield return null;
			}
		}
	}

	
	//get enemy count to know when the wave is over
	public void SetNumberOfEnemies(int SizeOfWave)
	{
		CurrentEnemies = SizeOfWave;
	}

	//used when enemies are added extra from the main footsoldiers
	public void addEnemyToWaveSize()
	{
		CurrentEnemies++;
	}
	
	//goes through different scripts and saves info
	public void SaveGame(){
		playerMathStats.SaveState();
		PlayerPrefs.SetString("PlayerName", playerName);
	}
}
