using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endMenuNav : MonoBehaviour {
    public GameObject levelSelectUI;
    public GameObject statUI;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadUI(string UI)
    { 
        if(UI == "levelSelect")
        {
            levelSelectUI.SetActive(true);
            statUI.SetActive(false);
        }
    } 
}
