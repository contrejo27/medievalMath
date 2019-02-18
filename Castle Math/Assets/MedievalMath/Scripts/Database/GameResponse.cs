using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameResponse
{
    public string answer;
    public int attempts;
    public int correct;
    public int id;
    public string question;
    public int round_id;
    public string solution;
    public string time_created;
    public string time_updated;

    public int incorrect;
    public int totalAnswers;

    public GameResponse()
    {
        attempts = 0;
        round_id = 0;
    }
}
