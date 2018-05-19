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
        if(level == "vordum")
        {
            SceneManager.LoadScene("BossLevel", LoadSceneMode.Single);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
