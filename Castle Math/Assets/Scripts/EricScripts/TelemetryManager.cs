using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

using UnityEngine;


public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;


    // Variables to keep database details private and outside git
    public string DB_URL;
    public string DB_DATABASE;
    public string DB_USER;
    public string DB_PASSWORD;
    public string API_URL;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        // TODO: Understand why GameMetrics was attaching to UnityInitializer
        /* UnityInitializer.AttachToGameObject(this.gameObject); */
    }

    private void Start() {
        if (instance.API_URL == "") {
            instance.API_URL = "medieval-math.herokuapp.com/api/";
        }
    }

    private void Update() {
        /// <summary>
        /// Unity Function that runs after every screen update
        /// </summary>
        if (Application.isEditor) {
            TestAPI();
	}
    }


    /////////
    // API //
    /////////
    
    // TODO: Move API functions to separte file

    private void TestAPI() {
        /// <summary>
        /// Check which key is being pressed and sends a request to the API
        /// </summary>

        // Number row above QWERTY
        if(Input.GetKeyDown(KeyCode.Alpha0)) {
            LogSession();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            PlayerPrefs.SetInt("score", Random.Range(0,1000));
	    Debug.Log("Score set to: " + PlayerPrefs.GetInt("score").ToString());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            PlayerPrefs.SetInt("score", Random.Range(1000,2000));
	    Debug.Log("Score set to: " + PlayerPrefs.GetInt("score").ToString());
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            PlayerPrefs.SetInt("score", Random.Range(2000,3000));
	    Debug.Log("Score set to: " + PlayerPrefs.GetInt("score").ToString());
        }
    }

    public static void APIPost(string key, string jsonPayload) {
	string url = "http://" + instance.API_URL + key;
	Debug.Log(url);
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";


        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
            streamWriter.Write(jsonPayload);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
            var result = streamReader.ReadToEnd();
            Debug.Log(result);
        }
    }

    public List<string> APIRead(string propertyName, string dataRange) {
        //TODO: Implement Read()
        Debug.Log("TelemetryManager.Read() is not implemented yet...\nSorry o_o");
        return new List<string>();
    }

    public string addJson<T>(string old, string key, T val) {
        if (old == "") {
    	return string.Format("\"{0}\":\"{1}\"", key, val.ToString());
        }
        return string.Format("{0},\"{1}\":\"{2}\"", old, key, val.ToString());
    }

    public string SessionPayload() {
        GameStateManager m_gameState = GetComponent<GameStateManager>();

        string playerName = PlayerPrefs.GetString("PlayerName");
        string tutorialDone = PlayerPrefs.GetString("tutorialDone");
        string skillLevel = PlayerPrefs.GetInt("Skill Level").ToString();
        string stopTime = Time.time.ToString();
        string score = PlayerPrefs.GetInt("score").ToString();
        string levelsUnlocked = m_gameState.levelsUnlocked.ToString();

        // TODO: Remove fake
        playerName = "eu";
        skillLevel = "9001";
        string fake_waves = Random.Range(1, 10).ToString();
        string fake_arrows = Random.Range(10, 50).ToString();

	string payload = "";
	payload = addJson(payload, "playerName", playerName);
	payload = addJson(payload, "score", score);
	payload = addJson(payload, "skillLevel", skillLevel);
	payload = addJson(payload, "tutorialDone", tutorialDone);
	payload = addJson(payload, "stopTime", stopTime);
	payload = addJson(payload, "levelsUnlocked", levelsUnlocked);
	payload = addJson(payload, "deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
	payload = addJson(payload, "deviceModel", SystemInfo.deviceModel);
	payload = addJson(payload, "operatingSystem", SystemInfo.operatingSystem);
	payload = addJson(payload, "graphicsDeviceVendorId", SystemInfo.graphicsDeviceVendorID);
	payload = addJson(payload, "graphicsDeviceId", SystemInfo.graphicsDeviceID);
	payload = addJson(payload, "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
	payload = addJson(payload, "graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded);
	payload = addJson(payload, "graphicsShaderLevel", SystemInfo.graphicsShaderLevel);
	payload = addJson(payload, "maxTextureSize", SystemInfo.maxTextureSize);
	payload = addJson(payload, "systemMemorySize", SystemInfo.systemMemorySize);
	payload = addJson(payload, "graphicsMemorySize", SystemInfo.graphicsMemorySize);
	payload = addJson(payload, "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
	payload = addJson(payload, "processorCount", SystemInfo.processorCount);
	payload = addJson(payload, "processorType", SystemInfo.processorType);
	payload = addJson(payload, "supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount);
	payload = addJson(payload, "supports2DArrayTextures", SystemInfo.supports2DArrayTextures);
	payload = addJson(payload, "supports3DRenderTextures", SystemInfo.supports3DRenderTextures);
	payload = addJson(payload, "supports3DTextures", SystemInfo.supports3DTextures);
	payload = addJson(payload, "supportsComputeShaders", SystemInfo.supportsComputeShaders);
	payload = addJson(payload, "supportsInstancing", SystemInfo.supportsInstancing);



        // TODO: Remove telemetry from MathController
	MathController m_mathcontroller = GetComponent<MathController>();
	// Debug.Log("Level 1 Completed: " + m_mathcontroller.level1_Completed.ToString());
	// Debug.Log("Level 2 Completed: " + m_mathcontroller.level2_Completed.ToString());
	// Debug.Log("Level 3 Completed: " + m_mathcontroller.level3_Completed.ToString());
	// Debug.Log("Level 4 Completed: " + m_mathcontroller.level4_Completed.ToString());
	WaveManager m_wavemanager = GetComponent<WaveManager>();
	// Debug.Log(m_wavemanager.currentWave);
	WaveMathManager m_wavemathmanager = GetComponent<WaveMathManager>();
	// Debug.Log(m_wavemathmanager.totalQuestionsAnswered);
	// Debug.Log(m_wavemathmanager.mathDifficulty);
	// m_wavemathmanager.totalQuestionsAnswered
	// int m_wavemathmanager.mathDifficulty
	// int m_wavemathmanager.ProblemType
	// Question m_wavemathmanager.currentQuestion
	MathManager m_mathmanager = GetComponent<MathManager>();
	// Question m_mathmanager.currentQuestion
	// int m_mathmanager.questionType
	// m_mathmanager.totalQuestionsAnswered
	// Debug.Log(m_mathmanager.IncorrectAnswersPerQuestion);
	// Utility.SaveData PlayerData.questionTypesActive
	// player.PlayerMathStats
	PlayerMathStats m_playermathstats = GetComponent<PlayerMathStats>();
	// Debug.Log(m_playermathstats.grade);
	// Debug.Log(m_playermathstats.hsWave);
	// Debug.Log(m_playermathstats.hsName);
	// Debug.Log(m_playermathstats.correctAnswers);
	// Debug.Log(m_playermathstats.gradeNumber);
	// Debug.Log(m_playermathstats.personalHighScore);
	// Debug.Log(m_playermathstats.AddOrSubtractScore);
	// Debug.Log(m_playermathstats.MultiOrDivideScore);
	// Debug.Log(m_playermathstats.CompareScore);
	// Debug.Log(m_playermathstats.TrueOrFalseScore);
	// Debug.Log(m_playermathstats.FractionScore);
	// mathstats.grade
	// mathstats.towerWave
	// mathstats.hsWave
	// mathstats.hsName
	// mathstats.correctAnswers
	// mathstats.gradeNumber
	// mathstats.incorrectAnswers
	// mathstats.gradeNumber
	// mathstats.personalHighScore
	// mathstats.AddOrSubtractScore
	// mathstats.MultiOrDivideScore
	// mathstats.CompareScore
	// mathstats.TrueOrFalseScore
	// mathstats.FractionScore
	// mathstats. aSupplier.NumberOfArrows
	// mathstats. wManager.currentWave

        string jsonPayload = "{" + payload + "}";

        Debug.Log("Made JSON payload");
        Debug.Log(jsonPayload);

	return jsonPayload;
    }

    public string RoundPayload() {
        // TODO: make RoundPayLoad()
        return SessionPayload();
    }

    public void LogRound(string level, bool b) {
        APIPost("round", RoundPayload());
    }

    public void LogSession() {
        APIPost("session", SessionPayload());
    }
}
