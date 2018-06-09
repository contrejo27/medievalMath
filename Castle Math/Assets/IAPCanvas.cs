using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;

public class IAPCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IAPHandler _iapPrefab;
    [SerializeField] private Transform _iapDisplay;

    private IAPProduct[] _products;

	private void Start()
	{
        StartCoroutine(CoWaitForIAPInitializtion());
	}

    void InitializeProducts()
    {
        bool isInitialized = InAppPurchasing.IsInitialized();
        if(isInitialized)
        {
            if(!_iapPrefab)
            {
                Debug.LogError("No IAP Prefab set in Inspector!");
                return;
            }

            if(!_iapDisplay)
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

                IAPHandler iap = Instantiate(_iapPrefab, _iapDisplay);
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
}
