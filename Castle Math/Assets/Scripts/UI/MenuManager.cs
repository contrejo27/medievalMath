using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.VR;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public GameObject loginPopup;
    public Animator mathMenu;
    public GameObject mathSelectMenu;
    public GameObject lessonSelectOptions;
    public GameObject detailedViewOptions;
    public Text instructions;

    public AudioSource UIAudio;
    public AudioClip splashScreenSountrack;

    public Button m_loginButton;
    public MathController mController;

    public Text userInfoText;

    private void Awake()
    {
        if (userInfoText) userInfoText.text = "";
    }

	private void Start()
    {
        UIAudio.clip = splashScreenSountrack;
        UIAudio.Play();
        print("Logged in " + PlayerPrefs.GetInt("LoggedIn"));

        if(DatabaseManager.instance && PlayerPrefs.GetString("LoggedInEmail") != null)
        {
            string userEmail = PlayerPrefs.GetString("LoggedInEmail");
            if(userInfoText)
            {
                string userInfo = DatabaseManager.instance.GetUserName(userEmail);
                userInfo += "\n" + DatabaseManager.instance.GetDaysLeftOfSub(userEmail) + " Days Left";

                userInfoText.text = userInfo;
            }
        }
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("LoggedIn") == 1)
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

    public void loadGame()
    {
        if (PlayerPrefs.GetInt("LoggedIn") == 1)
        {
            mController.StartGame();
        }
        else
        {
            OpenLoginPopup();
        }
    }

    public void OpenLoginPopup()
    {
        if (loginPopup)
        {
            GameObject newLoginPopup = Instantiate(loginPopup, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
            Debug.LogError("No loginPopup attached on the menuManager object");
    }

    public void OpenMathMenu()
    {
        mathMenu.Play("slideIn");
    }

    public void CloseMathMenu()
    {
        mathMenu.Play("slideOut");
    }

    public void OpenMathSelectMenu()
    {
        mathSelectMenu.SetActive(true);
        mathSelectMenu.GetComponent<Animator>().Play("slideIn");
    }

    public void CloseMathSelectMenu()
    {
        mathSelectMenu.GetComponent<Animator>().Play("slideOut");
        StartCoroutine(hideAfterWait());
    }

    IEnumerator hideAfterWait()
    {
        yield return new WaitForSeconds(.3f);
        mathSelectMenu.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void updateMathMenu(string menuSelection)
    {
        if (menuSelection == "detailView")
        {
            instructions.text = "Scroll to view all results";
            lessonSelectOptions.SetActive(false);
            detailedViewOptions.SetActive(true);

        }
        if (menuSelection == "lessonSelect")
        {
            instructions.text = "Select in-game lessons";
            detailedViewOptions.SetActive(false);
            lessonSelectOptions.SetActive(true);
        }
    }


}