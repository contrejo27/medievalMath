using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalUserData
{
    public static string loggedInPref = "LoggedInEmail";

    public static bool IsLoggedIn()
    {
        return false;
    }

    public static void ActivateSubscription()
    {
        PlayerPrefs.SetInt("Subscribed", 1);
    }

    public static void DeactivateSubscription()
    {
        PlayerPrefs.SetInt("Subscribed", 0);
    }

    public static int GetSubscription()
    {
        return PlayerPrefs.GetInt("Subscribed");
    }

    public static string GetUserEmail()
    {
        if(PlayerPrefs.HasKey(loggedInPref)) return PlayerPrefs.GetString(loggedInPref); 
        else return "No email found"; 
    }

    public static void SetUserEmail(string userEmail)
    {
        PlayerPrefs.SetString(loggedInPref, userEmail);
    }

    public static bool IsSubActive()
    {
        return false;
    }

    public static double GetDaysLeftOfSub()
    {
            return 0;
    }

    public static void DestroyPref()
    {
       // PlayerPrefs.DeleteKey(loggedInPref);
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
