using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Login_Canvas : CanvasNavigation 
{
#pragma warning disable
    [SerializeField] private GameObject signUpCanvas;

    [Header("UI References")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Text feedbackText;

    MathController mController;

	private void Start()
	{
        if (loginButton) loginButton.onClick.AddListener(LoginPressed);
        if (signUpButton) signUpButton.onClick.AddListener(SignUpPressed);

        //mController = GameObject.Find("mathController").GetComponent<MathController>();

        if (feedbackText) feedbackText.text = "";
	}

    void liteLogin()
    {
        //login is set to 0 so this will launch game without unlocking full content
        mController.StartGame();
    }

    void LoginPressed()
    {
        if (feedbackText) feedbackText.text = "";

        if (DatabaseManager.instance == null) return;

        if(emailInput.text == "" || passwordInput.text == "")
        {
            feedbackText.text = "All fields must be entered!";
            return;
        }

        if(DatabaseManager.instance.IsEmailValid(emailInput.text) == false)
        {
            feedbackText.text = "Email not found!";
            return;
        }

        if(DatabaseManager.instance.IsPasswordValid(emailInput.text, passwordInput.text) == false)
        {
            feedbackText.text = "Incorrect password entered!";
            return;
        }

        Debug.Log("Login Successful!");
        PlayerPrefs.SetString("LoggedInEmail", emailInput.text.ToLower());
        GoToNextCanvas();
    }

    void SignUpPressed()
    {
        if (signUpCanvas)
            GoToNextCanvas(signUpCanvas);
    }
}
