using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class GameData : MonoBehaviour
{
    public GameSession gameSession;
    public GameRound gameRound;
    public GameResponse gameResponse;
    public TelemetryManager telManager;
    public string sessionFile = "TestSession";
    public string roundFile = "TestRound";
    public string responseFile = "TestResponse";
    string sessionPath;
    string roundPath;
    string responsePath;

    //Debugging
    public bool create = false;
    public bool load = false;

    public static GameData instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //roundPath = Application.persistentDataPath + "/testRound.json";
        sessionPath = System.IO.Path.Combine(Application.persistentDataPath, sessionFile + ".json");
        roundPath = System.IO.Path.Combine(Application.persistentDataPath, roundFile + ".json");
        responsePath = System.IO.Path.Combine(Application.persistentDataPath, responseFile + ".json");
        InitializeFiles();
        telManager = Object.FindObjectOfType<TelemetryManager>();
        
        //gameRound.SetID("1234565", "yeet", PlayerPrefs.GetString("playerName"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("Logged Round");
            //telManager.LogRound();
            //StartCoroutine(telManager.APIPut("round", 5, telManager.RoundPayload()));
            
        }
    }

    private void OnApplicationQuit()
    {
        UpdateSessionData();
        UpdateResponseData();
        UpdateRoundData();
        StartCoroutine(telManager.ExitApplication());
    }

    public string GetCurrentTime()
    {
        string time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return time;
    }

    public void InitializeID()
    {
        //Attach session id to round, and round id to response
        gameRound.session_id = gameSession.id;
        gameResponse.round_id = gameRound.id;
        
    }

    public void InitializeFiles()
    {
        CreateSessionData();
        UpdateSessionData();
        CreateRoundData();
        UpdateRoundData();
    }

    //Session Functions******************************************************************
    public void CreateSessionData()
    {
        if (!System.IO.File.Exists(roundPath) || create)
        {
            Debug.Log("Created the testing json file");
            //Loads default
            GameSession gameSession = new GameSession();     
            string gameSessionData = JsonUtility.ToJson(gameSession, true);
            gameSession.startTime = Time.time;
            System.IO.File.WriteAllText(roundPath, gameSessionData);
            Debug.Log(sessionPath);
            Debug.Log(gameSessionData);
        }
        else
        {
            LoadSessionData();
            //gameSession.startTime = Time.time;
        }
    }

    //Loads the variable from the file stored in persistent path
    public void LoadSessionData()
    {
        if (System.IO.File.Exists(roundPath))
        {
            string jsonData = System.IO.File.ReadAllText(sessionPath);
            Debug.Log("File exists");
            JsonUtility.FromJsonOverwrite(jsonData, gameSession);
            //gameSession = JsonUtility.FromJsonOverwrite<GameSession>(jsonData);
            Debug.Log(jsonData);
        }
        else
        {
            Debug.LogError("File does not exist");
        }
    }

    public string GetSessionData()
    {
        string jsonData = System.IO.File.ReadAllText(sessionPath);

        return jsonData;
    }

    public GameSession GetSessionServerData(string serverJson)
    {
        GameSession serverSession = new GameSession();
        var parsedServerInfo = JSON.Parse(serverJson);

        serverSession.game_id = parsedServerInfo["game_id"].Value;
        serverSession.id = parsedServerInfo["id"].AsInt;
        serverSession.system_id = parsedServerInfo["system_id"].Value;
        serverSession.time_created = parsedServerInfo["time_created"].Value;
        serverSession.time_updated = parsedServerInfo["time_updated"].Value;
        serverSession.user_id = parsedServerInfo["user_id"].AsInt;

        return serverSession;
    }

    public void SetSessionData(GameSession otherSessionData, bool canWriteID)
    {
        gameSession.game_id = otherSessionData.game_id;
        if (canWriteID)
            gameSession.id = otherSessionData.id;
        gameSession.system_id = otherSessionData.system_id;
        gameSession.time_created = otherSessionData.time_created;
        gameSession.time_updated = otherSessionData.time_updated;
        gameSession.user_id = otherSessionData.user_id;

        UpdateSessionData();
    }

    public void UpdateSessionData()
    {
        //overwrites locally stored session data
        string gameSessionData = JsonUtility.ToJson(gameSession, true);

        System.IO.File.WriteAllText(sessionPath, gameSessionData);
    }

    //Round Functions*************************************************************************************
    public void CreateRoundData()
    {
        Debug.Log("Created the testing json file");

        //Loads default
        GameRound gameRound = new GameRound();
        gameRound.Level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        //Storing locally
        string gameRoundData = JsonUtility.ToJson(gameRound, true);


        System.IO.File.WriteAllText(roundPath, gameRoundData);
        //Debug.Log(roundPath);
        //Debug.Log(gameRoundData);
    }

    //Loads the variable from the file stored in persistent path
    public void LoadRoundData()
    {
        if (System.IO.File.Exists(roundPath))
        {
            string jsonData = System.IO.File.ReadAllText(roundPath);
            Debug.Log("File exists");
            JsonUtility.FromJsonOverwrite(jsonData, gameRound);
            //gameRound = JsonUtility.FromJson<GameRound>(jsonData);
            Debug.Log(jsonData);
        }
        else
        {
            Debug.LogError("File does not exist");
        }
    }

    public string GetRoundData()
    {
        string jsonData = System.IO.File.ReadAllText(roundPath);

        return jsonData;
    }

    public GameRound GetRoundServerData(string serverJson)
    {
        GameRound serverRound = new GameRound();
        var parsedServerInfo = JSON.Parse(serverJson);

        serverRound.barrier1_health = parsedServerInfo["barrier1_health"].AsInt;
        serverRound.barrier2_health = parsedServerInfo["barrier2_health"].AsInt;
        serverRound.barrier3_health = parsedServerInfo["barrier2_health"].AsInt;
        serverRound.difficulty = parsedServerInfo["difficulty"].Value;
        serverRound.id = parsedServerInfo["id"].AsInt;
        serverRound.level_name = parsedServerInfo["level_name"].Value;
        serverRound.max_wave = parsedServerInfo["max_wave"].AsInt;
        serverRound.mode = parsedServerInfo["mode"].Value;
        serverRound.score = parsedServerInfo["score"].AsInt;
        serverRound.session_id = parsedServerInfo["session_id"].AsInt;
        serverRound.time_created = parsedServerInfo["time_created"].Value;
        serverRound.time_updated = parsedServerInfo["time_updated"].Value;
        serverRound.won = parsedServerInfo["won"].AsBool;

        return serverRound;
    }

    public void SetRoundData(GameRound otherRoundData, bool canWriteID)
    {
        gameRound.barrier1_health = otherRoundData.barrier1_health;
        gameRound.barrier2_health = otherRoundData.barrier2_health;
        gameRound.barrier3_health = otherRoundData.barrier3_health;
        gameRound.difficulty = otherRoundData.difficulty;
        if (canWriteID)
            gameRound.id = otherRoundData.id;
        gameRound.level_name = otherRoundData.level_name;
        gameRound.max_wave = otherRoundData.max_wave;
        gameRound.mode = otherRoundData.mode;
        gameRound.score = otherRoundData.score;
        gameRound.session_id = otherRoundData.session_id;
        gameRound.time_created = otherRoundData.time_created;
        gameRound.time_updated = otherRoundData.time_updated;
        gameRound.won = otherRoundData.won;

        UpdateRoundData();
    }

    public void UpdateRoundData()
    {
        //overwrites locally stored round data
        string gameRoundData = JsonUtility.ToJson(gameRound, true);

        System.IO.File.WriteAllText(roundPath, gameRoundData);
    }

    //Response Functions******************************************************************
    public void CreateResponseData()
    {
        if (!System.IO.File.Exists(roundPath) || create)
        {
            Debug.Log("Created the testing json file");
            //Loads default
            GameResponse gameResponse = new GameResponse();
            string gameResponseData = JsonUtility.ToJson(gameResponse, true);

            System.IO.File.WriteAllText(roundPath, gameResponseData);
            Debug.Log(responsePath);
            Debug.Log(gameResponseData);
        }
        else
        {
            LoadResponseData();
            //gameResponse.startTime = Time.time;
        }
    }

    //Loads the variable from the file stored in persistent path
    public void LoadResponseData()
    {
        if (System.IO.File.Exists(roundPath))
        {
            string jsonData = System.IO.File.ReadAllText(responsePath);
            Debug.Log("File exists");
            JsonUtility.FromJsonOverwrite(jsonData, gameResponse);
            //gameResponse = JsonUtility.FromJsonOverwrite<GameResponse>(jsonData);
            Debug.Log(jsonData);
        }
        else
        {
            Debug.LogError("File does not exist");
        }
    }

    public string GetResponseData()
    {
        string jsonData = System.IO.File.ReadAllText(responsePath);

        return jsonData;
    }

    public GameResponse GetResponseServerData(string serverJson)
    {
        GameResponse serverResponse = new GameResponse();
        var parsedServerInfo = JSON.Parse(serverJson);

        serverResponse.attempts = parsedServerInfo["attempts"].AsInt;
        serverResponse.correct = parsedServerInfo["correct"].AsInt;
        serverResponse.question = parsedServerInfo["question"].Value;
        serverResponse.round_id = parsedServerInfo["round_id"].AsInt;
        serverResponse.solution = parsedServerInfo["solution"].Value;
        serverResponse.time_created = parsedServerInfo["time_created"].Value;
        serverResponse.time_updated = parsedServerInfo["time_updated"].Value;

        return serverResponse;
    }

    public void SetResponseData(GameResponse otherResponseData, bool canWriteID)
    {
        gameResponse.attempts = otherResponseData.attempts;
        gameResponse.correct = otherResponseData.correct;
        gameResponse.question = otherResponseData.question;
        gameResponse.round_id = otherResponseData.round_id;
        gameResponse.solution = otherResponseData.solution;
        gameResponse.time_created = otherResponseData.time_created;
        gameResponse.time_updated = otherResponseData.time_updated;

        UpdateResponseData();
    }

    public void UpdateResponseData()
    {
        //overwrites locally stored response data
        string gameResponseData = JsonUtility.ToJson(gameResponse, true);

        System.IO.File.WriteAllText(responsePath, gameResponseData);
    }
}
