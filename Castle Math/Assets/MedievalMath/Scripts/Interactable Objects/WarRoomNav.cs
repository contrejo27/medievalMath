using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarRoomNav : BaseInteractableObject {

    public GameObject showOnPassover;
    public GameObject spotCollider;
    [HideInInspector]
    bool isOccupied;
    WarRoomNav[] allNavs;


	// Use this for initialization
	protected override void Init () {
        allNavs = transform.parent.gameObject.GetComponentsInChildren<WarRoomNav>();
        Debug.Log("Warroom nav starting");
        base.Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnPassOver()
    {
        if(!isOccupied)
            showOnPassover.SetActive(true);
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        if (!isOccupied)
            showOnPassover.SetActive(false);
        base.OnEndPassOver();
    }

    public override void OnInteract()
    {
        if (!isOccupied)
        {
            foreach(WarRoomNav wrn in allNavs)
            {
                if (wrn.isOccupied)
                {
                    wrn.OnLeave();
                    break;
                }
            }

            spotCollider.SetActive(false);
            showOnPassover.SetActive(false);
            isOccupied = true;

            GameStateManager.instance.playerController.transform.position =
                new Vector3(
                    transform.position.x,
                    GameStateManager.instance.playerController.transform.position.y,
                    transform.position.z
                );
        }
        base.OnInteract();
    }

    public void OnLeave()
    {
        spotCollider.SetActive(true);
        isOccupied = false;
    }
}
