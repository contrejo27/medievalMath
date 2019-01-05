using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class levelSelect : MonoBehaviour {

    public Button[] levelButtons;


	// Use this for initialization
	void Start () {
        UnlockLevelButtons();
	}

    void UnlockLevelButtons(){
        int levelsUnlockedUI = GameStateManager.instance.levelsUnlocked;
        int i = 0;
        foreach (Button btn in levelButtons){
            if (levelsUnlockedUI < i){
                btn.interactable = true;
            }
            i++;
        }
    }

    public void loadLevel(string level){
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
