using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {
	public GameObject KnightPrefab;
	public GameObject trollPrefab;
	public GameObject horseRiderPrefab;

	public CanvasGroup waveEffect;
	public Transform[] SpawnPoints;

	public int FirstWaveSize = 8;

	public AudioClip AnotherWave;

	private int WaveSize;
	private int CurrentWave = 0;

	private gameStateManager GameManager;

	public Text WaveTitle;

	private AudioSource A_Source;
	
	public AudioSource enemySounds;
	public AudioClip horseRiderSpawn;
	
	// Use this for initialization
	void Start () {

		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();
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
		setWaveText();
		//Create all of the enemies
		StartCoroutine (ActivateEnemies(WaveSize));
	}

	void setWaveText(){
		WaveTitle.text = "Wave " + (CurrentWave + 1).ToString ();
		StartCoroutine(FadeOut(waveEffect));
	}
	
	IEnumerator ActivateEnemies(int WaveSize)
	{
		//display the wave number 
		
		//delay so the player can breather/ do math
		yield return new WaitForSeconds (5);
		int randomSpawn =0;
		int specialCharacters=0;
		for (int i = 0; i <  WaveSize; i++) {
			spawnEnemy(KnightPrefab, null);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}

        if (CurrentWave % 3 == 0 && CurrentWave != 0){
            for (int i = 0; i < CurrentWave / 3; i++) { 
			    spawnEnemy(trollPrefab, horseRiderSpawn);
				specialCharacters++;
                yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
            }
        }

        if (CurrentWave % 2 == 0 && CurrentWave != 0)
        {
            for (int i = 0; i < CurrentWave / 2; i++)
            {
                spawnEnemy(horseRiderPrefab, horseRiderSpawn);
				specialCharacters++;
				
				yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
				
            }
        }
		
		if(CurrentWave > 3){
		    spawnEnemy(horseRiderPrefab, horseRiderSpawn);
				specialCharacters++;
		}

		if(CurrentWave > 5){
		    spawnEnemy(trollPrefab, horseRiderSpawn);
				specialCharacters++;
		}
		//Let the game manager know how many enemies were spawned
		GameManager.SetNumberOfEnemies (WaveSize+specialCharacters);
		specialCharacters =0;
		//leave the title up for another second
		yield return new WaitForSeconds (1);

		//play another wave audio
		if (CurrentWave != 0) {
			A_Source.clip = AnotherWave;
			A_Source.Play ();
		}
		WaveTitle.text = "";
		//waveEffect.alpha = 1f;
	}

	void spawnEnemy(GameObject enemy, AudioClip spawnSound){
		int randomSpawn = Random.Range(0, SpawnPoints.Length);
		Instantiate(enemy, SpawnPoints[randomSpawn].position+ new Vector3(Random.Range(-15, 12), 0,0), SpawnPoints[randomSpawn].rotation);
		if(spawnSound != null){
			enemySounds.clip = spawnSound;
			enemySounds.Play ();
		}
	}
	
	private YieldInstruction fadeInstruction = new YieldInstruction();

	IEnumerator FadeOut(CanvasGroup image)
	{
		yield return new WaitForSeconds (1f);
		float elapsedTime = 0.0f;
		float totalTime =2f;
		while (elapsedTime < totalTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime ;
			image.alpha = 1.0f - Mathf.Clamp01(elapsedTime / totalTime);
		}
	}
}
