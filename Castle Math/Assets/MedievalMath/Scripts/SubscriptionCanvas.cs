using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#pragma warning disable 0649

public class SubscriptionCanvas : CanvasNavigation
{
    [Header("References")]
    [SerializeField] private IAPProductDisplay productPrefab;
    [SerializeField] private Transform subscriptionDisplay;
    //[SerializeField] private Button loginButton;
	//[SerializeField] private Text activeSubText;

   
	void OnEnable()
	{
		StartCoroutine (CoWaitForIAPInitializtion ());
		Refresh ();
	}

	public void Refresh()
	{
        /*if (loginButton)
		{
			loginButton.gameObject.SetActive (LocalUserData.IsLoggedIn () == false);
			loginButton.onClick.RemoveAllListeners ();
			loginButton.onClick.AddListener (LogInPressed);
		}
        
		if (activeSubText)
		{
			activeSubText.text = "";

			activeSubText.gameObject.SetActive((LocalUserData.IsSubActive()));

			if(LocalUserData.IsSubActive())
			{
				activeSubText.text = "You already have an active subscription!\n";
				activeSubText.text += "Days Left:" + LocalUserData.GetDaysLeftOfSub ().ToString();
			}
		}*/
    }
/*
    void OnDisable()
	{
		StopAllCoroutines ();
		if (loginButton)
			loginButton.onClick.RemoveAllListeners ();
	}
	void OnDestroy()
	{
		StopAllCoroutines ();
		if (loginButton)
			loginButton.onClick.RemoveAllListeners ();

	}*/
	void InitializeProducts()
    {
        /*
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
        */
    }

    IEnumerator CoWaitForIAPInitializtion()
    {
        yield return null;
        /*
        bool isInitialized = InAppPurchasing.IsInitialized();

        while(isInitialized == false)
        {
			yield return new WaitForSeconds (.5f);
			Debug.Log ("Waiting for initialization...");
            isInitialized = InAppPurchasing.IsInitialized();
            yield return null;
        }

        if (isInitialized)
            InitializeProducts();
        else
            Debug.LogWarning("IAP Not initialized!");*/
    }

    void LogInPressed()
    {
		GoToNextCanvas(destroyPrevious:false);
    }
}
