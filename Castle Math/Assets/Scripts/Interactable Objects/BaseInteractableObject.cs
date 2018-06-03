using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseInteractableObject : MonoBehaviour {
    PlayerController playerController;
    EventTrigger eventTrigger;

    // Use this for initialization
    void Start () {
        Init();	
	}

    protected virtual void Init() {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnPassOver()
    {
        //Debug.Log("passing over " + name);
    }

    public virtual void OnEndPassOver()
    {
        //Debug.Log("end pass over " + name);
    }

    public virtual void OnInteract()
    {
        //playerController = eventTrigger.
        //Debug.Log("Interact with " + name);
    }


}
