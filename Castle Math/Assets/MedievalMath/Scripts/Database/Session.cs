using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session
{
    [System.Serializable]
    public class GameSession    
    {
        public string id;
        public string user_id;
        public string system_id;
        public string game_id = "MedievalMath";
    }
}
