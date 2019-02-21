using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameResponse
{
    public string answer;
    public int attempts;
    public bool correct;
    public int id;
    public string question;
    public int round_id;
    public string solution;

    public int incorrect;
    public int totalAnswers;

    public GameResponse()
    {
        attempts = 0;
        round_id = 0;
    }
}
