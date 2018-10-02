using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using EasyMobile;

public class MenuManager : MonoBehaviour
{
    public Animator mathMenu;
    public Animator creditsScreen;
    public GameObject mathSelectMenu;
    public GameObject lessonSelectOptions;
    public GameObject detailedViewOptions;
    public Text instructions;

    public AudioSource UIAudio;
    public AudioClip splashScreenSountrack;
    public AudioClip btnClick;

    public MathController mController;

	public GameObject freeOrSubMenu;
    public GameObject loadingSign;

    [Header("Login References")]
	public GameObject loginPopup;
    public Button loginButton;
	public Button logOutButton;
	public Text userNameText;

	void Awake()
	{
		InAppPurchasing.InitializePurchasing ();
	}
	private void Start()
    {
        UIAudio.clip = splashScreenSountrack;
        UIAudio.Play();

		if(loginButton)
			loginButton.onClick.AddListener (OpenLoginPopup);
		if(logOutButton)
			logOutButton.onClick.AddListener (LogOut);
    }

	void Update()
	{
		RefreshLoginData ();
//		if(Time.time > timeToStart)
//			RefreshLoginData ();
	}

	public void RefreshLoginData()
	{
		if (loginButton) 
			loginButton.gameObject.SetActive (LocalUserData.IsLoggedIn () == false);

		if (logOutButton) 
			logOutButton.gameObject.SetActive (LocalUserData.IsLoggedIn ());

		if (userNameText)
			userNameText.text = (LocalUserData.IsLoggedIn () && DatabaseManager.instance) ? DatabaseManager.instance.GetUserName (LocalUserData.GetUserEmail ()) : "";
	}

	public void StartGameButtonPressed()
	{
		GameObject newCanvas = null;

        UIAudio.clip = btnClick;
        UIAudio.loop = false;
        UIAudio.Play();
        loadingSign.SetActive(true);
        mController.StartGame();
        
        // TODO check if sub is not active also (if logged in)
        //temp took this out so people with login can log in
        /*if ((LocalUserData.IsLoggedIn () == false || LocalUserData.IsSubActive() == false ) && !Application.isEditor && !Debug.isDebugBuild) {
			if (freeOrSubMenu)
				newCanvas = Instantiate (freeOrSubMenu) as GameObject;
		} 
		else */
    }
    private void OpenLoginPopup()
    {
        if (loginPopup)
            Instantiate(loginPopup);
        else
            Debug.LogError("No loginPopup attached on the menuManager object");
    }

	private void LogOut()
	{
		LocalUserData.DestroyPref ();
	}

    public void OpenMathMenu()
    {
        UIAudio.Play();
        mathMenu.Play("slideIn");
    }

    public void CloseMathMenu()
    {
        mathMenu.Play("slideOut");
    }
    public void OpenCredits()
    {
        UIAudio.Play();
        creditsScreen.Play("slideIn");
    }

    public void CloseCredits()
    {
        creditsScreen.Play("slideOut");
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