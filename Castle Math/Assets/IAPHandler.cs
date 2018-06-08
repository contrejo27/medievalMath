using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;
public class IAPHandler : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField]private Text _description;
    private Button _button;
    private IAPProduct _product;

	private void Awake()
	{
        _button = GetComponentInChildren<Button>();
        if (_description) _description.text = "";
        if (_button) _button.interactable = false;
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
        _product = product;

        string prodDescription = "";
        prodDescription += _product.Description;
        prodDescription += "\n";
        prodDescription += _product.Price;

        if (_description) _description.text = prodDescription;

        if (_button) 
        {
            _button.interactable = true;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(PurchaseProduct);
        }
    }

    // Purhcase the product
    void PurchaseProduct()
    {
        switch (_product.Name)
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
