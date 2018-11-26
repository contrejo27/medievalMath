using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateAccount_Canvas : CanvasNavigation 
{
#pragma warning disable
    [Header("UI References")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button schoolButton;

    MathController mController;

	private void Start()
	{
        if (schoolButton) schoolButton.onClick.AddListener(SchoolSelected);
        if (homeButton) homeButton.onClick.AddListener(homePressed);

    }


    void SchoolSelected()
    {
            GoToNextCanvas();
    }
    void homePressed()
    {
        GoToNextCanvas();
    }
}
