using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSession
{
    public string game_id = "MedievalMath";
    public int id;
    public string system_id;
    public string time_created;
    public string time_updated;
    public int user_id;
    public float startTime;
    public string stopTime;

    public GameSession()
    {
        SetID();
    }

    public void SetID()
    {
        //get IDs from server
        user_id = 12345;
        system_id = "12345";
    }

    public string StopTime
    {
        get
        {
            return (float.Parse(stopTime) - this.startTime).ToString();
        }
        set
        {
            stopTime = (float.Parse(value) - this.startTime).ToString();
        }
    }
}
