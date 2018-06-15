using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public static class LocalUserData 
{
	public static string loggedInPref = "LoggedInEmail";

	public static bool IsLoggedIn()
	{
		return PlayerPrefs.HasKey (loggedInPref) && PlayerPrefs.GetString (loggedInPref) != "";
	}

	public static string GetUserEmail()
	{
		return PlayerPrefs.GetString (loggedInPref);
	}

	public static void SetUserEmail(string userEmail)
	{
		PlayerPrefs.SetString (loggedInPref, userEmail);
	}

	public static bool IsSubActive()
	{
		// Check if logged in
		if (IsLoggedIn () == false)
			return false;

		// Check for valid receipt
		#if UNITY_IOS
		return IsAppleReceiptActive();
		#elif UNITY_ANDROID
		return IsGoogleReceiptActive();
		#endif

		return false;
	}

	static bool IsAppleReceiptActive()
	{
		AppleInAppPurchaseReceipt receipt = null;

		// Checked if user has ever purchased a sub
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
		{
			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_1_Month_Subscription);

			if (receipt != null) 
			{
				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
					return true;
			}
		}
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
		{
			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_6_Month_Subscription);

			if (receipt != null) 
			{
				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
					return true;
			}
		}
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
		{
			receipt = InAppPurchasing.GetAppleIAPReceipt(EM_IAPConstants.Product_12_Month_Subscription);

			if (receipt != null) 
			{
				Debug.Log("Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());
				if (receipt.subscriptionExpirationDate.Date >= System.DateTime.Today.Date)
					return true;
			}
		}

		if (Application.isEditor)
			return true;
		
		return false;

	}

	static bool IsGoogleReceiptActive()
	{
		GooglePlayReceipt receipt = null;

		// Checked if user has ever purchased a sub
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_1_Month_Subscription))
		{
			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_1_Month_Subscription);

			if (receipt != null) 
			{
			}
		}
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_6_Month_Subscription))
		{
			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_6_Month_Subscription);

			if (receipt != null) 
			{
				// Check receipt activity status 
			}
		}
		if (InAppPurchasing.IsProductOwned (EM_IAPConstants.Product_12_Month_Subscription))
		{
			receipt = InAppPurchasing.GetGooglePlayReceipt(EM_IAPConstants.Product_12_Month_Subscription);

			if (receipt != null) 
			{
				// Check receipt activity status 
			}
		}

		if (Application.isEditor)
			return true;
		
		return false;
	}
	public static void DestroyPref()
	{
		PlayerPrefs.DeleteKey (loggedInPref);
	}
}
