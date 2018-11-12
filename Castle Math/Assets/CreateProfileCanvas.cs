using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;
using System.Security.Cryptography;

public class CreateProfileCanvas : CanvasNavigation
{
    [Header("Profile Entry References")]
#pragma warning disable
    [SerializeField] ProfileEntry displayName = null;
    [SerializeField] ProfileEntry email = null;
    [SerializeField] ProfileEntry password = null;
    [SerializeField] ProfileEntry confirmPassword = null;

    [Header("UI References")]
    [SerializeField] Button signUpButton;
    [SerializeField] Text errorText;

#pragma warning restore

    private void Start()
    {
        if (signUpButton) signUpButton.onClick.AddListener(SignUpPressed);
        if (errorText) errorText.text = "";
    }

    void SignUpPressed()
    {
        errorText.text = "";

        if (!IsProfileValid())
            return;

        UserNameTemp = displayName.InputField.text;
        UserEmailTemp = email.InputField.text;
        UserPasswordTemp = password.InputField.text;
        DaysLeftTemp = 0;

        GoToNextCanvas();
    }

    bool IsProfileValid()
    {
        if  (displayName.InputField.text == "" ||
            email.InputField.text == "" ||
            password.InputField.text == "" ||
            confirmPassword.InputField.text == "")
        {
            DisplayErrorMessage("All fields must be filled in.");
            return false;
        }
        else if (DatabaseManager.instance && DatabaseManager.instance.IsEmailValid(email.InputField.text))
        {
            DisplayErrorMessage("Email already exists.");
            return false;
        }
        else if (password.InputField.text != confirmPassword.InputField.text)
        {
            DisplayErrorMessage("Passwords must match.");
            return false;
        }

        return true;
    }

    void DisplayErrorMessage(string message)
    {
        if (errorText == null) return;
        errorText.text = message;
    }
}
