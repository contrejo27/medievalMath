using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginPopup : MonoBehaviour 
{
    public Text m_instructionsText;
    public Text m_feedbackText;

    public InputField m_input;
    public Button m_loginButton;

    public Button m_liteButton;

    public GameObject m_createPasswordObject;

    private LogInDatabase m_database;

    mathController mController;

	private void Start()
	{
        m_database = FindObjectOfType<LogInDatabase>();
        mController = GameObject.Find("mathController").GetComponent<mathController>();
        if (m_database == null)
            Debug.LogError("No <b>LogInDatabase</b> object found in scene");

        //m_instructionsText.text = "Enter Email";
        m_feedbackText.text = "";

        m_input.ActivateInputField();
        m_input.Select();

        m_liteButton.onClick.AddListener(liteLogin);

        m_loginButton.onClick.AddListener(SubmitEmail);
        m_loginButton.transform.GetChild(0).GetComponent<Text>().text = "Submit";
	}

    void liteLogin()
    {
        //login is set to 0 so this will launch game without unlocking full content
        mController.StartGame();
    }

    void SubmitEmail()
    {
        m_feedbackText.text = "";

        if(m_database.IsEmailValid(m_input.text)) // if email is found in database 
        {
            if(m_database.DoesEmailHavePassword()) // if there is a password associated with the email
                GetPassword(); 
            else // otherwise 
            {
                // Spawn Create Password pop up 
                GameObject createPasswordObject = Instantiate(m_createPasswordObject);
            }
        }
        else // otherwise, 
        {
            m_feedbackText.text = "Email Not Found";
            m_input.text = "";
        }
    }

    void SubmitPassword()
    {
        m_feedbackText.text = "";

        if (m_database.IsPasswordValid(m_input.text)) {
            SuccessfulLogin();
            mController.StartGame();
        }
        else{
            m_feedbackText.text = "Invalid Password";
            m_input.text = "";
        }
    }

    void GetPassword()
    {
        m_instructionsText.text = "Enter Password";
        m_input.text = "";
        m_input.contentType = InputField.ContentType.Password;

        m_input.ActivateInputField();
        m_input.Select();

        m_loginButton.onClick.RemoveAllListeners();
        m_loginButton.onClick.AddListener(SubmitPassword);
        m_loginButton.transform.GetChild(0).GetComponent<Text>().text = "Log In";
       
    }

    void SuccessfulLogin()
    {
        Debug.Log("Logged in!");
        PlayerPrefs.SetInt("LoggedIn", 1);
        Destroy(this.gameObject);
    }

    public void Exit()
    {
        Destroy(this.gameObject);
    }

}
