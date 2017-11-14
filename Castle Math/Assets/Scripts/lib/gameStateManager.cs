using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class gameStateManager : MonoBehaviour {
	public UIEffects mainMenuEffects;
	public UIEffects notificationEffects;

	public GameObject LoseScreen;
	public GameObject MathScreen;
	public GameObject StatScreen;

	public GameObject InsidePoint;

	private int CurrentEnemies;

	public AudioClip WaveCleared;
	public AudioClip LostTheCastle;

	public AudioClip[] CastleScreams;

	private AudioSource A_Source;
	
	public AudioSource music;
	public AudioClip gameplaySong;

	private WaveManager W_Manager;

	private LaunchProjectile Player;

	// Use this for initialization
	void Start () {

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

	public void LoseState()
	{
		//Restart game
		//SceneManager.LoadScene (0);

		A_Source.clip = LostTheCastle;
		A_Source.Play ();

		StartCoroutine (PlaySounds (A_Source.clip.length));

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

	}


	public void SetNumberOfEnemies(int SizeOfWave)
	{
		CurrentEnemies = SizeOfWave;
	}


	IEnumerator PlaySounds(float delay)
	{

		float delayMultiplier = Random.Range (1, 6);

		yield return new WaitForSeconds (delay * delayMultiplier);

		A_Source.clip = CastleScreams [Random.Range (0, CastleScreams.Length)];

		A_Source.Play ();

		StartCoroutine (PlaySounds (A_Source.clip.length));

	}
}
