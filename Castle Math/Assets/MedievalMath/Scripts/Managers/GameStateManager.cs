using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.UI;


// This script should contain anything that activates or deactivates things
// between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {

    public EnumManager.GameState currentState;
    public EnumManager.GameplayMode currentDifficulty;


    // Game statistics
    public QuestionTracker tracker = new QuestionTracker();
    public PlayerMathStats playerMathStats;

    // Environment
    [HideInInspector]
    public LaunchProjectile player;
    private static bool loseState = false;
    public int currentSkillLevel;
    public bool isVR = true;
    [HideInInspector]
    public int levelsUnlocked = 1;

    // References
    [HideInInspector]
    public PlayerController playerController;
    public PotionShop potionShop;
    [HideInInspector]
    public Inventory inventory;
    //[HideInInspector]
    public WaveManager waveManager;
    //[HideInInspector]
    public MathManager mathManager;
    //[HideInInspector]
    public LevelManager levelManager;


    // User stats
    private int numStars;

    // Singleton
    public static GameStateManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        // TODO: What does tracker do?
        tracker.ReadCSV();
        SaveData.LoadDataFromJSon();
    }

    void loadPlayerPrefs() {
      Debug.Log("PlayerName:" + PlayerPrefs.GetString("playerName"));
      if (!PlayerPrefs.HasKey("isFirstTime")) {
          PlayerPrefs.SetInt("tutorialDone", 0);

          // Set and save all your PlayerPrefs here.
          // Now set the value of isFirstTime to be false in the PlayerPrefs.
          PlayerPrefs.SetInt("isFirstTime", 1);
          PlayerPrefs.SetString("globalHS1", "JGC,3,8");
          PlayerPrefs.SetString("globalHS2", "HBK,2,5");
          PlayerPrefs.SetString("globalHS3", "JGC,2,3");
          // TODO: Why save before skill level?
          PlayerPrefs.Save();
          PlayerPrefs.SetInt("Skill Level", 1);
          currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
      }
      else {
          currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
      }
    }

    // Use this for initialization
    void Start () {
        Init();

    }

    void Init()
    {
        PlayerPrefs.SetInt("tutorialDone", 0); // temp to force tutorial

        SceneManager.sceneLoaded += OnSceneLoaded;

        waveManager = GameObject.FindObjectOfType<WaveManager>();
        mathManager = GameObject.FindObjectOfType<MathManager>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        playerMathStats = GameObject.FindObjectOfType<PlayerMathStats>();

        loadPlayerPrefs();
        //m_telemetry = GameObject.FindObjectOfType<TelemetryManager>();

        numStars = SaveData.numStars;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex > 0) {
            //RenderSettings.skybox.SetFloat("_Exposure", 0.8f);

            // TODO: Why change exposure?
            RenderSettings.skybox.SetFloat("_Exposure", 1.0f); //reset exposure
            player = GameObject.FindObjectOfType<LaunchProjectile>();
            //mainMenuEffects.fadeIn(.4f);

            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }
    }

    public void SetTimeScale(float newTimeScale, float duration) {
        StartCoroutine(ChangeTimeScale(newTimeScale, duration));
    }

    public bool IsLost() {
        return loseState;
    }

    public void UnlockNextLevel() {
        if (currentDifficulty == EnumManager.GameplayMode.Medium) {
            levelsUnlocked++;
        }
    }

    public void LoseState() {
        if(!loseState) {
                loseState = true;
        }
        /*
        SaveGame();

        int currentLevel = 1;   // EnumManager.sceneNameToLevelNumber[SceneManager.GetActiveScene().name];

        if (waveManager.currentWave < 9) {

        }
        else if(waveManager.currentWave < 14) {
            if( SaveData.starsPerLevel[currentLevel] < 1) {
                SaveData.starsPerLevel[currentLevel] = 1;
                SaveData.numStars++;
                SaveData.SaveDataToJSon();
            }
        }
        else if(waveManager.currentWave < 20) {
            if (SaveData.starsPerLevel[currentLevel] < 1) {
                SaveData.starsPerLevel[currentLevel] = 2;
                SaveData.numStars+=2;
                SaveData.SaveDataToJSon();
            }
            else if(SaveData.starsPerLevel[currentLevel] < 2) {
                SaveData.starsPerLevel[currentLevel] = 2;
                SaveData.numStars++;
                SaveData.SaveDataToJSon();
            }
        }
        */
        player.isAlive = false;
        levelManager.DoLoseGameEffects();
        TelemetryManager.instance.LogRound();
    }

    public void LoadScene(int sceneNum) {
        SceneManager.LoadScene(sceneNum);
    }

    public void LoadSceneByName(string sceneName)
    {
        if(sceneName == "kellsLevel")
        {
            StartCoroutine(ActivatorVR("Cardboard", isVR,sceneName));
        } else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        
        
    }

    public void Retry() {
        loseState = false;
        //Changing lighting back to normal
        RenderSettings.skybox.SetFloat("_Exposure", 0.8f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() {
        //m_telemetry.LogSession();

        //SaveData.SaveDataToJSon();
        StartCoroutine(ActivatorVR("None", false, "None"));
    }

    void OnApplicationQuit() {
       // m_telemetry.LogSession();
    }

    public IEnumerator ActivatorVR(string vrToggle, bool isVROn, string levelName)
    {
        if(isVROn)
        {
            yield return new WaitForSeconds(.5f);
            UnityEngine.XR.XRSettings.LoadDeviceByName(vrToggle);
            yield return null;
            UnityEngine.XR.XRSettings.enabled = true;
            yield return new WaitForSeconds(.5f);
            if (levelName != "None")
                SceneManager.LoadScene(levelName);
        } else
        {
            yield return new WaitForSeconds(.5f);
            if (levelName != "None")
                SceneManager.LoadScene(levelName);
        }
        
    }

    /*
    public void LoadNextLevel() {
        SceneManager.LoadScene("BossLevel", LoadSceneMode.Single);
    }
    */

    public void SaveGame() {
        playerMathStats.SaveState();
        SaveData.SaveDataToJSon();
    }

    public void ActivatePotionShop() {
        potionShop.gameObject.SetActive(true);
    }

    public int GetStars() {
        return numStars;
    }

    public void AddStars(int n) {
        SaveData.numStars += n;
        SaveData.SaveDataToJSon();
        numStars += n;
    }

    public void SpendStars(int n) {
        SaveData.numStars -= n;
        SaveData.SaveDataToJSon();
        // TODO: Why numStars after?
        numStars -= n;
    }

    // TODO: Why change time scale?
    IEnumerator ChangeTimeScale(float timeScale, float duration) {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    public void SetDifficulty(int mode)
    {
        switch(mode)
        {
            case 0:
                currentDifficulty = EnumManager.GameplayMode.Easy;
                break;
            case 1:
                currentDifficulty = EnumManager.GameplayMode.Medium;
                break;
            case 2:
                currentDifficulty = EnumManager.GameplayMode.Hard;
                break;
            default:
                currentDifficulty = EnumManager.GameplayMode.Easy;
                break;
        }

        GetComponent<GameData>().gameRound.mode = currentDifficulty.ToString();
    }

    public void SetActiveWindow(GameObject otherMenu)
    {
        otherMenu.SetActive(true);
    }

    public void SetInactiveWindow(GameObject otherMenu)
    {
        otherMenu.SetActive(false);
    }

    public void SetStage(string stage)
    {
        GetComponent<GameData>().gameRound.level_name = stage;
    }

    public void ActivateNextButton(Button other)
    {
        other.interactable = true;
    }

    public void TestClick()
    {
        Debug.Log("Pressed the button");
    }

    public void SetVRMode(bool isVROn)
    {
        isVR = isVROn;
    }

}

