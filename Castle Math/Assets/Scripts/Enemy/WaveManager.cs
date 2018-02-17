using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {
		
	//enemies
	public GameObject KnightPrefab;
	public GameObject trollPrefab;
	public GameObject horseRiderPrefab;
	public int firstWaveSize = 1;
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
	public AudioClip WaveCleared;

	//UI
	public Text WaveTitle;
	public CanvasGroup waveEffect;
	public TutorialBehavior interMath;
	public GameObject billboard;
	private int CurrentEnemies;

	
	// Use this for initialization
	void Start () {
		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();
    }

    public void NextWave()
	{
		Mathm.DeactivateInterMath();
		interMath.Deactivate();
		billboard.SetActive(false);

		if(currentWave%2 ==0) {
			Mathm.SetDifficulty();
		}
		
		currentWave += 1;
		ActivateWave (currentWave);
	}

	public void ActivateWave(int WaveIndex)
	{
		//the wave size increases by two men each wave
		WaveSize = firstWaveSize + (int)(WaveIndex * 1.5);
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
			
		SetNumberOfEnemies (WaveSize);
		for (int i = 0; i <  WaveSize; i++) {
			spawnEnemy(KnightPrefab, null);
			yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
		}

		//waves in between trolls
		int trollFrequency = 5;
        if (currentWave % trollFrequency == 0 && currentWave != 0){
        	//trolls increase by 1 every set WtrollFrequency
            for (int i = 0; i < currentWave / trollFrequency; i++) { 
			    spawnEnemy(trollPrefab, trollSpawnSound);
				addEnemyToWaveSize();
                yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));
            }
        }


        if (currentWave % 4 == 0 && currentWave != 0)
        {
            for (int i = 0; i < currentWave / 3; i++)
            {
                spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
				addEnemyToWaveSize();
				
				yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));				
            }
        }
		
		if(currentWave > 12){
		    spawnEnemy(horseRiderPrefab, horseRiderSpawnSound);
			addEnemyToWaveSize();
		}

		if(currentWave > 16){
		    spawnEnemy(trollPrefab, trollSpawnSound);
			addEnemyToWaveSize();
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

	//get enemy count to know when the wave is over
	public void SetNumberOfEnemies(int SizeOfWave)
	{
		CurrentEnemies = SizeOfWave;
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
	
	public void EnemyKilled()
	{
		CurrentEnemies -= 1;
		//if all enemies were killed
		if (CurrentEnemies <= 0) {
			billboard.SetActive(true);
			interMath.Activate();
			Mathm.ActivateInterMath();

			A_Source.clip = WaveCleared;
			A_Source.Play ();
		}
	}

	//used when enemies are added extra from the main footsoldiers
	public void addEnemyToWaveSize()
	{
		CurrentEnemies++;
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
