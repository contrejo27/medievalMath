using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// controls user info like subscription and email
/// </summary>
public class UserManager : MonoBehaviour
{
    public EnumManager.ActivationType currentActivation;

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
