using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ControllerMode
{
    MENU,
    GAME,
    MENU_AND_GAME
}

public class ControllerInput : MonoBehaviour 
{
    [SerializeField] private ControllerMode controllerMode = ControllerMode.MENU;

    private Button currentButton = null;
	
	// Update is called once per frame
	void Update () 
    {
        if(controllerMode == ControllerMode.MENU || controllerMode == ControllerMode.MENU_AND_GAME)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if(hit.collider)
                {
                    if (hit.transform.GetComponent<Button>())
                    {
                        currentButton = hit.transform.GetComponent<Button>();
                        //currentButton.Select();
                    }
                    else
                    {
                        currentButton = null;
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    currentButton = null;
                }

            }
        }
    }
}
