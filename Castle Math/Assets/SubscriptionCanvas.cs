using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;

#pragma warning disable 0649

public class SubscriptionCanvas : CanvasNavigation
{
    [Header("References")]
    [SerializeField] private IAPProductDisplay productPrefab;
    [SerializeField] private Transform subscriptionDisplay;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button purchaseButton;

    private IAPProduct[] _products;

    private IAPProduct chosenProduct;

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

	private void Start()
	{
        StartCoroutine(CoWaitForIAPInitializtion());
        if (loginButton) 
        {
            loginButton.onClick.AddListener(LogInPressed);
            if (PlayerPrefs.HasKey("LoggedInEmail") && PlayerPrefs.GetString("LoggedInEmail") != null)
                loginButton.gameObject.SetActive(false);
        }
            
        if (purchaseButton) 
        {
            purchaseButton.onClick.AddListener(PurchaseSubPressed);
            if (!PlayerPrefs.HasKey("LoggedInEmail") || PlayerPrefs.GetString("LoggedInEmail") == null)
                purchaseButton.gameObject.SetActive(false);
        }
	}

	void InitializeProducts()
    {
        bool isInitialized = InAppPurchasing.IsInitialized();
        if(isInitialized)
        {
            if(!productPrefab)
            {
                Debug.LogError("No IAP Prefab set in Inspector!");
                return;
            }

            if(!subscriptionDisplay)
            {
                Debug.LogError("No IAP Display set in Inspector!");
                return;
            }

            _products = InAppPurchasing.GetAllIAPProducts();

            foreach(IAPProduct prod in _products)
            {
                string prodDescription = "";
                prodDescription += prod.Description;
                prodDescription += "\n";
                prodDescription += prod.Price;

                IAPProductDisplay iap = Instantiate(productPrefab, subscriptionDisplay);
                iap.InitializeProduct(prod);
            }
        }
        else
        {
            Debug.LogWarning("IAP Not Initialized!");
        }
    }

    IEnumerator CoWaitForIAPInitializtion()
    {
        bool isInitialized = InAppPurchasing.IsInitialized();
        float timeToWaitUntil = Time.time + 5.0f;

        while(isInitialized == false && Time.time < timeToWaitUntil)
        {
            isInitialized = InAppPurchasing.IsInitialized();
            yield return null;
        }

        if (isInitialized)
            InitializeProducts();
        else
            Debug.LogWarning("IAP Not initialized!");
    }

    void PurchaseSubPressed()
    {
        if (chosenProduct == null) return;

        switch (chosenProduct.Name)
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

    void LogInPressed()
    {
        GoToNextCanvas();
    }

    // Successful purchase handler
    void PurchaseCompletedHandler(IAPProduct purchasedProduct)
    {
        int daysAdded = 0;

        switch (purchasedProduct.Name)
        {
            case EM_IAPConstants.Product_Small_Subscription:
                Debug.Log("Purchased Small Subscription");
                daysAdded = 1;
                break;
            case EM_IAPConstants.Product_Medium_Subscription:
                Debug.Log("Purchased Medium Subscription");
                daysAdded = 3;
                break;
            case EM_IAPConstants.Product_Large_Subscription:
                Debug.Log("Purchased Large Subscription");
                daysAdded = 6;
                break;
        }

        // TODO add daysAdded amount to user's subscription
    }

    // Failed purchase handler
    void PurchaseFailedHandler(IAPProduct failedProduct)
    {
        Debug.Log("The Purchase of product " + failedProduct.Name + " has failed.");
    }

}
