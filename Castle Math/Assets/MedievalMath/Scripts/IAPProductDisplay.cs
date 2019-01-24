using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IAPProductDisplay : MonoBehaviour 
{
    [Header("UI References")]
    [SerializeField] private Text description = null;
	[SerializeField] private Button purchaseButton;


    

	void Update()
	{
		if (purchaseButton)
		{
			purchaseButton.gameObject.GetComponentInChildren<Text> ().color = (LocalUserData.IsLoggedIn () && LocalUserData.IsSubActive() == false) ? Color.white : Color.grey;
			purchaseButton.interactable = (LocalUserData.IsLoggedIn () && LocalUserData.IsSubActive() == false);
		}
	}

    

}
