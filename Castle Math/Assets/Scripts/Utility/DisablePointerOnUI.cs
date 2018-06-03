using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisablePointerOnUI : MonoBehaviour {
    // Curently NOT automatic.
    // Functions still need to be assigned to the EventTrigger in the inpspector
    EventTrigger et;
	// Use this for initialization
	void Start () {
        //et = GetComponent<EventTrigger>();
        //et.OnPointerEnter += OnPointerEnter;
	}
	
    public void OnPointerEnter()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
    }

    public void OnPointerExit()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
