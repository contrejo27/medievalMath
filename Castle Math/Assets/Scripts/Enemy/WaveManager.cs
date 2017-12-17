using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {
	
	private GameStateManager GameManager;
	
	//enemies
	public GameObject KnightPrefab;
	public GameObject trollPrefab;
	public GameObject horseRiderPrefab;
	public int FirstWaveSize = 6;
	private int WaveSize;
	public int CurrentWave = 0;
	
	
	//Environment 
	public Transform[] SpawnPoints;

	//Audio
	public AudioClip AnotherWave;
	private AudioSource A_Source;
	public AudioSource enemySounds;
	public AudioClip horseRiderSpawnSound;

	//UI
	public Text WaveTitle;
	public CanvasGroup waveEffect;


	
	// Use this for initialization
	void Start () {

		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();
		GameManager = GameObject.FindObjectOfType<GameStateManager> ();
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
		yield return new WaitForSeconds (4f);
		if(CurrentWave != 0){
		A_Source.clip = AnotherWave;
		A_Source.Play ();
		}
			
		int randomSpawn =0;
		GameManager.SetNumberOfEnemies (WaveSize);
		for (int i = 0; i <  WaveSize; i++) {
			spawnEnemy(KnightPrefab, null);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}

        if (CurrentWave % 3 == 0 && CurrentWave != 0){
            for (int i = 0; i < CurrentWave / 3; i++) { 
			    spawnEnemy(trollPrefab, horseRiderSpawnSound);
				GameManager.addEnemyToWaveSize();

                yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
            }
        }

        if (CurrentWave % 2 == 0 && CurrentWave != 0)
        {
            for (int i = 0; i < CurrentWave / 2; i++)
            {
                spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
				GameManager.addEnemyToWaveSize();
				
				yield return new WaitForSeconds(Random.Range(0.2f, 1.8f));
                randomSpawn = Random.Range(0, SpawnPoints.Length);
				
            }
        }
		
		if(CurrentWave > 3){
		    spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
			GameManager.addEnemyToWaveSize();
		}

		if(CurrentWave > 5){
		    spawnEnemy(trollPrefab, horseRiderSpawnSound);
			GameManager.addEnemyToWaveSize();
		}
		
		//leave the title up for another second
		yield return new WaitForSeconds (1);

		WaveTitle.text = "";
		//waveEffect.alpha = 1f;
	}

	public void ResetWave()
	{
		CurrentWave = 0;
	}
	void spawnEnemy(GameObject enemy, AudioClip spawnSound){
		int randomSpawn = Random.Range(0, SpawnPoints.Length);
		Instantiate(enemy, SpawnPoints[randomSpawn].position+ new Vector3(Random.Range(-30, 20), 0,0), SpawnPoints[randomSpawn].rotation);
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
