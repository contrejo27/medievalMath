using UnityEngine;
using UnityEngine.UI;

public class UnlockedContentUI : MonoBehaviour
{
    public InputField emailInput;
    public Text warningText;
    public AnimationHelper subscribeMenu;

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
            GameStateManager.instance.gameObject.GetComponent<SendToWeb>().SendCustom(SystemInfo.deviceModel.ToString() + "," + Time.time.ToString() + ", EmailCreated, " + SystemInfo.deviceName.ToString() + "," + LocalUserData.GetUserEmail() + ",-");
        }
    }
}
