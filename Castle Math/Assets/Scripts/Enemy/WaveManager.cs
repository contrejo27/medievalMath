using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour {
		
	//enemies
	public GameObject KnightPrefab;
	public GameObject trollPrefab;
	public GameObject horseRiderPrefab;
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

    public ManaBar powerup;

	//UI
	public Text WaveTitle;
	public Text mathWaveTitle;
	public CanvasGroup waveEffect;
	public TutorialBehavior interMath;
	private int CurrentEnemies;

	int[][] footknightWaves = new int[20][];
	int[][] horseknightWaves = new int[20][];
	int[][] trollWaves = new int[20][];
    public Text floatingText;

    // Use this for initialization
    void Start () {
		A_Source = GameObject.Find ("CastleAudio").GetComponent<AudioSource> ();

		//first integer in array is type of launch (all at once/staggered/waves) second is number of enemies per lane
		footknightWaves[0] = new int[] { 0, 1 };
		footknightWaves[1] = new int[] { 2, 2 };
		footknightWaves[2] = new int[] { 2, 3 };
		footknightWaves[3] = new int[] { 0, 4 };
		footknightWaves[4] = new int[] { 2, 3 };
		footknightWaves[5] = new int[] { 2, 3 };
		footknightWaves[6] = new int[] { 2, 3 };
		footknightWaves[7] = new int[] { 2, 3};
		footknightWaves[8] = new int[] { 2, 3};
		footknightWaves[9] = new int[] { 1, 5};
		footknightWaves[10] = new int[] { 0, 4};
		footknightWaves[11] = new int[] { 2, 4};
		footknightWaves[12] = new int[] { 1, 3};
		footknightWaves[13] = new int[] { 0, 3};
		footknightWaves[14] = new int[] { 2, 5 };
		footknightWaves[15] = new int[] { 1, 4};
		footknightWaves[16] = new int[] { 2, 4};
		footknightWaves[17] = new int[] { 0, 8};
		footknightWaves[18] = new int[] { 1, 4};
		footknightWaves[19] = new int[] { 2, 6};

		horseknightWaves[0] = new int[] { 0, 0 };
		horseknightWaves[1] = new int[] { 0, 0 };
		horseknightWaves[2] = new int[] { 0, 0 };
		horseknightWaves[3] = new int[] { 0, 0 };
		horseknightWaves[4] = new int[] { 0, 1 };
		horseknightWaves[5] = new int[] { 1, 1 };
		horseknightWaves[6] = new int[] { 1, 1 };
		horseknightWaves[7] = new int[] { 0, 0};
		horseknightWaves[8] = new int[] { 1, 2};
		horseknightWaves[9] = new int[] { 3, 1};
		horseknightWaves[10] = new int[] { 1, 0};
		horseknightWaves[11] = new int[] { 1, 3};
		horseknightWaves[12] = new int[] { 1, 0};
		horseknightWaves[13] = new int[] { 1, 0};
		horseknightWaves[14] = new int[] { 0, 2};
		horseknightWaves[15] = new int[] { 0, 3};
		horseknightWaves[16] = new int[] { 0, 1};
		horseknightWaves[17] = new int[] { 0, 0};
		horseknightWaves[18] = new int[] { 3, 3};
		horseknightWaves[19] = new int[] { 2, 2};

		trollWaves[0] = new int[] { 0, 0 };
		trollWaves[1] = new int[] { 0, 0 };
		trollWaves[2] = new int[] { 0, 0 };
		trollWaves[3] = new int[] { 0, 0 };
		trollWaves[4] = new int[] { 0, 0 };
		trollWaves[5] = new int[] { 0, 0 };
		trollWaves[6] = new int[] { 1, 0 };
		trollWaves[7] = new int[] { 0, 0};
		trollWaves[8] = new int[] { 1, 0};
		trollWaves[9] = new int[] { 2, 0};
		trollWaves[10] = new int[] { 1, 0};
		trollWaves[11] = new int[] { 1, 1};
		trollWaves[12] = new int[] { 1, 0};
		trollWaves[13] = new int[] { 1, 2};
		trollWaves[14] = new int[] { 1, 1};
		trollWaves[15] = new int[] { 1, 1};
		trollWaves[16] = new int[] { 1, 2};
		trollWaves[17] = new int[] { 1, 0};
		trollWaves[18] = new int[] { 1, 0};
		trollWaves[19] = new int[] { 0, 3};
    }

    public void NextWave()
	{

        int adjustedCurrentWave = currentWave + 1; //adjusted for 0 being wave 1
        if(adjustedCurrentWave % 5 == 0 && PlayerPrefs.GetInt("LoggedIn") == 1)
        {
            powerup.UpgradeLevel(1);
            print("leveledUp");
            floatingText.text = "LEVEL UP!";
            StartCoroutine(FadingText(floatingText));
        }

		if (currentWave % 2 == 0) {
			Mathm.SetDifficulty ();
		}
		
		currentWave += 1;
		if(currentWave == 19)
		{
            print("newLevel");
			SceneManager.LoadScene("BossLevel" , LoadSceneMode.Single);
		}
		ActivateWave (currentWave);
	}

    IEnumerator FadingText(Text currentText)
    {
        yield return new WaitForSeconds(1.5f);
        currentText.text = "";

    }
    public void ActivateWave(int WaveIndex)
	{
		setWaveText();
		//Create all of the enemies
		if(currentWave != 0){ // add && currentWave < 2
			StartCoroutine (announceWave());
		}
		StartCoroutine (ActivateEnemies(footknightWaves, KnightPrefab,null));
		StartCoroutine (ActivateEnemies(trollWaves, trollPrefab, trollSpawnSound));
		StartCoroutine (ActivateEnemies(horseknightWaves, horseRiderPrefab, horseRiderSpawnSound));

	}

	void setWaveText(){
		WaveTitle.text = "Wave " + (currentWave + 1).ToString () + "/20";
		mathWaveTitle.text = "Wave: " + (currentWave + 1).ToString () + "/20";
		StartCoroutine(FadeOut(waveEffect));
	}
	
	IEnumerator announceWave(){
		yield return new WaitForSeconds (4f);
			A_Source.clip = AnotherWave;
			A_Source.Play ();
	}	

	IEnumerator ActivateEnemies(int[][] waveType, GameObject enemyPrefab,AudioClip spawnSound)
	{		
		//delay so the player can breather/ do math

		yield return new WaitForSeconds (2f);
		//if this wave is all at once
		if(waveType[currentWave][0] == 0)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				spawnEnemy(enemyPrefab, spawnSound,0);
				yield return new WaitForSeconds (.1f);
				spawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (.1f);
				spawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (Random.Range (.1f, .2f));
			}
		}


		//if this wave is staggered
		if(waveType[currentWave][0] == 1)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				spawnEnemy(enemyPrefab, spawnSound,0);
				yield return new WaitForSeconds (Random.Range (0.7f, 1.5f));
				spawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (Random.Range (0.7f, 1.5f));
				spawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (.2f);
			}
		}

		//if this wave is waves
		if(waveType[currentWave][0] == 2)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				spawnEnemy(enemyPrefab, spawnSound,0);
				yield return new WaitForSeconds (Random.Range (.05f, .1f));
				spawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (Random.Range (.05f, .1f));
				spawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (3.5f);
			}
		}

			/*
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
		*/
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
	void spawnEnemy(GameObject enemy, AudioClip spawnSound,int spawn){
		//int randomSpawn = Random.Range(0, SpawnPoints.Length);
		GameObject enemyObject = Instantiate(enemy, SpawnPoints[spawn].position + new Vector3(Random.Range(-15, 10), 0,0), SpawnPoints[0].rotation);
		enemyObject.GetComponent<EnemyBehavior>().SetTarget(spawn+1); //points enemy at right target. adjusted for UI name
		addEnemyToWaveSize();
		//GameObject enemyObject = Instantiate(enemy, SpawnPoints[randomSpawn].position+ new Vector3(Random.Range(-15, 10), 0,0), SpawnPoints[randomSpawn].rotation);

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

			if((currentWave+2) % 3 == 0 && currentWave+2 > 1){
				Mathm.ActivateInterMath();
			}
			else{
				NextWave();
			}

			A_Source.clip = WaveCleared;
			A_Source.Play ();
		}
	}

	//used when enemies are added extra from the main footsoldiers
	public void addEnemyToWaveSize()
	{
		CurrentEnemies++;
	}

	public int GetCurrentWaveNumber() {
		return currentWave;
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