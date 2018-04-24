using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject loginPopup;
    public Animator mathMenu;
    public Button m_loginButton;

	// Use this for initialization
	public void Start () {
		//StartCoroutine(ActivatorVR("Cardboard"));
	}
	
	public IEnumerator ActivatorVR(string vrToggle){
		UnityEngine.XR.XRSettings.LoadDeviceByName(vrToggle);
		yield return null;
		UnityEngine.XR.XRSettings.enabled = true;
		yield return new WaitForSeconds(.1f);
		SceneManager.LoadScene (1);
	}

	public void loadGame () {
		StartCoroutine(ActivatorVR("Cardboard"));
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
	public void Quit()
	{
		Application.Quit ();
	}

}
