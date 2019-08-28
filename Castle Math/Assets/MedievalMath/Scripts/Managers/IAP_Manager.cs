using System.Collections.Generic;
using UnityEngine;

public class IAP_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        IAPManager.Instance.InitializeIAPManager(InitializeResultCallback);
    }

    // Update is called once per frame
    //this method will be called after initialization process is done
    private void InitializeResultCallback(IAPOperationStatus status, string message, List<StoreProduct>
    shopProducts)
    {
        if (status == IAPOperationStatus.Success)
        {
            //IAP was successfully initialized
            //loop through all products
            /* for (int i = 0; i < shopProducts.Count; i++)
             {
                 if (shopProducts[i].productName == "YourProductName")
                 {
                     //if active variable is true, means that user had bought that product
                     //so enable access
                     if (shopProducts[i].active)
                     {*/
            // UserManager.instance.UpdateActivation(EnumManager.ActivationType.Paid);
            //}
            //}
            //}
        }
        else
        {
            Debug.Log("Error occurred " + message);
        }
    }

    public void PurchaseContent()
    {
        if (GameStateManager.instance.isTestBuild)
        {
            UserManager.instance.UpdateActivation(EnumManager.ActivationType.Paid);
        }
        else
        {
            IAPManager.Instance.BuyProduct(ShopProductNames.oneMonthSubscription, ProductBoughtCallback);
        }
    }

    // automatically called after one product is bought
    private void ProductBoughtCallback(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            Debug.Log("Buy product completed");
            UserManager.instance.UpdateActivation(EnumManager.ActivationType.Paid);
        }
        else
        {
            //an error occurred in the buy process, log the message for more details
            Debug.Log("Buy product failed: " + message);
        }
    }
}
