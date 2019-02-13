using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameRound gameRound;
    public GameSession gameSession;
    public TelemetryManager telManager;
    public string roundFile = "TestRound";
    public string sessionFile = "TestSession";
    string roundPath;
    string sessionPath;

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
        roundPath = System.IO.Path.Combine(Application.persistentDataPath, roundFile + ".json");
        sessionPath = System.IO.Path.Combine(Application.persistentDataPath, sessionFile + ".json");
        telManager = Object.FindObjectOfType<TelemetryManager>();
        if (!load)
        {
            CreateRoundData();
        } else
        {
            LoadRoundData();
        }

        gameRound.SetID("1234", "yeet", PlayerPrefs.GetString("playerName"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Logged Round");
            telManager.LogRound();
            
        }
    }

    //Round Functions*************************************************************************************
    public void CreateRoundData()
    {

        if (!System.IO.File.Exists(roundPath) || create)
        {
            Debug.Log("Created the testing json file");

            //Loads default
            GameRound gameRound = new GameRound();
            gameRound.Level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            //Storing locally
            string gameRoundData = JsonUtility.ToJson(gameRound, true);


            System.IO.File.WriteAllText(roundPath, gameRoundData);
            Debug.Log(roundPath);
            Debug.Log(gameRoundData);
        }
        else
        {
            LoadRoundData();
        }
    }

    public void LoadRoundData()
    {
        if (System.IO.File.Exists(roundPath))
        {
            string jsonData = System.IO.File.ReadAllText(roundPath);
            Debug.Log("File exists");
            gameRound = JsonUtility.FromJson<GameRound>(jsonData);
            Debug.Log(jsonData);
        } else
        {
            Debug.LogError("File does not exist");
        }
    }

    public string GetRoundData()
    {
        string jsonData = System.IO.File.ReadAllText(roundPath);

        return jsonData;
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
            gameSession.startTime = Time.time;
        }
    }

    public void LoadSessionData()
    {
        if (System.IO.File.Exists(roundPath))
        {
            string jsonData = System.IO.File.ReadAllText(sessionPath);
            Debug.Log("File exists");
            gameSession = JsonUtility.FromJson<GameSession>(jsonData);
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
}
