using UnityEngine;


/// <summary>
/// controls user info like subscription and email
/// </summary>
public class UserManager : MonoBehaviour
{
    [HideInInspector]
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
        GameStateManager.instance.GetComponent<SendToGoogle>().SendCustom(SystemInfo.deviceModel.ToString() + ",Time since launch: " + Time.time.ToString() + ", ContentUnlocked, " + SystemInfo.deviceName.ToString() + ",-,-");

        currentActivation = newActivation;
        if (currentActivation == EnumManager.ActivationType.Paid)
        {
            LocalUserData.ActivateSubscription();
            MathController.instance.unlockMath();
        }
    }
}
