using UnityEngine;
using UnityEngine.UI;

public class SubscribeButtonBehavior : MonoBehaviour
{
    public Text TitleText;
    public Text Description;
    public Text Price;
    enum subscribeButton { PromoCode, NormalPurchase };
    subscribeButton subscribeType = subscribeButton.NormalPurchase;

    public void ConvertToPromoButton()
    {
        subscribeType = subscribeButton.PromoCode;
        TitleText.text = "Unlock";
        Price.text = "0$";
        Description.text = "for 2 weeks!";
        Color approvalGreen = new Color(.503f, 1, .539f);
        GetComponent<Image>().color = approvalGreen;
    }

    public void PurchaseContent()
    {
        if (subscribeType == subscribeButton.PromoCode)
        {
            
            UserManager.instance.UpdateActivation(EnumManager.ActivationType.Paid);
            UserManager.instance.StartFreeTrialTime();
        }
        else
        {
            GetComponent<IAP_Manager>().PurchaseContent();
        }

    }
}
