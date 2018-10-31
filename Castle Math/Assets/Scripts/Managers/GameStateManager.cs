using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;



// This script should contain anything that activates or deactivates things
// between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {

    public EnumManager.GameState currentState;
    // Analytics
    public MathController m_Controller;
    public TelemetryManager telemetryManager;

    // UI
    private string playerName = "JGC";

    // Game statistics
    public QuestionTracker tracker = new QuestionTracker();
    public PlayerMathStats playerMathStats;

    // Environment
    [HideInInspector]
    public LaunchProjectile player;
    private bool loseState = false;
    public int currentSkillLevel;
    [HideInInspector]
    public int levelsUnlocked = 1;

    // References
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public PotionShop potionShop;
    [HideInInspector]
    public Inventory inventory;
    [HideInInspector]
    public WaveManager waveManager;
    [HideInInspector]
    public MathManager mathManager;
    [HideInInspector]
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
        Debug.Log("Loading JSON");
    }

    void loadPlayerPrefs() {
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
        PlayerPrefs.SetInt("tutorialDone", 0); // temp to force tutorial

        SceneManager.sceneLoaded += OnSceneLoaded;

        loadPlayerPrefs();
        telemetryManager = GetComponent<TelemetryManager>();
        m_Controller = GetComponent<MathController>();

        numStars = SaveData.numStars;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex > 0) {
            //RenderSettings.skybox.SetFloat("_Exposure", 0.8f);

            // If this block is commented, uncomment it
            m_Controller = GameObject.FindObjectOfType<MathController>();

            // TODO: Why change exposure?
            RenderSettings.skybox.SetFloat("_Exposure", 1.0f); //reset exposure
            player = GameObject.FindObjectOfType<LaunchProjectile>();
            //mainMenuEffects.fadeIn(.4f);

            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }
    }

    // TODO: Unity function?
    public void SetTimeScale(float newTimeScale, float duration) {
        StartCoroutine(ChangeTimeScale(newTimeScale, duration));
    }

    // TODO: Unity function?
    public bool IsLost() {
        return loseState;
    }

    // TODO: Unity function?
    public void UnlockNextLevel() {
        levelsUnlocked++;
    }

    // TODO: Unity function?
    public void LoseState() {
        if (!loseState) {
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
        telemetryManager.LogRound("ended", true);
    }

    public void LoadScene(int sceneNum) {
        SceneManager.LoadScene(sceneNum);
    }

    public void Retry() {
        loseState = false;
        // TODO: Why change exposure?
        RenderSettings.skybox.SetFloat("_Exposure", 0.8f);
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        telemetryManager.LogSession();

        SaveData.SaveDataToJSon();
        StartCoroutine(ActivatorVR("None"));
    }

    void OnApplicationQuit() {
        telemetryManager.LogSession();
    }

    public IEnumerator ActivatorVR(string vrToggle) {
        // TODO: Please explain yield
        UnityEngine.VR.VRSettings.LoadDeviceByName(vrToggle);
        yield return null;
        UnityEngine.VR.VRSettings.enabled = false;
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene (0);
    }

    /*
    public void LoadNextLevel() {
        SceneManager.LoadScene("BossLevel", LoadSceneMode.Single);
    }
    */

    public void SaveGame() {
        playerMathStats.SaveState();
        SaveData.SaveDataToJSon();
        PlayerPrefs.SetString("PlayerName", playerName);
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
}
