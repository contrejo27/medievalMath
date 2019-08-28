using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// controls user info like subscription and email
/// </summary>
public class UserManager : MonoBehaviour
{
    [HideInInspector]
    public EnumManager.ActivationType currentActivation;

    public InputField emailInput;
    public Text warningText;
    public AnimationHelper subscribeMenu;

    // Singleton
    public static UserManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    public void EmailCreatedAttempt()
    {
        if (!emailInput.text.Contains("@") || emailInput.text == "" || emailInput.text == null)
        {
            warningText.text = "Please enter valid email.";
        }
        else
        {
            LocalUserData.SetUserEmail(emailInput.text.ToLower());
            subscribeMenu.TriggerAnimation("slideOut");
            gameObject.GetComponent<SendToGoogle>().SendCustom(SystemInfo.deviceModel.ToString() + "," + Time.time.ToString() + ", EmailCreated, " + SystemInfo.deviceName.ToString() + ","+ LocalUserData.GetUserEmail() +",-");
        }
    }

    public void UpdateActivation(EnumManager.ActivationType newActivation)
    {
        GameStateManager.instance.GetComponent<SendToGoogle>().SendCustom(SystemInfo.deviceModel.ToString() + "," + Time.time.ToString() + ", ContentUnlocked, " + SystemInfo.deviceName.ToString() + ",-,-");

        currentActivation = newActivation;
        if(currentActivation == EnumManager.ActivationType.Paid)
        {
            MathController.instance.unlockMath();
        }
    }
}
