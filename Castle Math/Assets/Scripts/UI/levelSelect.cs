using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
		
	}
	
    public void loadLevel(string level)
    {
        switch (level)
        {
            case "kells":
                SceneManager.LoadScene("MathTest");
                break;
            case "frostLevel":
                SceneManager.LoadScene("frostLevel");
                break;
            case "desertLevel":
                SceneManager.LoadScene("desertLevel");
                break;
            case "vordum":
                SceneManager.LoadScene("BossLevel");
                break;
            default:
                break;
        }
       
    }
	// Update is called once per frame
	void Update () {
		
	}
}
