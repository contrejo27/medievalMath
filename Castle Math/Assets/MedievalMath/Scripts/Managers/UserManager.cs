using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// controls user info like subscription and email
/// </summary>
public class UserManager : MonoBehaviour
{
    [HideInInspector]
    public EnumManager.ActivationType currentActivation;
    Double freeTrialLeft;
    DateTime subscriptionStartTime;

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

        if (LocalUserData.GetSubscription() == 1)
        {
            currentActivation = EnumManager.ActivationType.Paid;
        }
        else
        {
            currentActivation = EnumManager.ActivationType.Free;
        }
    }


    public void UpdateActivation(EnumManager.ActivationType newActivation)
    {
        GameStateManager.instance.GetComponent<SendToWeb>().SendCustom(SystemInfo.deviceModel.ToString() + ",Time since launch: " + Time.time.ToString() + ", ContentUnlocked, " + SystemInfo.deviceName.ToString() + ",-,-");

        currentActivation = newActivation;
        if (currentActivation == EnumManager.ActivationType.Paid)
        {
            LocalUserData.ActivateSubscription();
            MathController.instance.unlockMath();
        }
    }

    public void StartFreeTrialTime()
    {
        subscriptionStartTime = DateTime.Now;
        CheckFreeTrialimeLeft();
    }

    public void CheckFreeTrialimeLeft()
    {
        if (subscriptionStartTime != null)
        {
            freeTrialLeft = 14d - (subscriptionStartTime - DateTime.Now).TotalDays;
            GameObject.Find("FreeTrialInfo").GetComponent<Text>().text = "Free Trial Left \n" + freeTrialLeft + " Days";
        }

    }
}
