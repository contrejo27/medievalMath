using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.VR;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject loginPopup;
    public Animator mathMenu;
    public Button m_loginButton;
    public mathController mController;

    private void Start()
    {
        print("Logged in " + PlayerPrefs.GetInt("LoggedIn"));
    }
    
    private void Update()
	{
        if(PlayerPrefs.GetInt("LoggedIn") == 1)
        {
            m_loginButton.interactable = false;
            m_loginButton.transform.GetChild(0).GetComponent<Text>().text = "Logged In";
        }
        else
        {
            m_loginButton.interactable = true;
            m_loginButton.transform.GetChild(0).GetComponent<Text>().text = "Log In";
        }
	}

    public void loadGame(){
        if (PlayerPrefs.GetInt("LoggedIn") == 1){
            mController.StartGame();
        }
        else{
            OpenLoginPopup();
        }
    }

	public void OpenLoginPopup()
    {
        if(loginPopup)
        {
            GameObject newLoginPopup = Instantiate(loginPopup, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
            Debug.LogError("No loginPopup attached on the menuManager object");
    }

    public void OpenMathMenu(){
		mathMenu.Play("slideIn");
    }

    public void CloseMathMenu(){
		mathMenu.Play("slideOut");
    }

	public void Quit(){
		Application.Quit ();
	}

}
