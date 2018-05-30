using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePasswordPopup : MonoBehaviour 
{
    public Text m_passwordsDontMatchText;
    public InputField m_password1Input;
    public InputField m_password2Input;

    public Button m_submitButton;

	private void Awake()
	{
        m_passwordsDontMatchText.text = "";
        m_submitButton.onClick.AddListener(SubmitPassword);
	}

    void SubmitPassword()
    {
        m_passwordsDontMatchText.text = "";

        if (m_password1Input.text == "")
            PasswordsDontMatch("Password must contain valid input");
        else if (m_password1Input.text != m_password2Input.text)
            PasswordsDontMatch("Password's must match");
        else
            PasswordsMatch();
    }

    void PasswordsDontMatch(string returnMessage)
    {
        m_passwordsDontMatchText.text = "Passwords Don't Match";
    }

    void PasswordsMatch()
    {
        if(FindObjectOfType<LogInDatabase>())
            FindObjectOfType<LogInDatabase>().CreateUserData(m_password1Input.text);

        LoginPopup loginPopup = FindObjectOfType<LoginPopup>();

        if (loginPopup)
        {
            loginPopup.m_feedbackText.text = "Password Created!";
            loginPopup.m_input.text = "";
        }
        Destroy(this.gameObject);
    }

    public void GoBack()
    {
        Destroy(this.gameObject);
    }
}
