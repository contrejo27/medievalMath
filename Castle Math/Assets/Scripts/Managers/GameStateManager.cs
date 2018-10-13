using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;



// This script should contain anything that activates or deactivates things
// between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {

    public EnumManager.GameState currentState;

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

    // Analytics
    GameMetrics gMetrics;
    MathController m_Controller;

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

    // Use this for initialization
    void Start () {
        PlayerPrefs.SetInt("tutorialDone", 0); // temp to force tutorial

        // TODO: Where is this component loaded?
        gMetrics = GetComponent<GameMetrics>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // TODO: Can this be extracted to a separate function called firstTimePrefs()? For readability.
        // TODO: Is there a time when PlayerPrefs do not have a key "isFirstTime"?
        // First time game is opened sets up initial playerPref values
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

    public void recordTimeinVR() {
        /* Sends play time information to database */
        float timeInVR;
        if (gMetrics != null) {
            if (m_Controller != null) {
                timeInVR = Time.time - m_Controller.startTime;
            }
            else {
                timeInVR = 0f;
            Debug.Log("m_Controller is null");
            }
            gMetrics.UpdateMetric("TimeInVR", timeInVR);
        }
        else {
            Debug.Log("gMetrics is null");
        }
    }

    public void recordMetrics() {
        // TODO: Add PlayerPrefs to database
        Debug.Log(PlayerPrefs.GetString("PlayerName"));
        Debug.Log(PlayerPrefs.GetInt("Skill Level"));
        Debug.Log(PlayerPrefs.GetString("globalHS1"));
        Debug.Log(PlayerPrefs.GetString("globalHS2"));
        Debug.Log(PlayerPrefs.GetString("globalHS3"));
        gMetrics.UpdateMetric("TimeInVR", Random.Range(1,100));
        recordTimeinVR();
    }

    public void Quit() {
        // TODO: Verify if the correct location for recordMetrics() is on Quit() or OnApplicationQuit()
        recordMetrics();

        SaveData.SaveDataToJSon();
        StartCoroutine(ActivatorVR("None"));
    }

    void OnApplicationQuit() {
        // TODO: Verify if the correct location for recordMetrics() is on Quit() or OnApplicationQuit()
        recordMetrics();
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
