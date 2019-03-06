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
    private GameData m_gameData;

    //Local Paths for storing round and session data
    //TODO: On close, clear/wipe data in these text files.
    string roundFilePath;
    string sessionFilePath;

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
        roundFilePath = Application.persistentDataPath + "/GameRound.json";
        sessionFilePath = Application.persistentDataPath + "/GameSession.json";
    }

   public void Init() {
        m_mathmanager = GameObject.FindObjectOfType<MathManager>();
        m_mathcontroller = GameObject.FindObjectOfType<MathController>();
        m_wavemathmanager = GameObject.FindObjectOfType<WaveMathManager>();
        m_wavemanager = GameObject.FindObjectOfType<WaveManager>();
        m_playermathstats = m_mathmanager.GetComponent<PlayerMathStats>();
        m_barriers = GameObject.FindObjectsOfType<DoorHealth>();
        m_gameData = GameObject.FindObjectOfType<GameData>();
    }

    private void Start()
    {
        StartCoroutine(InitializeAPI());
        
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
        
        /*
        if(Input.GetKeyDown(KeyCode.A))
        {
            LogRound();
            Debug.Log("LOGGED");
        }
        */
    }

    IEnumerator InitializeAPI()
    {
        yield return new WaitForSeconds(0.5f);
        yield return NewAPIPost("session", SessionPayload());
        yield return NewAPIPost("round", RoundPayload());
        yield return new WaitForSeconds(0.1f);

        //Responses will be managed in MathManager on GenerateProblem function
        //yield return NewAPIPost("response", ResponsePayload());
        m_gameData.InitializeID();
    }
    
    /* For testing purposes to delete excess data logs
    IEnumerator DeleteIt()
    {
        for (int i = 8; i <= 47; i++)
        {
            yield return StartCoroutine(DeleteAPIPosts("round", i));
        }
        Debug.Log("Completed deleting");
    }
    IEnumerator DeleteAPIPosts(string key, int id)
    {
        string url = "http://" + instance.API_URL + key + "/" + id;
        Debug.Log(url);
        using (UnityWebRequest www = UnityWebRequest.Delete(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }*/

    IEnumerator NewAPIPost(string key, string jsonPayload) {
        string url = "http://" + instance.API_URL + key;

        var www = new UnityWebRequest(url, "POST");
        //Debug.Log(jsonPayload);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            string jsonString = www.downloadHandler.text;
            Debug.Log(jsonString);
            string updateJson;

            if (key == "round")
            {
                m_gameData.SetRoundData(m_gameData.GetRoundServerData(jsonString), true);
                
            } else if(key == "session")
            {
                m_gameData.SetSessionData(m_gameData.GetSessionServerData(jsonString), true);
                //updateJson = JsonUtility.ToJson(m_gameData.gameSession, true);
                //yield return StartCoroutine(APIPut("session", m_gameData.gameSession.id, updateJson));
            } else if (key == "response")
            {
                m_gameData.SetResponseData(m_gameData.GetResponseServerData(jsonString), true);
            }
            // byte[] results = www.downloadHandler.data;
        }
    }

    /*
    public static List<string> APIRead(string propertyName, string dataRange) {
        //TODO: Implement Read()
        Debug.Log("TelemetryManager.APIRead() is not implemented yet...\nSorry o_o");
        return new List<string>();
    }*/
    public IEnumerator APIPut(string key, int id, string jsonPayload)
    {
        string url = "http://" + instance.API_URL + key + "/" + id;
        Debug.Log("Server: " + url);
        Debug.Log(jsonPayload);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");

        //Write data locally before uploading
        m_gameData.UpdateRoundData();
        m_gameData.UpdateResponseData();

        using (UnityWebRequest www = UnityWebRequest.Put(url, data))
        {
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }

    public string addJson<T>(string old, string key, T val) {
        if (old == "") {
            return string.Format("\"{0}\":\"{1}\"", key, val.ToString());
        }

        return string.Format("{0},\"{1}\":\"{2}\"", old, key, val.ToString());
    }

    public string RoundPayload() {
        // Player Telemetry
        m_gameData.gameRound.score = PlayerPrefs.GetInt("score");

        // Current Telemetry
        m_gameData.gameRound.max_wave = m_wavemanager.currentWave;
        m_gameData.gameRound.barrier1_health = m_barriers[0].currentHealth;
        m_gameData.gameRound.barrier2_health = m_barriers[1].currentHealth;
        m_gameData.gameRound.barrier3_health = m_barriers[2].currentHealth;       
        m_gameData.UpdateRoundData();

        return m_gameData.GetRoundData();
    }

    public string SessionPayload() {

        string playerName = PlayerPrefs.GetString("playerName");
        string tutorialDone = PlayerPrefs.GetString("tutorialDone");
        string skillLevel = PlayerPrefs.GetInt("Skill Level").ToString();
        string stopTime = Time.time.ToString();
        string levelsUnlocked = GameStateManager.instance.levelsUnlocked.ToString();

        string jsonPayload;
        string payload = "";
        string payloadSystem = "";
        // Player Telemetry
        payload = addJson(payload, "playerName", playerName);
        payload = addJson(payload, "skillLevel", skillLevel);
        payload = addJson(payload, "tutorialDone", tutorialDone);
        payload = addJson(payload, "stopTime", stopTime);
        payload = addJson(payload, "levelsUnlocked", levelsUnlocked);

        // System Telemetry
        payloadSystem = addJson(payloadSystem, "deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
        payloadSystem = addJson(payloadSystem, "deviceModel", SystemInfo.deviceModel);
        payloadSystem = addJson(payloadSystem, "operatingSystem", SystemInfo.operatingSystem);
        payloadSystem = addJson(payloadSystem, "graphicsDeviceVendorId", SystemInfo.graphicsDeviceVendorID);
        payloadSystem = addJson(payloadSystem, "graphicsDeviceId", SystemInfo.graphicsDeviceID);
        payloadSystem = addJson(payloadSystem, "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
        payloadSystem = addJson(payloadSystem, "graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded);
        payloadSystem = addJson(payloadSystem, "graphicsShaderLevel", SystemInfo.graphicsShaderLevel);
        payloadSystem = addJson(payloadSystem, "maxTextureSize", SystemInfo.maxTextureSize);
        payloadSystem = addJson(payloadSystem, "systemMemorySize", SystemInfo.systemMemorySize);
        payloadSystem = addJson(payloadSystem, "graphicsMemorySize", SystemInfo.graphicsMemorySize);
        payloadSystem = addJson(payloadSystem, "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
        payloadSystem = addJson(payloadSystem, "processorCount", SystemInfo.processorCount);
        payloadSystem = addJson(payloadSystem, "processorType", SystemInfo.processorType);
        payloadSystem = addJson(payloadSystem, "supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount);
        payloadSystem = addJson(payloadSystem, "supports2DArrayTextures", SystemInfo.supports2DArrayTextures);
        payloadSystem = addJson(payloadSystem, "supports3DRenderTextures", SystemInfo.supports3DRenderTextures);
        payloadSystem = addJson(payloadSystem, "supports3DTextures", SystemInfo.supports3DTextures);
        payloadSystem = addJson(payloadSystem, "supportsComputeShaders", SystemInfo.supportsComputeShaders);
        payloadSystem = addJson(payloadSystem, "supportsInstancing", SystemInfo.supportsInstancing);
        
        m_gameData.gameSession.system_id = payloadSystem;

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

        //payload = addJson(payload, "question", m_mathmanager.currentQuestion.GetQuestionString());


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

        //Debug.Log("JSON payload:\n" + payload);
        //Debug.Log(m_gameData.GetRoundData());

        //return jsonPayload;

        

        return m_gameData.GetSessionData();
    }

    public string ResponsePayload()
    {

        m_gameData.gameResponse.correct = m_mathmanager.GetComponent<AnswerInput>().GetIsCorrect();
        m_gameData.gameResponse.question = m_mathmanager.GetComponent<AnswerInput>().currentQuestion;
        m_gameData.gameResponse.incorrect = m_playermathstats.incorrectAnswers;
        m_gameData.gameResponse.totalAnswers = m_mathmanager.totalQuestionsAnswered;
        m_gameData.gameResponse.answer = m_mathmanager.GetComponent<AnswerInput>().selectedAnswer;
        m_gameData.gameResponse.solution = m_mathmanager.GetComponent<AnswerInput>().GetCorrectAnswer();
        m_gameData.gameResponse.attempts = m_mathmanager.GetComponent<PlayerMathStats>().attempts;


        return m_gameData.GetResponseData();
    }

    IEnumerator MMAnalytics()
    {
        //shortcut to both
        yield return StartCoroutine(NewAPIPost("round", RoundPayload()));
        yield return StartCoroutine(NewAPIPost("response", ResponsePayload()));
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

    public void LogResponse()
    {
        StoreResponse();
        StartCoroutine(NewAPIPost("response", ResponsePayload()));
    }

    public IEnumerator ExitApplication()
    {
        Debug.Log("Exiting Application");
        yield return StartCoroutine(NewAPIPost("session", SessionPayload()));
        yield return StartCoroutine(NewAPIPost("round", RoundPayload()));
        yield return StartCoroutine(NewAPIPost("response", ResponsePayload()));

        Debug.Log("Completed Exiting application and uploaded analytics");
    }

    public string ReadSession()
    {
        //TODO: Get json text value from session file
        
        return m_gameData.GetSessionData();
    }

    public void StoreSession()
    {
        //TODO: Assign class variables to GameSession and storing it locally.
        //Not the same as logging, but it's to store locally on the persistent path 
        m_gameData.UpdateSessionData();
    }

    public string ReadRound()
    {
        //TODO: Get json text value from round file

        return m_gameData.GetRoundData();
    }

    public void StoreRound()
    {
        //TODO: Assign class variables to GameRound and storing it locally.\
        m_gameData.UpdateRoundData();
    }

    public string ReadResponse()
    {
        //TODO: Get json text value from Response file

        return m_gameData.GetResponseData();
    }

    public void StoreResponse()
    {
        //TODO: Assign class variables to GameResponse and storing it locally.\
        m_gameData.UpdateResponseData();
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
