﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;



//This script should contain anything that activates or deactivates things between state changes like lose or next wave
public class GameStateManager : MonoBehaviour {
	
	//UI
	public UIEffects mainMenuEffects;
	public UIEffects notificationEffects;
	public GameObject LoseScreen;
	public GameObject MathScreen;
	public GameObject tutorialImage;
	public GameObject target;

	public GameObject StatScreen;
	private string playerName = "JGC";

	//enemy behavior
	public GameObject InsidePoint;

    // Game statistics
    public QuestionTracker tracker = new QuestionTracker();

	//audio
	public AudioClip LostTheCastle;
	public AudioClip[] CastleScreams;
	public AudioSource music;
	public AudioClip gameplaySong;
	public PlayerMathStats playerMathStats;
	
	//Environment
    [HideInInspector]
	public LaunchProjectile player;
	public Light directionalLight;
	private bool loseState = false;
	public doorHealth fence1,fence2,fence3;
	public GameObject billboard;
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
    }

    // Use this for initialization
    void Start () {
        //RenderSettings.skybox.SetFloat("_Exposure", 0.8f);
        // If this block is commented, uncomment it
        
        m_Controller = GameObject.FindObjectOfType<MathController>();
        PlayerPrefs.SetInt("tutorialDone", 0); //temp to force tutorial

        gMetrics = GameObject.FindObjectOfType<GameMetrics>();

        RenderSettings.skybox.SetFloat("_Exposure", 1.0f); //reset exposure
		player = GameObject.FindObjectOfType<LaunchProjectile> (); 
		mainMenuEffects.fadeIn (.4f);
		
		//first time game is opened sets up initial playerPref values
		if(!PlayerPrefs.HasKey("isFirstTime")){
            PlayerPrefs.SetInt("tutorialDone", 0);

            // Set and save all your PlayerPrefs here.
            // Now set the value of isFirstTime to be false in the PlayerPrefs.
            PlayerPrefs.SetInt("isFirstTime", 1);
			PlayerPrefs.SetString("globalHS1","JGC,3,8");
			PlayerPrefs.SetString("globalHS2","HBK,2,5");
			PlayerPrefs.SetString("globalHS3","JGC,2,3");
			PlayerPrefs.Save();
            PlayerPrefs.SetInt("Skill Level", 1);
            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }
        else{
            currentSkillLevel = PlayerPrefs.GetInt("Skill Level");
        }
        
    }

    public void StartGame(){
		billboard.GetComponent<Animator> ().Play("hide");
		tutorialImage.SetActive(false);
		target.SetActive(false);
		mainMenuEffects.fadeOut(1.5f);
		notificationEffects.fadeIn(1.5f);
		music.clip = gameplaySong;
		music.loop = true;
		music.Play ();

	}

	public void LoseState(){


        music.Stop ();
		music.clip = LostTheCastle;
        music.loop = false;
		music.Play ();
		if(!loseState){
			loseState = true;
		}
		SaveGame();

		player.isAlive = false;
		
		//set UI
		LoseScreen.SetActive (true);
		MathScreen.SetActive (false);
		StatScreen.SetActive (true);
		doorHealth[] dh = GameObject.FindObjectsOfType<doorHealth> ();
		for (int i = 0; i < dh.Length; i++) {
			dh[i].loseFences();
		}
		
		//change enemy target so they start running
		EnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnemyBehavior> ();
		for (int i = 0; i < Enemies.Length; i++) {
			Enemies[i].UpdateTarget(InsidePoint.transform);
			//Enemies [i].Target = InsidePoint;
			//Enemies [i].gameObject.transform.parent = null;
		}
		fadeWorldOut();
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

        gMetrics.UpdateMetric("TimeInVR", timeInVR);
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
        SaveData.SaveDataToJSon();
        gMetrics.UpdateMetric("TimeInVR", timeInVR);
    }

    public IEnumerator ActivatorVR(string vrToggle){
		UnityEngine.VR.VRSettings.LoadDeviceByName(vrToggle);
		yield return null;
		UnityEngine.VR.VRSettings.enabled = false;
		yield return new WaitForSeconds(.1f);
		SceneManager.LoadScene (0);
	}

	void fadeWorldOut(){
		StartCoroutine(fadeSky(0.8f,0.0f));
		StartCoroutine(fadeLight(false));
	}
	
	void fadeWorldIn(){
		StartCoroutine(fadeSky(0.0f,0.8f));
		StartCoroutine(fadeLight(true));
	}
	
	IEnumerator fadeSky(float initialValue, float endValue)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
		{
			RenderSettings.skybox.SetFloat("_Exposure",  Mathf.Lerp(initialValue,endValue,t));
			yield return null;
		}
	}
	
	//fades light when you lose
	IEnumerator fadeLight(bool fadeIn)
	{
		Color sunLightColor =  directionalLight.color;
		Color ambientLightColor = RenderSettings.ambientSkyColor;
		Color currentColor =  RenderSettings.fogColor;

		if(fadeIn){
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				directionalLight.color = Color.Lerp(sunLightColor,new Color(.93f,.87f,.76f), t);
				RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor,new Color(.93f,.87f,.76f), t);
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.98f,.918f,.7f), t);
				yield return null;
			}
		}
		else{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
			{
				RenderSettings.fogColor = Color.Lerp(currentColor,new Color(.01f,.02f,.03f), t);
				RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor,new Color(.05f,.06f,.08f), t);
				directionalLight.color = Color.Lerp(sunLightColor,new Color(.05f,.06f,.08f), t);
				yield return null;
			}
		}
	}

    public void loadNextLevel(){
        SceneManager.LoadScene("BossLevel", LoadSceneMode.Single);
    }
    //goes through different scripts and saves info
    public void SaveGame(){
		playerMathStats.SaveState();
		PlayerPrefs.SetString("PlayerName", playerName);
	}

    public void ActivatePotionShop()
    {
        potionShop.gameObject.SetActive(true);
    }
}
