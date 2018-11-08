using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

using UnityEngine;


public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;

    // NOTE: DO NOT HARD CODE API login. Keep info private and outside git
    public string API_URL;

    // Data hooks
    private MathManager m_mathmanager;
    private MathController m_mathcontroller;
    private WaveMathManager m_wavemathmanager;
    private WaveManager m_wavemanager;
    private PlayerMathStats m_playermathstats;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        // TODO: Understand why GameMetrics was attaching to UnityInitializer
        // UnityInitializer.AttachToGameObject(this.gameObject);
    }

    private void Start() {
        if (instance.API_URL == "") {
            instance.API_URL = "lucerna-api.herokuapp.com/api/";
        }
    
        m_mathmanager = GameObject.FindObjectOfType<MathManager>();
        m_mathcontroller = GameObject.FindObjectOfType<MathController>();
        m_wavemathmanager = GameObject.FindObjectOfType<WaveMathManager>();
        m_wavemanager = GameObject.FindObjectOfType<WaveManager>();
        m_playermathstats = GameObject.FindObjectOfType<PlayerMathStats>();
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
        string url = "http://" + instance.API_URL + "log/" + key;
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

    public string SessionPayload() {
        GameStateManager m_gameState = GetComponent<GameStateManager>();

        string playerName = PlayerPrefs.GetString("PlayerName");
        string tutorialDone = PlayerPrefs.GetString("tutorialDone");
        string skillLevel = PlayerPrefs.GetInt("Skill Level").ToString();
        string stopTime = Time.time.ToString();
        string score = PlayerPrefs.GetInt("score").ToString();
        string levelsUnlocked = m_gameState.levelsUnlocked.ToString();

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

        // TODO: Implement Current Telemetry
        payload = addJson(payload, "wave", m_wavemanager.currentWave.ToString());
        // IncorrectAnswersPerCurrentQuestion
        // Debug.Log("incorrectAnswersPerCurrentQuestion" + m_mathmanager.IncorrectAnswersPerQuestion.ToString());
        // Debug.Log("currentQuestion:" + Question m_mathmanager.currentQuestion.ToString());
        // Debug.Log("questionType:" + m_mathmanager.questionType.ToString());

        // Debug.Log("m_wavemanager.currentWave:" + m_wavemanager.currentWave.ToString());
        // Debug.Log("m_wavemathmanager.totalQuestionsAnswered:" + m_wavemathmanager.totalQuestionsAnswered.ToString());
        // Debug.Log("m_wavemathmanager.mathDifficulty:" + m_wavemathmanager.mathDifficulty.ToString());
        // Debug.Log("m_wavemathmanager.totalQuestionsAnswered:" + m_wavemathmanager.totalQuestionsAnswered.ToString());
        // Debug.Log("m_wavemathmanager.mathDifficulty:" + m_wavemathmanager.mathDifficulty.ToString());
        // Debug.Log("m_wavemathmanager.ProblemType:" + m_wavemathmanager.ProblemType.ToString());
        // Debug.Log("m_wavemathmanager.currentQuestion:" + m_wavemathmanager.currentQuestion.ToString());
    
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

        string jsonPayload = "{" + payload + "}";

        Debug.Log("Assemblem JSON payload...\n" + jsonPayload);

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
