using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;

public class CreateProfile2Canvas : CanvasNavigation
{
    [Header("Profile Entry References")]
#pragma warning disable
    [Header("UI References")]
    [SerializeField] Button signUpButton;

#pragma warning restore

    private void Start()
    {
        //if (signUpButton) signUpButton.onClick.AddListener(SignUpPressed);
    }

    void SignUpPressed()
    {
       /* if (!IsProfileValid())
            return;*/
       
		string hashPass = PasswordEncryption.Md5Sum (UserPasswordTemp);

        if (DatabaseManager.instance)
        {
            DatabaseManager.UserData userData = new DatabaseManager.UserData
            {
                UserName = UserNameTemp,
                UserEmail = UserEmailTemp,
                UserPassword = hashPass,
                DaysLeft = DaysLeftTemp
            };

            DatabaseManager.instance.CreateNewProfile(userData);
			LocalUserData.SetUserEmail (UserEmailTemp.ToLower ());
			GoToNextCanvas ();
        }
    }
    /*
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
    }*/


}
