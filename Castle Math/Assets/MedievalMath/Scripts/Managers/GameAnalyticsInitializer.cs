using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAnalyticsInitializer : MonoBehaviour
{
    public GameData gameData;


    // Start is called before the first frame update
    void Awake()
    {
        gameData = GameObject.FindObjectOfType<GameData>();
    }

    private void Start()
    {
        gameData.gameRound.difficulty = "Normal";
        gameData.gameRound.level_name = "Kells";
        gameData.gameRound.mode = "Add/Subtract";

    }
}
