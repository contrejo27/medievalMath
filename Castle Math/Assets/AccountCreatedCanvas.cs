using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountCreatedCanvas : MonoBehaviour
{
	#pragma warning disable
	[SerializeField] private Button continueButton;
	#pragma warning restore

	void Start()
	{
		if (continueButton)
			continueButton.onClick.AddListener (ContinuePressed);
	}

	void ContinuePressed()
	{
		Destroy (this.gameObject);
	}

}
