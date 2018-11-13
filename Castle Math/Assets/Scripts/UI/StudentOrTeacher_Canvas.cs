using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StudentOrTeacher_Canvas : CanvasNavigation 
{
#pragma warning disable
    [Header("UI References")]
    [SerializeField] private Button studentButton;
    [SerializeField] private Button teacherButton;

    MathController mController;

	private void Start()
	{
        if (teacherButton) teacherButton.onClick.AddListener(SchoolSelected);
        if (studentButton) studentButton.onClick.AddListener(homePressed);

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
