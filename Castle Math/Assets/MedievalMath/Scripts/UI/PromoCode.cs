using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromoCode : MonoBehaviour
{
    public Text feedback;
    public InputField promoCodeInput;

    public void PromoCodeSubmit()
    {
        if (CheckPromoCode())
        {
            feedback.text = "Promo Applied!";
            feedback.color = Color.green;

        }
        else
        {
            feedback.text = "Incorrect Code";
            feedback.color = Color.red;
        }
    }

    bool CheckPromoCode()
    {
        string validCode = "PREPTIME19";
        if (promoCodeInput.text == validCode)return  true;
        else return false;
    }
}
