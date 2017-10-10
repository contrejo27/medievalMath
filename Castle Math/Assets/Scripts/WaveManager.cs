using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {


	public GameObject KnightPrefab;
	public GameObject trollPrefab;
	public GameObject horseRiderPrefab;

	public Transform[] SpawnPoints;

	public int FirstWaveSize = 8;

	public AudioClip AnotherWave;

	private int WaveSize;
	private int CurrentWave = 0;

	private gameStateManager GameManager;

	private TextMesh WaveTitle;

	private AudioSource A_Source;

	// Use this for initialization
	void Start () {

		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();
		WaveTitle = this.transform.GetChild (0).gameObject.GetComponent<TextMesh> ();
		GameManager = GameObject.FindObjectOfType<gameStateManager> ();
		//ActivateWave (0);
	}


	public void NextWave()
	{
		CurrentWave += 1;
		ActivateWave (CurrentWave);
	}

	public void ActivateWave(int WaveIndex)
	{
		//the wave size increases by two men each wave
		WaveSize = FirstWaveSize + (WaveIndex * 4);

		//Let the game manager know how many enemies were spawned
		GameManager.SetNumberOfEnemies (WaveSize);

		//Create all of the enemies
		StartCoroutine (ActivateEnemies(WaveSize));
	}


	IEnumerator ActivateEnemies(int WaveSize)
	{
		//display the wave number 
		WaveTitle.text = "Wave " + (CurrentWave + 1).ToString ();
		//a 2 second delay so the player can breather/ do math
		yield return new WaitForSeconds (5);
		int randomSpawn =0;

		for (int i = 0; i <  WaveSize; i++) {

			randomSpawn = Random.Range (0, SpawnPoints.Length);

			Instantiate (KnightPrefab, SpawnPoints[randomSpawn].position, SpawnPoints[randomSpawn].rotation);

			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}
					
		//this should randomly spawn enemies, the more waves the more trolls will show up
		Instantiate (trollPrefab, SpawnPoints[2].position, SpawnPoints[2].rotation);
		Instantiate (horseRiderPrefab, SpawnPoints[1].position, SpawnPoints[1].rotation);

		
		//leave the title up for another second
		yield return new WaitForSeconds (1);

		//play another wave audio
		if (CurrentWave != 0) {
			A_Source.clip = AnotherWave;
			A_Source.Play ();
		}

		WaveTitle.text = "";

	}

}
