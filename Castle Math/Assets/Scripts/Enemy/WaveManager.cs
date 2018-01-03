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
	public int firstWaveSize = 5;
	private int WaveSize;
	public int currentWave = 0;
	
	//Environment 
	public Transform[] SpawnPoints;
	public MathManager Mathm;
	//Audio
	public AudioClip AnotherWave;
	private AudioSource A_Source;
	public AudioSource enemySounds;
	public AudioClip horseRiderSpawnSound;
	public AudioClip trollSpawnSound;

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
		if(currentWave%2 ==0)		Mathm.increaseMathDifficulty();
		currentWave += 1;
		ActivateWave (currentWave);
	}

	public void ActivateWave(int WaveIndex)
	{
		//the wave size increases by two men each wave
		WaveSize = firstWaveSize + (WaveIndex * 2);
		setWaveText();
		//Create all of the enemies
		StartCoroutine (ActivateEnemies(WaveSize));
	}

	void setWaveText(){
		WaveTitle.text = "Wave " + (currentWave + 1).ToString ();
		StartCoroutine(FadeOut(waveEffect));
	}
	
	IEnumerator ActivateEnemies(int WaveSize)
	{		
		//delay so the player can breather/ do math
		yield return new WaitForSeconds (4f);
		if(currentWave != 0){ // add && currentWave < 2
		A_Source.clip = AnotherWave;
		A_Source.Play ();
		}
			
		GameManager.SetNumberOfEnemies (WaveSize);
		for (int i = 0; i <  WaveSize; i++) {
			spawnEnemy(KnightPrefab, null);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}

        if (currentWave % 6 == 0 && currentWave != 0){
            for (int i = 0; i < currentWave / 6; i++) { 
			    spawnEnemy(trollPrefab, trollSpawnSound);
				GameManager.addEnemyToWaveSize();

                yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));
            }
        }

        if (currentWave % 4 == 0 && currentWave != 0)
        {
            for (int i = 0; i < currentWave / 4; i++)
            {
                spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
				GameManager.addEnemyToWaveSize();
				
				yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));				
            }
        }
		
		if(currentWave > 8){
		    spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
			GameManager.addEnemyToWaveSize();
		}

		if(currentWave > 10){
		    spawnEnemy(trollPrefab, trollSpawnSound);
			GameManager.addEnemyToWaveSize();
		}
		
		//leave the title up for another second
		yield return new WaitForSeconds (1);

		WaveTitle.text = "";
		//waveEffect.alpha = 1f;
	}

	public void ResetWave()
	{
		currentWave = 0;
	}
	
	//spawns enemy at a random spawn point
	void spawnEnemy(GameObject enemy, AudioClip spawnSound){
		int randomSpawn = Random.Range(0, SpawnPoints.Length);
		GameObject enemyObject = Instantiate(enemy, SpawnPoints[randomSpawn].position+ new Vector3(Random.Range(-15, 10), 0,0), SpawnPoints[randomSpawn].rotation);
		enemyObject.GetComponent<EnemyBehavior>().SetTarget(randomSpawn + 1); //adjusted for UI name
	
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
