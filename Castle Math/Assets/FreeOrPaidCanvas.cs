using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class FreeOrPaidCanvas : CanvasNavigation 
{
    [Header("References")]

    [SerializeField] private Button freeButton;
    [SerializeField] private Button subscriptionButton;

	private void Start()
	{
        // Add button listeners
        if (freeButton) freeButton.onClick.AddListener(FreeButtonPressed);
		if (subscriptionButton) subscriptionButton.onClick.AddListener (SubscriptionButtonPressed);
	}

    void FreeButtonPressed()
    {
        Debug.Log("Free button pressed");

		MathController mController = FindObjectOfType<MathController> ();
		if (mController)
			mController.StartGame ();
		// TODO start free demo 
    }

    void SubscriptionButtonPressed()
    {
		GoToNextCanvas ();
    }
}
