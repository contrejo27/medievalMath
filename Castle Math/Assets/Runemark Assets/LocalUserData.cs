using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
#if EM_UIAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

public static class LocalUserData
{
    public static string loggedInPref = "LoggedInEmail";

    public static bool IsLoggedIn()
    {
        return PlayerPrefs.HasKey(loggedInPref) && PlayerPrefs.GetString(loggedInPref) != "";
    }

    public static string GetUserEmail()
    {
        return PlayerPrefs.GetString(loggedInPref);
    }

    public static void SetUserEmail(string userEmail)
    {
        PlayerPrefs.SetString(loggedInPref, userEmail);
    }

    public static bool IsSubActive()
    {
        // Check if logged in
        if (IsLoggedIn() == false)
            return false;

        Dictionary<string, string> dict = InAppPurchasing.StoreExtensionProvider.GetExtension<IAppleExtensions>().GetIntroductoryPriceDictionary();

        foreach (Product item in InAppPurchasing.StoreController.products.all)
        {
            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

                    if (intro_json != null)
                    {
                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        SubscriptionInfo info = p.getSubscriptionInfo();

                        if (info.isExpired() == Result.False && info.isSubscribed() == Result.True)
                            return true;
                    }

                }
            }
        }

        return false;
    }

    public static double GetDaysLeftOfSub()
    {
        Dictionary<string, string> dict = InAppPurchasing.StoreExtensionProvider.GetExtension<IAppleExtensions>().GetIntroductoryPriceDictionary();

        foreach (Product item in InAppPurchasing.StoreController.products.all)
        {
            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

                    if (intro_json != null)
                    {
                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        SubscriptionInfo info = p.getSubscriptionInfo();

                        if (info.isExpired() == Result.False && info.isSubscribed() == Result.True)
                            return info.getRemainingTime().TotalDays;
                    }

                }
            }
        }

        return 0;
    }

    public static void DestroyPref()
    {
        PlayerPrefs.DeleteKey(loggedInPref);
    }

//	static bool IsAppleReceiptActive()
//	{
//#if EM_UIAP
//		AppleInAppPurchaseReceipt receipt = null;

//		// Checked if user has ever purchased a sub
//		// If a receipt is valid for a sub, check if it is currently valid 

//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_1_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//					return true;
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_6_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//					return true;
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_12_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//					return true;
//			}
//		}

//#endif
//		if (Application.isEditor)
//			return true;
		
//		return false;

//	}

//	static int GetAppleReceiptDaysLeft()
//	{
//#if EM_UIAP
//		AppleInAppPurchaseReceipt receipt = null;

//		// Checked if user has ever purchased a sub
//		// If a receipt is valid for a sub, check if it is currently valid 

//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_1_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//				{
//					Debug.Log(receipt.subscriptionExpirationDate.TimeOfDay);
//					Debug.Log(receipt.subscriptionExpirationDate.Date.Subtract(System.DateTime.Today).TotalDays);
//					return (int)receipt.subscriptionExpirationDate.Subtract(System.DateTime.Today).TotalDays;
//				}
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_6_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//				{
//					Debug.Log(receipt.subscriptionExpirationDate.Date.TimeOfDay);
//					Debug.Log(receipt.subscriptionExpirationDate.Date.Subtract(System.DateTime.Today).TotalDays);
//					return (int)receipt.subscriptionExpirationDate.Subtract(System.DateTime.Today).TotalDays;
//				}
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_12_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
//				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
//				{
//					Debug.Log(receipt.subscriptionExpirationDate.Date.TimeOfDay);
//					Debug.Log(receipt.subscriptionExpirationDate.Date.Subtract(System.DateTime.Today).TotalDays);
//					return (int)receipt.subscriptionExpirationDate.Subtract(System.DateTime.Today).TotalDays;
//				}
//			}
//		}

//#endif
//		if (Application.isEditor)
//			return 0;

//		return 0;

//	}

//	static bool IsGoogleReceiptActive()
//	{
//#if EM_UIAP
//		GooglePlayReceipt receipt = null;

//		// Checked if user has ever purchased a sub
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_1_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Purchase Token: " + receipt.purchaseToken);
//				Debug.Log("Purchase State: " + receipt.purchaseState);
//				Debug.Log("Purchase Date: " + receipt.purchaseDate);
//				return System.DateTime.Today.Date <= receipt.purchaseDate.AddDays(31); 

//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_6_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Purchase Token: " + receipt.purchaseToken);
//				Debug.Log("Purchase State: " + receipt.purchaseState);
//				Debug.Log("Purchase Date: " + receipt.purchaseDate);
//				return System.DateTime.Today.Date <= receipt.purchaseDate.AddDays(186); 

//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_12_Month_Subscription);

//			if (receipt != null) 
//			{
//				Debug.Log("Purchase Token: " + receipt.purchaseToken);
//				Debug.Log("Purchase State: " + receipt.purchaseState);
//				Debug.Log("Purchase Date: " + receipt.purchaseDate);
//				return System.DateTime.Today.Date <= receipt.purchaseDate.AddDays(365); 

//			}
//		}

//#endif
//		if (Application.isEditor)
//			return true;
		
//		return false;
//	}

//	static int GetGoogleReceiptDaysLeft()
//	{
//#if EM_UIAP
//		GooglePlayReceipt receipt = null;

//		// Checked if user has ever purchased a sub
//		// If a receipt is valid for a sub, check if it is currently valid 

//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_1_Month_Subscription);

//			if (receipt != null) 
//			{
//				System.DateTime expirationDate = receipt.purchaseDate.AddDays(31); 
//				int daysLeft = (int)expirationDate.Subtract(System.DateTime.Today).TotalDays;
//				if(daysLeft < 0) daysLeft = 0;

//				return daysLeft;
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_6_Month_Subscription);

//			if (receipt != null) 
//			{
//				System.DateTime expirationDate = receipt.purchaseDate.AddDays(186); 
//				int daysLeft = (int)expirationDate.Subtract(System.DateTime.Today).TotalDays;
//				if(daysLeft < 0) daysLeft = 0;

//				return daysLeft;
//			}
//		}
//		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
//		{
//			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_12_Month_Subscription);

//			if (receipt != null) 
//			{
//				System.DateTime expirationDate = receipt.purchaseDate.AddDays(365); 
//				int daysLeft = (int)expirationDate.Subtract(System.DateTime.Today).TotalDays;
//				if(daysLeft < 0) daysLeft = 0;

//				return daysLeft;
//			}
//		}

//#endif
	//	if (Application.isEditor)
	//		return 0;

	//	return 0;

	//}
	
}
