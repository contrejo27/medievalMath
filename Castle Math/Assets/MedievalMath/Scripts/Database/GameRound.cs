using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameRound
{
    public int barrier1_health;
    public int barrier2_health;
    public int barrier3_health;
    public string difficulty;
    public int id;
    public string level_name;
    public int max_wave = 0;
    public string mode;
    public int score = 0;
    public int session_id;

    public bool won;

    public GameRound()
    {
        //set the session id, and username, and id of round
        //SetID(); 
        barrier1_health = 100;
        barrier2_health = 100;
        barrier3_health = 100;
        //All other variables will be set from telemetry
    }

    public void SetID(int idAsInit, int sessionIdAsInit, string usernameAsInit)
    {
        id = idAsInit;
        session_id = sessionIdAsInit;
        //username = usernameAsInit;
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
            //Debug.Log("Level Name in Game Round has been changed to: " + level_name);
        }
    }

}
