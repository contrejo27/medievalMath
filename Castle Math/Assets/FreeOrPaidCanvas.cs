using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class FreeOrPaidCanvas : CanvasNavigation 
{
    [SerializeField] private GameObject subscriptionCanvas;
    [Header("References")]

    [SerializeField] private Button freeButton;
    [SerializeField] private Button subscriptionButton;

    [Space(5)]

    [SerializeField] private Text freeDescriptionText;
    [SerializeField] private Text subscriptionDescriptionText;

    [Header("Descriptions")]

    [SerializeField, TextArea(minLines:1, maxLines:5)] private string freeDescription;
    [SerializeField, TextArea(minLines: 1, maxLines: 5)] private string subscriptionDescription;

	private void Start()
	{
        // Set Descriptions
        if (freeDescriptionText) freeDescriptionText.text = freeDescription;
        if (subscriptionDescriptionText) subscriptionDescriptionText.text = subscriptionDescription;

        // Add button listeners
        if (freeButton) freeButton.onClick.AddListener(FreeButtonPressed);
        if (subscriptionButton) subscriptionButton.onClick.AddListener(SubscriptionbuttonPressed);
	}


    void FreeButtonPressed()
    {
        Debug.Log("Free button pressed");
    }

    void SubscriptionbuttonPressed()
    {
        if (subscriptionCanvas)
            GoToNextCanvas(subscriptionCanvas);
    }
}
