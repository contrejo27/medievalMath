using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;



//This script should contain anything that activates or deactivates things between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {

    public EnumManager.GameState currentState;

	//UI
	private string playerName = "JGC";
    
    // Game statistics
    public QuestionTracker tracker = new QuestionTracker();

	//audio
	public PlayerMathStats playerMathStats;
	
	//Environment
    [HideInInspector]
	public LaunchProjectile player;
	private bool loseState = false;
    public int currentSkillLevel;

    //References
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


    //analytics
    GameMetrics gMetrics;
    MathController m_Controller;

    //Singleton
    public static GameStateManager instance;

    void Awake(){

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        tracker.ReadCSV();
        SaveData.LoadDataFromJSon();
        Debug.Log("Loading JSON");
    }

    // Use this for initialization
    void Start () {
        PlayerPrefs.SetInt("tutorialDone", 0); //temp to force tutorial

        gMetrics = GetComponent<GameMetrics>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        //first time game is opened sets up initial playerPref values
        if (!PlayerPrefs.HasKey("isFirstTime"))
        {
            PlayerPrefs.SetInt("tutorialDone", 0);

            // Set and save all your PlayerPrefs here.
            // Now set the value of isFirstTime to be false in the PlayerPrefs.
            PlayerPrefs.SetInt("isFirstTime", 1);
            PlayerPrefs.SetString("globalHS1", "JGC,3,8");
            PlayerPrefs.SetString("globalHS2", "HBK,2,5");
            PlayerPrefs.SetString("globalHS3", "JGC,2,3");
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("Skill Level", 1);
            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }
        else
        {
            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }

        numStars = SaveData.numStars;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > 0)
        {
            //RenderSettings.skybox.SetFloat("_Exposure", 0.8f);

            // If this block is commented, uncomment it
            m_Controller = GameObject.FindObjectOfType<MathController>();
            
            RenderSettings.skybox.SetFloat("_Exposure", 1.0f); //reset exposure
            player = GameObject.FindObjectOfType<LaunchProjectile>();
            //mainMenuEffects.fadeIn(.4f);

            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }

    }

    public void SetTimeScale(float newTimeScale, float duration)
    {
        
    }

    public bool IsLost()
    {
        return loseState;
    }

    public void StartGame(){
		

	}

	public void LoseState(){
                
		if(!loseState){
			loseState = true;
		}
		SaveGame();

		player.isAlive = false;
        levelManager.DoLoseGameEffects();
		
	}

	public void Retry()
	{
		loseState = false;
		RenderSettings.skybox.SetFloat("_Exposure", 0.8f);
		SceneManager.LoadScene (1);

	}

	public void Quit()
	{
        float timeInVR;

        if (m_Controller != null)
        {
            timeInVR = Time.time - m_Controller.startTime;
        }
        else{
            timeInVR = 0f;
        }

        if (GameMetrics.m_instance)
        {
            gMetrics.UpdateMetric("TimeInVR", timeInVR);
        }
        SaveData.SaveDataToJSon();
        StartCoroutine(ActivatorVR("None"));
	}

    void OnApplicationQuit()
    {
        float timeInVR;

        if (m_Controller != null)
        {
            timeInVR = Time.time - m_Controller.startTime;
        }
        else
        {
            timeInVR = 0f;
        }
        if (gMetrics != null)
        {
            gMetrics.UpdateMetric("TimeInVR", timeInVR);
        }
    }

    public IEnumerator ActivatorVR(string vrToggle){
		UnityEngine.VR.VRSettings.LoadDeviceByName(vrToggle);
		yield return null;
		UnityEngine.VR.VRSettings.enabled = false;
		yield return new WaitForSeconds(.1f);
		SceneManager.LoadScene (0);
	}

    public void LoadNextLevel(){
        SceneManager.LoadScene("BossLevel", LoadSceneMode.Single);
    }
    //goes through different scripts and saves info
    public void SaveGame(){
		playerMathStats.SaveState();
        SaveData.SaveDataToJSon();
		PlayerPrefs.SetString("PlayerName", playerName);
	}

    public void ActivatePotionShop()
    {
        potionShop.gameObject.SetActive(true);
    }

    public int GetStars()
    {
        return numStars; 
    }

    public void AddStars(int n)
    {
        SaveData.numStars += n;
        SaveData.SaveDataToJSon();
        numStars += n;
    }

    public void SpendStars(int n)
    {
        SaveData.numStars -= n;
        SaveData.SaveDataToJSon();
        numStars -= n;
    }

    IEnumerator ChangeTimeScale(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
