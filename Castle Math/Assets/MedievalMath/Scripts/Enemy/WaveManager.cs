using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class WaveManager : MonoBehaviour
{
    //enemies
    public GameObject smallEnemyPrefab;
    public GameObject bigEnemyPrefab;
    public GameObject midEnemyPrefab;
    public GameObject bonusEnemy;
    public int currentWave;
    int finalWave;
    public GameObject storyText;

    //temp
    public GameObject billboardCanvas;
    public GameObject billboardTutorialImage;

    //Environment 
    public Transform[] SpawnPoints;
    public Transform[] BonusSpawnPoints;
    public Transform[] fenceTargets;
    public GameObject billboard;
    public Transform gemSpawnPoint;


    //Audio
    public AudioClip AnotherWave;
    private AudioSource A_Source;
    public AudioSource enemySounds;
    private AudioSource music;

    public AudioClip bigEnemysUnleashed;
    public AudioClip winSound;
    public AudioClip midEnemySpawnSound;
    public AudioClip halfwayThrough;

    public AudioClip bigEnemySpawnSound;
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
    public GameObject statCanvas;
    public TelemetryManager m_telemetry;

    //debug
    public bool testMidEnemy = false;

    //2D integer Array to determine parameters of waves for the 3 enemy types
    int[][] smallEnemyWaves = new int[20][];
    int[][] midEnemyWaves = new int[20][];
    int[][] bigEnemyWaves = new int[20][];
    //floating text
    public Text floatingText;

    private List<int> pendingSpawnPoints = new List<int>();

    private void CreateSpawnList(int avoidedStartIndex)
    {
        List<int> tempList = new List<int>();
        for (int i = 0; i < BonusSpawnPoints.Length; i++)
        {
            tempList.Add(i);
        }
        pendingSpawnPoints.Clear();
        for (int i = 0; i < BonusSpawnPoints.Length; i++)
        {
            int idx = UnityEngine.Random.Range(0, tempList.Count);
            if (tempList[idx] == avoidedStartIndex)
                idx = (idx + 1) % tempList.Count;
            pendingSpawnPoints.Add(tempList[idx]);
            tempList.RemoveAt(idx);
        }
    }

    // Use this for initialization
    void Start()
    {
        CreateSpawnList(-1);

        levelComplete = false;

        if (!Application.isEditor)
            currentWave = 0;

        A_Source = GameObject.Find("CastleAudio").GetComponent<AudioSource>();

        music = GameObject.Find("Music").GetComponent<AudioSource>();
        GameStateManager.instance.waveManager = this;
        m_telemetry = GameObject.FindObjectOfType<TelemetryManager>();
        readLevel(SceneManager.GetActiveScene().name);
    }

    public void StartNewDifficulty(string newDifficulty)
    {
        switch (newDifficulty)
        {
            case "Skirmish":
                GameStateManager.instance.currentDifficulty = EnumManager.GameplayMode.Easy;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "Battle":
                GameStateManager.instance.currentDifficulty = EnumManager.GameplayMode.Medium;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
        //set at -1 because in NextWave it adds +1. 
        GameStateManager.instance.Retry();
    }

    public void readLevel(string level)
    {
        //get Wave csv
        if (GameStateManager.instance.currentDifficulty.ToString() == "Easy") finalWave = 10;
        if (GameStateManager.instance.currentDifficulty.ToString() == "Hard") finalWave = 15;

        //Finds text file of the following name in waves folder for the use of the function
        string waveFileName = "waves/" + level.Replace("Level", "") + "_Wave" + GameStateManager.instance.currentDifficulty.ToString();
        TextAsset waveDat = Resources.Load(waveFileName, typeof(TextAsset)) as TextAsset;
        print("Loading : " + waveFileName);
        debugText.instance.AddDebugText(waveDat.ToString() + "\n");

        //each row is split with a '\n' a.k.a an enter to a new row
        string[] data = waveDat.text.Split(new char[] { '\n' });
        debugText.instance.AddDebugText(data[0] + data[1]);

        //skip first line reading
        bool first = true;

        //declares wave integer as zero for the foreach loop
        int wave = 0;

        //the foreach loop proceeds to move through the text file found earlier'
        foreach (string a in data)
        {
            //splits invidiual data based on the presence of a comma
            string[] waveInfo = a.Split(',');
            if (first) first = false;
            else if (waveInfo.Length > 1)
            {
                if (!waveInfo[0].StartsWith("*"))
                {
                    //for smallEnemys/midEnemys/bigEnemys of each wave (the current number), a new integer is declared with the wave type and amount of waves
                    smallEnemyWaves[wave] = new int[] { int.Parse(waveInfo[2]), int.Parse(waveInfo[1]) };
                    midEnemyWaves[wave] = new int[] { int.Parse(waveInfo[4]), int.Parse(waveInfo[3]) };
                    bigEnemyWaves[wave] = new int[] { int.Parse(waveInfo[6]), int.Parse(waveInfo[5]) };
                    //increments wave, moving to the next row of the grid
                    wave++;
                }
            }
        }
    }
    public void NextWave()
    {
        int adjustedCurrentWave = currentWave + 1; //adjusted for 0 being wave 1
        if (adjustedCurrentWave % 5 == 0 && LocalUserData.IsLoggedIn())
        {
            powerup.UpgradeLevel(1);
            //print("leveledUp");
            floatingText.text = "LEVEL UP!";
            StartCoroutine(FadingText(floatingText));
        }

        if (currentWave % 2 == 0)
        {
            MathManager.instance.SetDifficulty();
        }

        if (currentWave == finalWave / 2)
        {
            A_Source.clip = halfwayThrough;
            A_Source.Play();
        }

        if (currentWave == 9)
        {
            A_Source.clip = bigEnemysUnleashed;
            A_Source.Play();
        }

        currentWave += 1;
        if (currentWave >= finalWave)
        {
            mStats.showWinUI();
            music.clip = winSound;
            music.loop = false;
            music.Play();
            levelComplete = true;
        }
        else
        {
            ActivateWave(currentWave);
        }
        GameStateManager.instance.currentState = EnumManager.GameState.Wave;
        //m_telemetry.LogRound();
    }


    IEnumerator FadingText(Text currentText)
    {
        yield return new WaitForSeconds(1.5f);
        currentText.text = "";

    }
    public void ActivateWave(int WaveIndex)
    {
        if (testMidEnemy)
        {
            StartCoroutine(ActivateTestEnemies(midEnemyWaves, midEnemyPrefab, midEnemySpawnSound));

        }
        else
        {
            setWaveText();
            //Create all of the enemies
            if (currentWave != 0)
            { // add && currentWave < 2
                StartCoroutine(announceWave());
            }
            StartCoroutine(ActivateEnemies(smallEnemyWaves, smallEnemyPrefab, null));
            StartCoroutine(ActivateEnemies(bigEnemyWaves, bigEnemyPrefab, bigEnemySpawnSound));
            StartCoroutine(ActivateEnemies(midEnemyWaves, midEnemyPrefab, midEnemySpawnSound));
        }
    }

    void setWaveText()
    {
        WaveTitle.text = "Wave " + (currentWave + 1).ToString() + "/" + finalWave;
        mathWaveTitle.text = "Wave: " + (currentWave + 1).ToString() + "/" + finalWave;
        StartCoroutine(FadeOut(waveEffect));
    }

    IEnumerator announceWave()
    {
        yield return new WaitForSeconds(4f);
        A_Source.clip = AnotherWave;
        A_Source.Play();
    }

    //will spawn test enemy every x seconds
    IEnumerator ActivateTestEnemies(int[][] waveType, GameObject enemyPrefab, AudioClip spawnSound)
    {

        while (true)
        {
            yield return new WaitForSeconds(6f);

            SpawnEnemy(enemyPrefab, spawnSound, 0);
            yield return new WaitForSeconds(.1f);
            SpawnEnemy(enemyPrefab, spawnSound, 1);
            yield return new WaitForSeconds(.1f);
            SpawnEnemy(enemyPrefab, spawnSound, 2);
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .2f));
        }
    }


    IEnumerator ActivateEnemies(int[][] waveType, GameObject enemyPrefab, AudioClip spawnSound)
    {
        //delay so the player can breather/ do math
        yield return new WaitForSeconds(2f);

        //if this wave is all at once
        if (waveType[currentWave][0] == 0)
        {
            for (int i = 0; i < waveType[currentWave][1]; i++)
            {
                SpawnEnemy(enemyPrefab, spawnSound, 0);
                yield return new WaitForSeconds(.1f);
                SpawnEnemy(enemyPrefab, spawnSound, 1);
                yield return new WaitForSeconds(.1f);
                SpawnEnemy(enemyPrefab, spawnSound, 2);
                yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, .2f));
            }
        }


        //if this wave is staggered
        if (waveType[currentWave][0] == 1)
        {
            for (int i = 0; i < waveType[currentWave][1]; i++)
            {
                SpawnEnemy(enemyPrefab, spawnSound, 0);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1.6f));
                SpawnEnemy(enemyPrefab, spawnSound, 1);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 1.6f));
                SpawnEnemy(enemyPrefab, spawnSound, 2);
                yield return new WaitForSeconds(.2f);
            }
        }

        //if this wave is waves
        if (waveType[currentWave][0] == 2)
        {
            for (int i = 0; i < waveType[currentWave][1]; i++)
            {
                SpawnEnemy(enemyPrefab, spawnSound, 0);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 1.5f));
                SpawnEnemy(enemyPrefab, spawnSound, 1);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.3f, 1.8f));
                SpawnEnemy(enemyPrefab, spawnSound, 2);
                yield return new WaitForSeconds(2.5f);
            }
        }

        //if this wave is Singles
        if (waveType[currentWave][0] == 3)
        {
            for (int i = 0; i < waveType[currentWave][1]; i++)
            {
                SpawnEnemy(enemyPrefab, spawnSound, UnityEngine.Random.Range(0, 3));
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.6f, 3.0f));
            }
        }

        if (bonusEnemy)
        {
            if (currentWave >= 3)
            {
                for (int i = 0; i < waveType[currentWave][1]; i++)
                {
                    SpawnBonusEnemy(bonusEnemy, spawnSound, UnityEngine.Random.Range(0, 3));
                    yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 6f));

                    if (i >= 3)
                    {
                        break;
                    }
                }
            }
        }

        /*
           SetNumberOfEnemies (WaveSize);
           for (int i = 0; i <  WaveSize; i++) {
           spawnEnemy(midEnemy, null);
           yield return new WaitForSeconds (Random.Range (0.2f, 0.8f));
           }
        //waves in between bigEnemys
        int bigEnemyFrequency = 5;
        if (currentWave % bigEnemyFrequency == 0 && currentWave != 0){
//bigEnemys increase by 1 every set WbigEnemyFrequency
for (int i = 0; i < currentWave / bigEnemyFrequency; i++) { 
spawnEnemy(bigEnemyPrefab, bigEnemySpawnSound);
addEnemyToWaveSize();
yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));
}
}
if (currentWave % 4 == 0 && currentWave != 0)
{
for (int i = 0; i < currentWave / 3; i++)
{
spawnEnemy(midEnemyPrefab, midEnemySpawnSound);
addEnemyToWaveSize();

yield return new WaitForSeconds(Random.Range(0.2f, 1.1f));
}
}

if(currentWave > 12){
spawnEnemy(midEnemyPrefab, midEnemySpawnSound);
addEnemyToWaveSize();
}
if(currentWave > 16){
spawnEnemy(bigEnemyPrefab, bigEnemySpawnSound);
addEnemyToWaveSize();
}
*/
        //leave the title up for another second
        yield return new WaitForSeconds(1);

        WaveTitle.text = "";
        //waveEffect.alpha = 1f;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }

        Array.Resize(ref arr, arr.Length - 1);
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
    public void SpawnEnemy(GameObject enemy, AudioClip spawnSound, int spawn, bool spawnAsClone = false)
    {
        //int randomSpawn = Random.Range(0, SpawnPoints.Length);

        GameObject enemyObject = Instantiate(enemy, SpawnPoints[spawn].position + SpawnPoints[spawn].right * UnityEngine.Random.Range(spawnAdjustmentMin, spawnAdjustmentMax), SpawnPoints[0].rotation);
        enemyObject.GetComponent<EnemyBehavior>().SetTarget(fenceTargets[spawn]); //points enemy at right target. adjusted for UI name
        enemyObject.GetComponent<EnemyBehavior>().isClone = spawnAsClone;
        addEnemyToWaveSize();
        //GameObject enemyObject = Instantiate(enemy, SpawnPoints[randomSpawn].position+ new Vector3(Random.Range(-15, 10), 0,0), SpawnPoints[randomSpawn].rotation);

        if (spawnSound != null)
        {
            enemySounds.clip = spawnSound;
            enemySounds.Play();
        }
    }

    public void SpawnBonusEnemy(GameObject enemy, AudioClip spawnSound, int spawn, bool spawnAsClone = false)
    {
        int spawnPointIndex;
        GameObject enemyObject;
        if (pendingSpawnPoints.Count != 0)
        {
            spawnPointIndex = pendingSpawnPoints[0];
        }
        else
        {
            spawnPointIndex = 0;
        }

        if (pendingSpawnPoints.Count == 0)
        {

            CreateSpawnList(spawnPointIndex);
        }
        else
        {
            pendingSpawnPoints.RemoveAt(0);
        }
        if (!enemy)
        {
            Debug.LogError("Enemy Doesn't exist. Will spawn from default position");
            enemyObject = Instantiate(enemy, BonusSpawnPoints[0].position, BonusSpawnPoints[0].rotation);
        }
        else
        {
            enemyObject = Instantiate(enemy, BonusSpawnPoints[spawnPointIndex].position, BonusSpawnPoints[spawnPointIndex].rotation);
            enemyObject.GetComponent<SarcophagusScript>().SetTarget(fenceTargets[spawnPointIndex]);
        }

        addEnemyToWaveSize();

        //RemoveAt (ref BonusSpawnPoints, spawn);

        if (spawnSound != null)
        {
            enemySounds.clip = spawnSound;
            enemySounds.Play();
        }
    }

    public void KillAllEnemies()
    {
        CurrentEnemies = 1;
        EnemyKilled();
    }

    public void EnemyKilled()
    {
        CurrentEnemies -= 1;

        //if all enemies were killed
        if (CurrentEnemies <= 0)
        {

            //Test
            /* if (currentWave == 0) AwardGems(10, EnumManager.GemType.Penny);

               if (currentWave == 4) AwardGems(1, EnumManager.GemType.Quarter);
               else if (currentWave == 9) AwardGems(3, EnumManager.GemType.Quarter);
               else if (currentWave == 14) AwardGems(2, EnumManager.GemType.Dollar);

    */
            GameStateManager.instance.gameObject.GetComponent<SendToGoogle>().SendCustom(SystemInfo.deviceModel.ToString() + ",Time Since Launch" + Time.time.ToString() + ", Beat Wave" + currentWave + ", "+ SystemInfo.deviceName.ToString() + ", -,-");

            //load intermath
            if ((currentWave + 2) % 3 == 0 && currentWave + 2 > 1 && currentWave != 19 && currentWave != 13)
            {
                GameStateManager.instance.currentState = EnumManager.GameState.Intermath;
                MathManager.instance.ActivateInterMath();
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

    public int GetCurrentWaveNumber()
    {
        return currentWave;
    }

    private YieldInstruction fadeInstruction = new YieldInstruction();

    IEnumerator FadeOut(CanvasGroup image)
    {
        yield return new WaitForSeconds(1f);
        float elapsedTime = 0.0f;
        float totalTime = 2f;
        while (elapsedTime < totalTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            image.alpha = 1.0f - Mathf.Clamp01(elapsedTime / totalTime);
        }
    }


}
