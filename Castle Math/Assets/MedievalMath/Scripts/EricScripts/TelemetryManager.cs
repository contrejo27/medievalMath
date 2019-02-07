using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;

public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;
    
    // NOTE: DO NOT HARD CODE API login. Keep info private and outside git
    public string API_URL;
    
    // Data hooks
    private MathManager m_mathmanager;
    private MathController m_mathcontroller;
    private WaveMathManager m_wavemathmanager;
    private WaveManager m_wavemanager;
    public PlayerMathStats m_playermathstats;
    public DoorHealth[] m_barriers;

    //Local class variables for round and sesion
    private GameRound gameRound;
    private GameSession gameSession;

    //Local Paths for storing round and session data
    //TODO: On close, clear/wipe data in these text files.
    string roundFilePath = Application.persistentDataPath + "/GameRound.json";
    string sessionFilePath = Application.persistentDataPath + "/GameSession.json";

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(this);
        // TODO: Understand why GameMetrics was attaching to UnityInitializer
        // Amazon.UnityInitializer.AttachToGameObject(this.gameObject);
        if (instance.API_URL == "") {
            instance.API_URL = "lucerna-api.herokuapp.com/api/";
        }

        Init();
   }

   public void Init() {
        m_mathmanager = GameObject.FindObjectOfType<MathManager>();
        m_mathcontroller = GameObject.FindObjectOfType<MathController>();
        m_wavemathmanager = GameObject.FindObjectOfType<WaveMathManager>();
        m_wavemanager = GameObject.FindObjectOfType<WaveManager>();
        m_playermathstats = m_mathmanager.GetComponent<PlayerMathStats>();
        m_barriers = GameObject.FindObjectsOfType<DoorHealth>();
    }

    private void Start() {
        
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
            LogRound();
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

    IEnumerator NewAPIPost(string key, string jsonPayload) {
        string url = "http://" + instance.API_URL + "log/" + key;
        Debug.Log("Server: " + url);

        var www = new UnityWebRequest(url, "POST");
        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.Send();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log(www.downloadHandler.text);
            // byte[] results = www.downloadHandler.data;
        }
    }

    public static List<string> APIRead(string propertyName, string dataRange) {
        //TODO: Implement Read()
        Debug.Log("TelemetryManager.APIRead() is not implemented yet...\nSorry o_o");
        return new List<string>();
    }

    public string addJson<T>(string old, string key, T val) {
        if (old == "") {
            return string.Format("\"{0}\":\"{1}\"", key, val.ToString());
        }

        return string.Format("{0},\"{1}\":\"{2}\"", old, key, val.ToString());
    }

    public string RoundPayload() {
        // TODO: Specialize RoundPayLoad()
        return SessionPayload();
    }

    public string SessionPayload() {

        string playerName = PlayerPrefs.GetString("playerName");
        string tutorialDone = PlayerPrefs.GetString("tutorialDone");
        string skillLevel = PlayerPrefs.GetInt("Skill Level").ToString();
        string stopTime = Time.time.ToString();
        string score = PlayerPrefs.GetInt("score").ToString();
        string levelsUnlocked = GameStateManager.instance.levelsUnlocked.ToString();

        string jsonPayload;
        string payload = "";
        // Player Telemetry
        payload = addJson(payload, "playerName", playerName);
        payload = addJson(payload, "score", score);
        payload = addJson(payload, "skillLevel", skillLevel);
        payload = addJson(payload, "tutorialDone", tutorialDone);
        payload = addJson(payload, "stopTime", stopTime);
        payload = addJson(payload, "levelsUnlocked", levelsUnlocked);

        // System Telemetry
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

        // Math Telemetry
        payload = addJson(payload, "correct", m_playermathstats.correctAnswers.ToString());
        payload = addJson(payload, "incorrect", m_playermathstats.incorrectAnswers.ToString());
        payload = addJson(payload, "totalAnswers ", m_mathmanager.totalQuestionsAnswered.ToString());
        payload = addJson(payload, "gradeNumber", m_playermathstats.gradeNumber.ToString());
        payload = addJson(payload, "personalHighScore", m_playermathstats.personalHighScore.ToString());
        payload = addJson(payload, "addOrSubtractScore", m_playermathstats.AddOrSubtractScore.ToString());
        payload = addJson(payload, "multiOrDivideScore", m_playermathstats.MultiOrDivideScore.ToString());
        payload = addJson(payload, "compareScore", m_playermathstats.CompareScore.ToString());
        payload = addJson(payload, "trueOrFalseScore", m_playermathstats.TrueOrFalseScore.ToString());
        payload = addJson(payload, "fractionScore", m_playermathstats.FractionScore.ToString());
        payload = addJson(payload, "question", m_mathmanager.currentQuestion.GetQuestionString());

        // Current Telemetry
        payload = addJson(payload, "wave", m_wavemanager.currentWave.ToString());
        payload = addJson(payload, "barrier1Health", m_barriers[0].currentHealth.ToString());
        payload = addJson(payload, "barrier2Health", m_barriers[1].currentHealth.ToString());
        payload = addJson(payload, "barrier3Health", m_barriers[2].currentHealth.ToString());
        // IncorrectAnswersPerCurrentQuestion
        // Debug.Log("incorrectAnswersPerCurrentQuestion" + m_mathmanager.IncorrectAnswersPerQuestion.ToString());
        // Debug.Log("currentQuestion:" + Question m_mathmanager.currentQuestion.ToString());
        // Debug.Log("questionType:" + m_mathmanager.questionType.ToString());

        // Debug.Log("m_wavemathmanager.mathDifficulty:" + m_wavemathmanager.mathDifficulty.ToString());
        // Debug.Log("m_wavemathmanager.ProblemType:" + m_wavemathmanager.ProblemType.ToString());
    
        // Debug.Log("aSupplier.NumberOfArrows:" + aSupplier.NumberOfArrows.ToString());
        // Debug.Log("Utility.SaveData PlayerData.questionTypesActive:" + Utility.SaveData PlayerData.questionTypesActive.ToString());
        // Debug.Log("player.PlayerMathStats:" + player.PlayerMathStats.ToString());
    
        // Debug.Log("Level 1 Completed: " + m_mathcontroller.level1_Completed.ToString());
        // Debug.Log("Level 2 Completed: " + m_mathcontroller.level2_Completed.ToString());
        // Debug.Log("Level 3 Completed: " + m_mathcontroller.level3_Completed.ToString());
        // Debug.Log("Level 4 Completed: " + m_mathcontroller.level4_Completed.ToString());
    
        // TODO: See if I can extract any useful information from these Unity Text Objects
        // Debug.Log("m_playermathstats.grade:" + m_playermathstats.grade.ToString());
        // Debug.Log("m_playermathstats.towerWave:" + m_playermathstats.towerWave.ToString());
        // Debug.Log("m_playermathstats.hsWave:" + m_playermathstats.hsWave.ToString());
        // Debug.Log("m_playermathstats.hsName:" + m_playermathstats.hsName.ToString());

        jsonPayload = "{" + payload + "}";

        //Debug.Log("JSON payload:\n" + jsonPayload);

        return jsonPayload;
    }

    public void LogRound() {
        StoreRound();
        StartCoroutine(NewAPIPost("round", RoundPayload()));
        // APIPost("round", RoundPayload());
    }

    public void LogSession() {
        StoreSession();
        StartCoroutine(NewAPIPost("session", SessionPayload()));
        // APIPost("session", SessionPayload());
    }

    public string ReadSession()
    {
        //TODO: Get json text value from session file
        
        return "session";
    }

    public void StoreSession()
    {
        //TODO: Assign class variables to GameSession and storing it locally.
        //Not the same as logging, but it's to store locally on the persistent path 
        
    }

    public string ReadRound()
    {
        //TODO: Get json text value from round file

        return "round";
    }

    public void StoreRound()
    {
        //TODO: Assign class variables to GameRound and storing it locally.
    }

    public void WipeRoundData()
    {
        //TODO: Delete all text data entry in the file for Round
        //Only use this during game close or when leaving the map/level.
    }

    public void WipeSessionData()
    {
        //TODO: Delete all text data entry in the file for Session
        //Only use this during game close.
    }
}
