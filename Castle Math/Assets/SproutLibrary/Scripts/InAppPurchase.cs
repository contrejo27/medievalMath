using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class InAppPurchase : MonoBehaviour
{
    const string subStatus = "Subbed";
    private List<MyStoreProducts> subscriptions = new List<MyStoreProducts>();

    private void Start()
    {
        //initializeIAP
        IAPManager.Instance.InitializeIAPManager(InitializeResult);
    }


    /// <summary>
    /// automatically called after initialization is success
    /// </summary>
    /// <param name="status">The initialization result: Success/Failed</param>
    /// <param name="message">Error message if status is failed</param>
    /// <param name="shopProducts">list of all shop products, can be used to unlock update some game values</param>
    private void InitializeResult(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
    {
        subscriptions = new List<MyStoreProducts>();

        if (status == IAPOperationStatus.Success)
        {
            //IAP was succesfully initialized
            //loop through all products and check which one are bought to update our variables
            for (int i = 0; i < shopProducts.Count; i++)
            {

                if (shopProducts[i].productName == "Subscription")
                {
                    //if a subscription is active meand that the subscription is still valid so enable access
                    if (shopProducts[i].active)
                    {
                        //subscription = true;
                    }
                }

                //construct a different list of each category of products, for an easy display purpose, not required
                switch (shopProducts[i].productType)
                {
                    case ProductType.Subscription:
                        subscriptions.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                }
            }
        }
        else
        {
            //Error initializing IAP
        }

        if (IAPManager.Instance.debug)
        {
            Debug.Log("Init status: " + status + " message " + message);
            GleyEasyIAP.ScreenWriter.Write("Init status: " + status + " message " + message);
        }
    }

    public class MyStoreProducts
    {
        public ShopProductNames name;
        public bool bought;

        public MyStoreProducts(ShopProductNames name, bool bought)
        {
            this.name = name;
            this.bought = bought;
        }
    }

    public void BuySubscription()
    {
        IAPManager.Instance.BuyProduct(subscriptions[0].name, ProductBought);
    }

    //0 is not subscribed, 1 is subscribed
    public static void SetSubscription(int isSub)
    {
        PlayerPrefs.SetInt(subStatus, isSub);
    }

    //use to check if player has subscribed
    public static bool IsSubscriptionBought()
    {
        int status = 0;
        if (PlayerPrefs.HasKey(subStatus))
        {
            status = PlayerPrefs.GetInt(subStatus);
        }
        if (status == 0)
        {
            return false;
        }
        else return true;
    }

    private void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            if (IAPManager.Instance.debug)
            {
                Debug.Log("Buy product completed: " + product.localizedTitle + " receive value: " + product.value);
                GleyEasyIAP.ScreenWriter.Write("Buy product completed: " + product.localizedTitle + " receive value: " + product.value);
            }
            
            if (product.productType == ProductType.Subscription)
            {
                SetSubscription(1);
                gameObject.SetActive(false);
            }
        }
        else
        {
            //en error occured in the buy process, log the message for more details
            if (IAPManager.Instance.debug)
            {
                Debug.Log("Buy product failed: " + message);
                GleyEasyIAP.ScreenWriter.Write("Buy product failed: " + message);
            }
        }
    }
    /*
    const string subStatus = "Subbed";


    private void Start()
    {
        //initializeIAP
        IAPManager.Instance.InitializeIAPManager(InitComplete);
    }

    private void InitComplete(IAPOperationStatus status, string arg1, List<StoreProduct> arg2)
    {
        throw new NotImplementedException();
    }

    //0 is not subscribed, 1 is subscribed
    public static void SetSubscription(int isSub)
    {
        PlayerPrefs.SetInt(subStatus, isSub);
    }

    public void Subscribe()
    {
        IAPManager.Instance.BuyProduct(ShopProductNames.SproutXRSubscription, ProductBought);
    }
    
    public static bool IsSubscriptionBought()
    {
        int status = 0;
        if (PlayerPrefs.HasKey(subStatus))
        {
            status = PlayerPrefs.GetInt(subStatus);
        }
        if(status == 0)
        {
            return false;
        }
        return true;
    }

    private void ProductBought(string errorMessage, StoreProduct boughtProduct)
    {
        if(boughtProduct.productName == ShopProductNames.SproutXRSubscription.ToString())
        {
            Save.SetSubscription();
        }
    }*/
}
