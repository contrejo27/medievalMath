using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSession
{
    public string id;
    public string user_id;
    public string system_id;
    public float startTime;
    private string stopTime;
    public string game_id = "MedievalMath";

    public GameSession()
    {
        SetID();
    }

    public void SetID()
    {
        //get IDs from server
        user_id = "12345";
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
