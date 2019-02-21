using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSession
{
    public string Game_id = "MedievalMath";
    public string game_id
    {
        get
        {
            return Game_id;
        }
        set
        {
            Debug.Log("game id changed to: " + value);
            Game_id = value;
        }
    }
    public int id;
    public string system_id;
    public int user_id;
    //public float startTime;
    //public string stopTime;

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
}
