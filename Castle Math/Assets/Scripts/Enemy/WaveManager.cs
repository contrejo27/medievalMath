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
    public int currentWave;
    public int finalWave;
	public GameObject storyText;
    
    //temp
    public GameObject billboardCanvas;
    public GameObject billboardTutorialImage;

    //Environment 
    public Transform[] SpawnPoints;
    public Transform[] fenceTargets;
	public MathManager Mathm;
    public GameObject billboard;
    public GameStateManager gManager;
    public MathManager mManager;
    public Transform gemSpawnPoint;

    //Audio
    public AudioClip AnotherWave;
	private AudioSource A_Source;
	public AudioSource enemySounds;
    private AudioSource music;

    public AudioClip trollsUnleashed;
    public AudioClip winSound;
    public AudioClip horseRiderSpawnSound;
    public AudioClip halfwayThrough;

    public AudioClip trollSpawnSound;
	public AudioClip WaveCleared;

    public ManaBar powerup;

    public float spawnAdjustmentMin = -15;
    public float spawnAdjustmentMax = 10;

	public bool levelComplete;

    //UI
    public Text WaveTitle;
	public Text mathWaveTitle;
	public CanvasGroup waveEffect;
	public TutorialBehavior interMath;
	public int CurrentEnemies;
    public PlayerMathStats mStats;

	//2D integer Array to determine parameters of waves for the 3 enemy types
	int[][] footknightWaves = new int[20][];
	int[][] horseknightWaves = new int[20][];
	int[][] trollWaves = new int[20][];
	//floating text
    public Text floatingText;


    // Use this for initialization
    void Start () {

		levelComplete = false;

        if (!Application.isEditor)
            currentWave = 0;

        A_Source = GameObject.Find("CastleAudio").GetComponent<AudioSource>();

        music = GameObject.Find ("Music").GetComponent<AudioSource> ();
        GameStateManager.instance.waveManager = this;
        //first integer in array is type of launch (0 all at once/1 staggered/2 waves/3 singles) second is number of enemies per lane
        int allAtOnce = 0;
        int staggered = 1;
        int waves = 2;
        int singles = 3;

		//Does a check to see what level/scene it is and what wave data to fetch from csv
		//checks if current scene is MathTest (Kells)
		if (SceneManager.GetActiveScene().name == "MathTest") {
			//get Wave csv for kells
			//TextAsset waveDat = Resources.Load<TextAsset>("waves/kells_Wave.csv");

			//Finds text file of the following name in waves folder for the use of the function
			TextAsset waveDat = Resources.Load("waves/kells_Wave", typeof(TextAsset)) as TextAsset;

			//each row is split with a '\n' a.k.a an enter to a new row
			string[] data = waveDat.text.Split (new char[] { '\n' });


			//declares wave integer as zero for the foreach loop
			//the foreach loop proceeds to move through the text file found earlier
			int wave = 0;
			foreach (string a in data) {
				//splits invidiual data based on the presence of a comma
				string[] waveInfo = a.Split(',');
				
				print("LINE NUMBER: " + wave);
				print (waveInfo [2]);
				print (int.Parse(waveInfo[2]));
				print (waveInfo [1]);
				print (int.Parse(waveInfo[1]));
				
				//for footknights/horseknights/trolls of each wave (the current number), a new integer is declared with the wave type and amount of waves
				footknightWaves [wave] = new int[] { int.Parse (waveInfo [2]), int.Parse (waveInfo [1]) };
				horseknightWaves [wave] = new int[] { int.Parse (waveInfo [4]), int.Parse (waveInfo [3]) };
				trollWaves [wave] = new int[] { int.Parse (waveInfo [6]), int.Parse (waveInfo [5]) };
				//increments wave, moving to the next row of the grid
				wave++;
			}
		}
		//checks if current scene is frost
		if (SceneManager.GetActiveScene().name == "frostLevel") {
			//get Wave csv for frost
			//Finds text file of the following name in waves folder for the use of the function
			TextAsset waveDat = Resources.Load("waves/frost_Wave", typeof(TextAsset)) as TextAsset;
			//each row is split with a '\n' a.k.a an enter to a new row
			string[] data = waveDat.text.Split (new char[] { '\n' });
			//declares wave integer as zero for the foreach loop
			//the foreach loop proceeds to move through the text file found earlier
			int wave = 0;
			foreach (string a in data) {
				//splits invidiual data based on the presence of a comma
				string[] waveInfo = a.Split(',');

				print("LINE NUMBER: " + wave);
				print (waveInfo [2]);
				print (int.Parse(waveInfo[2]));
				print (waveInfo [1]);
				print (int.Parse(waveInfo[1]));

				//for footknights/horseknights/trolls of each wave (the current number), a new integer is declared with the wave type and amount of waves
				footknightWaves [wave] = new int[] { int.Parse(waveInfo[2]), int.Parse(waveInfo[1]) };
				horseknightWaves [wave] = new int[] { int.Parse (waveInfo [4]), int.Parse (waveInfo [3]) };
				trollWaves [wave] = new int[] { int.Parse (waveInfo [6]), int.Parse (waveInfo [5]) };
				//increments wave, moving to the next row of the grid
				wave++;
			}
		}
		//checks if current scene is desert
		if (SceneManager.GetActiveScene().name == "desertLevel") {
			//get Wave csv for desert
			//Finds text file of the following name in waves folder for the use of the function
			TextAsset waveDat = Resources.Load("waves/desert_Wave", typeof(TextAsset)) as TextAsset;
			//each row is split with a '\n' a.k.a an enter to a new row
			string[] data = waveDat.text.Split (new char[] { '\n' });
			//declares wave integer as zero for the foreach loop
			//the foreach loop proceeds to move through the text file found earlier
			int wave = 0;
			foreach (string a in data) {
				//splits invidiual data based on the presence of a comma
				string[] waveInfo = a.Split(',');
				//for footknights/horseknights/trolls of each wave (the current number), a new integer is declared with the wave type and amount of waves
				footknightWaves [wave] = new int[] { int.Parse(waveInfo[2]), int.Parse(waveInfo[1]) };
				horseknightWaves [wave] = new int[] { int.Parse (waveInfo [4]), int.Parse (waveInfo [3]) };
				trollWaves [wave] = new int[] { int.Parse (waveInfo [6]), int.Parse (waveInfo [5]) };
				//increments wave, moving to the next row of the grid
				wave++;
			}
		}
		//checks if current scene is boss level
		if (SceneManager.GetActiveScene().name == "bossLevel") {
			//get Wave csv for Boss Level
			//Finds text file of the following name in waves folder for the use of the function
			TextAsset waveDat = Resources.Load("waves/boss_Wave", typeof(TextAsset)) as TextAsset;
			//each row is split with a '\n' a.k.a an enter to a new row
			string[] data = waveDat.text.Split (new char[] { '\n' });
			//declares wave integer as zero for the foreach loop
			//the foreach loop proceeds to move through the text file found earlier
			int wave = 0;
			foreach (string a in data) {
				//splits invidiual data based on the presence of a comma
				string[] waveInfo = a.Split(',');
				//for footknights/horseknights/trolls of each wave (the current number), a new integer is declared with the wave type and amount of waves
				footknightWaves [wave] = new int[] { int.Parse(waveInfo[2]), int.Parse(waveInfo[1]) };
				horseknightWaves [wave] = new int[] { int.Parse (waveInfo [4]), int.Parse (waveInfo [3]) };
				trollWaves [wave] = new int[] { int.Parse (waveInfo [6]), int.Parse (waveInfo [5]) };
				//increments wave, moving to the next row of the grid
				wave++;
			}
		}

		/*
		footknightWaves[0] = new int[] { waves, 1 };
        horseknightWaves[0] = new int[] { allAtOnce, 0 };
		//trollWaves[0] = new int[] { allAtOnce, 1 }; trolls on level 1 for test
        trollWaves[0] = new int[] { allAtOnce, 0 };

        footknightWaves[1] = new int[] { waves, 2 };
        horseknightWaves[1] = new int[] { singles, 2 };
        trollWaves[1] = new int[] { allAtOnce, 0 };

		footknightWaves[2] = new int[] { allAtOnce, 1 };
        horseknightWaves[2] = new int[] { singles, 5 };
        trollWaves[2] = new int[] { allAtOnce, 0 };

        footknightWaves[3] = new int[] { waves, 2 };
        horseknightWaves[3] = new int[] { waves, 1 };
        trollWaves[3] = new int[] { allAtOnce, 0 };

        footknightWaves[4] = new int[] { waves, 2};
        horseknightWaves[4] = new int[] { singles, 7 };
		trollWaves[4] = new int[] { singles, 0 };

        footknightWaves[5] = new int[] { waves, 2};
		horseknightWaves[5] = new int[] { singles, 7 };
        trollWaves[5] = new int[] { singles, 0 };

        footknightWaves[6] = new int[] { waves, 4};
        horseknightWaves[6] = new int[] { singles, 5 };
		trollWaves[6] = new int[] { singles, 0 };

        footknightWaves[7] = new int[] { waves, 2};
        horseknightWaves[7] = new int[] { singles, 6 };
        trollWaves[7] = new int[] { singles, 1 };

        footknightWaves[8] = new int[] { waves, 3};
        horseknightWaves[8] = new int[] { allAtOnce, 2 };
        trollWaves[8] = new int[] { singles, 2 };

        footknightWaves[9] = new int[] { waves, 3};
        horseknightWaves[9] = new int[] { waves, 2};
        trollWaves[9] = new int[] { singles, 2 };
        */
    }

    public void NextWave()
	{
        int adjustedCurrentWave = currentWave + 1; //adjusted for 0 being wave 1
		if(adjustedCurrentWave % 5 == 0 && LocalUserData.IsLoggedIn())
        {
            powerup.UpgradeLevel(1);
            //print("leveledUp");
            floatingText.text = "LEVEL UP!";
            StartCoroutine(FadingText(floatingText));
        }

		if (currentWave % 2 == 0) {
			Mathm.SetDifficulty ();
		}

        if (currentWave == 4)
        {
            A_Source.clip = halfwayThrough;
            A_Source.Play();
        }

        if (currentWave == 9)
        {
            A_Source.clip = trollsUnleashed;
            A_Source.Play();
        }

        currentWave += 1;
		if(currentWave == finalWave){
            mStats.showWinUI();
            music.clip = winSound;
            music.loop = false;
            music.Play();
            GameStateManager.instance.UnlockNextLevel();
			levelComplete = true;
        }
        else{
            ActivateWave(currentWave);
        }
        GameStateManager.instance.currentState = EnumManager.GameState.Wave;
		
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
		WaveTitle.text = "Wave " + (currentWave + 1).ToString () + "/" + finalWave;
		mathWaveTitle.text = "Wave: " + (currentWave + 1).ToString () + "/" + finalWave;
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
		yield return new WaitForSeconds (3f);

		//if this wave is all at once
		print(waveType[currentWave][0]);
		if(waveType[currentWave][0] == 0)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				SpawnEnemy(enemyPrefab, spawnSound,0);
                //while(GameStateManager.instance.is)
				yield return new WaitForSeconds (.1f);
				SpawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (.1f);
				SpawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (Random.Range (.1f, .2f));
			}
		}


		//if this wave is staggered
		if(waveType[currentWave][0] == 1)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				SpawnEnemy(enemyPrefab, spawnSound,0);
				yield return new WaitForSeconds (Random.Range (1f, 1.6f));
				SpawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (Random.Range (1f, 1.6f));
				SpawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (.2f);
			}
		}

		//if this wave is waves
		if(waveType[currentWave][0] == 2)
		{
			for (int i = 0; i <  waveType[currentWave][1]; i++) {
				SpawnEnemy(enemyPrefab, spawnSound,0);
				yield return new WaitForSeconds (Random.Range (1.0f, 1.5f));
				SpawnEnemy(enemyPrefab, spawnSound,1);
				yield return new WaitForSeconds (Random.Range (1.3f, 1.8f));
				SpawnEnemy(enemyPrefab, spawnSound,2);
				yield return new WaitForSeconds (2.5f);
			}
		}

        //if this wave is Singles
        if (waveType[currentWave][0] == 3)
        {
            for (int i = 0; i < waveType[currentWave][1]; i++)
            {
                SpawnEnemy(enemyPrefab, spawnSound, Random.Range(0,3));
                yield return new WaitForSeconds(Random.Range(1.6f, 3.0f));
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
	public void SpawnEnemy(GameObject enemy, AudioClip spawnSound,int spawn, bool spawnAsClone = false){
        //int randomSpawn = Random.Range(0, SpawnPoints.Length);
        
		GameObject enemyObject = Instantiate(enemy, SpawnPoints[spawn].position + SpawnPoints[spawn].right* Random.Range(spawnAdjustmentMin, spawnAdjustmentMax), SpawnPoints[0].rotation);
		enemyObject.GetComponent<EnemyBehavior>().SetTarget(fenceTargets[spawn]); //points enemy at right target. adjusted for UI name
        enemyObject.GetComponent<EnemyBehavior>().isClone = spawnAsClone;
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

            //Test
            if (currentWave == 0) AwardGems(10, EnumManager.GemType.Penny);
            
            if (currentWave == 4) AwardGems(1, EnumManager.GemType.Quarter);
            else if (currentWave == 9) AwardGems(3, EnumManager.GemType.Quarter);
            else if (currentWave == 14) AwardGems(2, EnumManager.GemType.Dollar);



            
            if ((currentWave + 2) % 3 == 0 && currentWave + 2 > 1 && currentWave != 19 && currentWave != 13 && (mManager.QuestionTypes[1] || mManager.QuestionTypes[0]))
            {
                GameStateManager.instance.currentState = EnumManager.GameState.Intermath;
                Mathm.ActivateInterMath();
            }
            else
            {
                NextWave();
            }
            if (currentWave != finalWave)
            {
                A_Source.clip = WaveCleared;
                A_Source.Play();
            }
		}
	}

    public void AwardGems(int count, EnumManager.GemType type)
    {
        GameStateManager.instance.levelManager.RecieveGems(count, type);
        GameObject GemSpawner = Instantiate(Resources.Load("Misc/GemSpawner"), gemSpawnPoint.position, Quaternion.identity) as GameObject;
        GemSpawner.GetComponent<GemSpawner>().SetGemAndStartSpawn(type, gemSpawnPoint, count);
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