using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round
{
    [System.Serializable]
    public class GameRound
    {
        public string id;
        public string session_id;
        public string username;
        public int barrier_health;
        public int wave = 0;
        public string level_name;
        public int score = 0;
        public int correct = 0;
        public int incorrect = 0;
        public int attempts = 0;
        public string mode;


        public GameRound()
        {
            //set the session id, and username, and id of round
            SetID();
        }

        void SetID()
        {
            id = "1234";
            session_id = "12345";
        }
    }
}
