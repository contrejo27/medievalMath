using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;


public class IAPProductDisplay : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Text description = null;
	[SerializeField] private Button purchaseButton;

    private IAPProduct product;

	private void Awake()
	{
        if (description) description.text = "";
		if (purchaseButton) 
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(PurchaseProduct);
		}
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
        
		prodDescription += product.Name;
		prodDescription += "\n";
        prodDescription += product.Price;
		prodDescription += "\n";
		prodDescription += product.Description;

        if (description) description.text = prodDescription;
    }

	void Update()
	{
		if (purchaseButton)
		{
			purchaseButton.gameObject.GetComponentInChildren<Text> ().color = (LocalUserData.IsLoggedIn () && LocalUserData.IsSubActive() == false) ? Color.white : Color.grey;
			purchaseButton.interactable = (LocalUserData.IsLoggedIn () && LocalUserData.IsSubActive() == false);
		}
	}


    // Purhcase the product
    void PurchaseProduct()
    {
        switch (product.Name)
        {
            case EM_IAPConstants.Product_1_Month_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_1_Month_Subscription);
                break;
            case EM_IAPConstants.Product_6_Month_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_6_Month_Subscription);
                break;
            case EM_IAPConstants.Product_12_Month_Subscription:
                InAppPurchasing.Purchase(EM_IAPConstants.Product_12_Month_Subscription);
                break;
        }
    }

    // Successful purchase handler
    void PurchaseCompletedHandler(IAPProduct product)
    {
        switch(product.Name)
        {
            case EM_IAPConstants.Product_1_Month_Subscription:
                Debug.Log("Purchased Small Subscription");
                break;
            case EM_IAPConstants.Product_6_Month_Subscription:
                Debug.Log("Purchased Medium Subscription");
                break;
            case EM_IAPConstants.Product_12_Month_Subscription:
                Debug.Log("Purchased Large Subscription");
                break;
        }

		Destroy (this.transform.root.gameObject);
    }

    // Failed purchase handler
    void PurchaseFailedHandler(IAPProduct product)
    {
        Debug.Log("The Purchase of product " + product.Name + " has failed.");
    }

}
