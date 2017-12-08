using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//This script should contain anything that activates or deactivates things between state changes like lose or next wave
public class gameStateManager : MonoBehaviour {
	
	//UI
	public UIEffects mainMenuEffects;
	public UIEffects notificationEffects;
	public GameObject LoseScreen;
	public GameObject MathScreen;
	public GameObject StatScreen;

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

	//Environment
	private LaunchProjectile Player;
	public Light directionalLight;
	
	// Use this for initialization
	void Start () {
		RenderSettings.skybox.SetFloat("_Exposure", 1.0f);

		Player = GameObject.FindObjectOfType<LaunchProjectile> ();

		W_Manager = GameObject.FindObjectOfType<WaveManager> ();

		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();

		mainMenuEffects.fadeIn (.4f);
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
			//you defeated a wave!!!
		}

	}

	public void LoseState(){
	
/*
		music.Stop ();
		music.clip = LostTheCastle;
		music.Play ();*/

		Player.isAlive = false;
		LoseScreen.SetActive (true);
		MathScreen.SetActive (false);
		StatScreen.SetActive (true);

		EnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnemyBehavior> ();

		for (int i = 0; i < Enemies.Length; i++) {
			Enemies [i].Target = InsidePoint;
			//Enemies [i].AtTarget = false;
			Enemies [i].gameObject.transform.parent = null;
		}
		
		fadeWorldOut();
	}

	public void Retry()
	{
		RenderSettings.skybox.SetFloat("_Exposure", 1.0f);
		SceneManager.LoadScene (0);/*
		fadeWorldIn();
		W_Manager.ResetWave();
		Player.isAlive = true;
		LoseScreen.SetActive (false);
		MathScreen.SetActive (true);
		StatScreen.SetActive (false);*/
	}

	public void Quit()
	{
		Application.Quit ();
	}
	
	//all the fading functionality will need refactoring 
	void fadeWorldOut(){
		StartCoroutine(fadeSky(1.0f,0.0f));
		StartCoroutine(fadeFog(false));
		StartCoroutine(fadeLight(false));
		StartCoroutine(fadeAmbient(false));

	}
	
	void fadeWorldIn(){
		StartCoroutine(fadeSky(0.0f,1.0f));
		StartCoroutine(fadeFog(true));
		StartCoroutine(fadeLight(true));
		StartCoroutine(fadeAmbient(true));
	}
	
	IEnumerator fadeSky(float initialValue, float endValue)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
		{
			RenderSettings.skybox.SetFloat("_Exposure",  Mathf.Lerp(initialValue,endValue,t));
			yield return null;
		}
	}
	
	IEnumerator fadeFog(bool fadeIn)
	{
		Color currentColor =  RenderSettings.fogColor;
		if(fadeIn){
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.98f,.918f,.7f), t);
				yield return null;
			}
		}
		else{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.01f,.02f,.03f), t);
				yield return null;
			}
		}
	}
	
	IEnumerator fadeLight(bool fadeIn)
	{
		Color currentColor =  directionalLight.color;

		if(fadeIn){
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				directionalLight.color = Color.Lerp(currentColor,new Color(.93f,.87f,.76f), t);
				yield return null;
			}
		}
		else{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				directionalLight.color = Color.Lerp(currentColor,new Color(.05f,.06f,.08f), t);
				yield return null;
			}
		}
	}
	
	IEnumerator fadeAmbient(bool fadeIn)
	{
		Color currentColor = RenderSettings.ambientSkyColor;
		if(fadeIn){
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.ambientSkyColor = Color.Lerp(currentColor,new Color(.93f,.87f,.76f), t);
				yield return null;
			}
		}
		else{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.ambientSkyColor = Color.Lerp(currentColor,new Color(.05f,.06f,.08f), t);
				yield return null;
			}
		}
	}
	
	public void SetNumberOfEnemies(int SizeOfWave)
	{
		CurrentEnemies = SizeOfWave;
	}

	public void addEnemyToWaveSize()
	{
		CurrentEnemies++;
	}

}
