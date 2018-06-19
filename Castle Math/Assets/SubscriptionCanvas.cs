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
	[SerializeField] private Text activeSubText;

    private IAPProduct[] _products;

    private int daysLeft = 0;

	private void Start()
	{
        StartCoroutine(CoWaitForIAPInitializtion());
        if (loginButton) 
            loginButton.onClick.AddListener(LogInPressed);
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

			_products = InAppPurchasing.GetAllIAPProducts ();

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

    void LogInPressed()
    {
		GoToNextCanvas(destroyPrevious:false);
    }

    void Update()
    {
		if (loginButton)
			loginButton.gameObject.SetActive (LocalUserData.IsLoggedIn () == false);

		if (activeSubText)
			activeSubText.gameObject.SetActive((LocalUserData.IsSubActive()));
    }
}
