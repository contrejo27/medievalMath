using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;


public class IAPProductDisplay : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Text description = null;

    private Button button;
    private IAPProduct product;

	private void Awake()
	{
        button = GetComponentInChildren<Button>();
        if (description) description.text = "";
        if (button) button.interactable = false;
	}

    #region Event Subscriptions
    private void OnEnable()
    {
        InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
    }

    private void OnDisable()
    {
        InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
        InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
    }
    #endregion

    // Initialize the product and display proper details
	public void InitializeProduct(IAPProduct product)
    {
        this.product = product;

        string prodDescription = "";
        prodDescription += product.Description;
        prodDescription += "\n";
        prodDescription += product.Price;

        if (description) description.text = prodDescription;

        if (button) 
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PurchaseProduct);
        }
    }

    // Purhcase the product
    void PurchaseProduct()
    {
        switch (product.Name)
        {
            case EM_IAPConstants.Product_Small_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_Small_Subscription);
                break;
            case EM_IAPConstants.Product_Medium_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_Medium_Subscription);
                break;
            case EM_IAPConstants.Product_Large_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_Large_Subscription);
                break;
        }
    }

    // Successful purchase handler
    void PurchaseCompletedHandler(IAPProduct product)
    {
        switch(product.Name)
        {
            case EM_IAPConstants.Product_Small_Subscription:
                Debug.Log("Purchased Small Subscription");
                break;
            case EM_IAPConstants.Product_Medium_Subscription:
                Debug.Log("Purchased Medium Subscription");
                break;
            case EM_IAPConstants.Product_Large_Subscription:
                Debug.Log("Purchased Large Subscription");
                break;
        }
    }

    // Failed purchase handler
    void PurchaseFailedHandler(IAPProduct product)
    {
        Debug.Log("The Purchase of product " + product.Name + " has failed.");
    }

}
