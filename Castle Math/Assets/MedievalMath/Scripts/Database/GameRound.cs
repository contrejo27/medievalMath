using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameRound
{

    public string id;
    public string session_id;
    public string username;
    public int wave = 0;
    public string level_name;
    public int score = 0;
    public int correct = 0;
    public int incorrect = 0;
    public int totalAnswers = 0;
    public int attempts = 0;
    public string mode;
    public int barrier_health1;
    public int barrier_health2;
    public int barrier_health3;


    public GameRound()
    {
        //set the session id, and username, and id of round
        SetID(); 
        
        //All other variables will be set from telemetry
    }


    void SetID()
    {
        //get id given from server
        id = "1234";
        session_id = "yes";
        username = "default";
    }

    public void SetID(string idAsInit, string sessionIdAsInit, string usernameAsInit)
    {
        id = idAsInit;
        session_id = sessionIdAsInit;
        username = usernameAsInit;
    }

    public string Level_name
    {
        get
        {
            return this.level_name;
        }
        set
        {
            this.level_name = value;
            Debug.Log("Level Name in Game Round has been changed to: " + level_name);
        }
    }

}
