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
    }


    public void NextWave()
	{
		CurrentWave += 1;
		ActivateWave (CurrentWave);
	}

	public void ActivateWave(int WaveIndex)
	{
		//the wave size increases by two men each wave
		WaveSize = FirstWaveSize + (WaveIndex * 3);



		//Create all of the enemies
		StartCoroutine (ActivateEnemies(WaveSize));
	}


	IEnumerator ActivateEnemies(int WaveSize)
	{
		//display the wave number 
		WaveTitle.text = "Wave " + (CurrentWave + 1).ToString ();
		//delay so the player can breather/ do math
		yield return new WaitForSeconds (5);
		int randomSpawn =0;
		int specialCharacters=0;
		for (int i = 0; i <  WaveSize; i++) {
			randomSpawn = Random.Range (0, SpawnPoints.Length);
			Instantiate (KnightPrefab, SpawnPoints[randomSpawn].position + new Vector3(Random.Range(-15, 12), 0,0), SpawnPoints[randomSpawn].rotation);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}

        if (CurrentWave % 3 == 0 && CurrentWave != 0){
            for (int i = 0; i < CurrentWave / 3; i++) { 
                Instantiate(trollPrefab, SpawnPoints[randomSpawn].position, SpawnPoints[randomSpawn].rotation);
				specialCharacters++;
                yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
            }
        }

        if (CurrentWave % 2 == 0 && CurrentWave != 0)
        {
            for (int i = 0; i < CurrentWave / 2; i++)
            {
                Instantiate(horseRiderPrefab, SpawnPoints[randomSpawn].position, SpawnPoints[randomSpawn].rotation);
				specialCharacters++;
                yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
            }
        }


		//Let the game manager know how many enemies were spawned
		GameManager.SetNumberOfEnemies (WaveSize+specialCharacters);
		
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
