using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseQuiver : BaseInteractableObject {
    
    public float cooldownTime = 5;

    protected ArrowModifier arrowModifier;

    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool isInCooldown;
    
    // Use this for initialization
    protected override void Init ()
    {
        base.Init();	
	}
	
	// Update is called once per frame
	void Update () {
       

	}

    public override void OnInteract()
    {
        //StartCoroutine(Cooldown());
        if (!isInCooldown && !GameStateManager.instance.inventory.isQuiverActive)
        {
            GameStateManager.instance.player.AddModifier(arrowModifier, -1);
            GameStateManager.instance.inventory.isQuiverActive = isActive = true;
        }else if (isActive)
        {
            GameStateManager.instance.player.RemoveModifier(arrowModifier);
            GameStateManager.instance.player.RefreshArrow();
            GameStateManager.instance.inventory.isQuiverActive = isActive = false;
        }

        base.OnInteract();
    }

    public void StartCooldown()
    {
        StartCoroutine(Cooldown());
    }

    public override void OnPassOver()
    {
        // Add a check for the other quivers not being activated
        // yellow if they are.
        if (GameStateManager.instance.inventory.isQuiverActive) SetHighlight(Color.red);
        else SetHighlight(Color.green);
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        if(!isActive && !isInCooldown)
            RemoveHighlight();

        base.OnEndPassOver();
    }

    IEnumerator Cooldown()
    {
        /*
        isActive = false;
        isInCooldown = true;
        */
        GameStateManager.instance.inventory.isQuiverActive = isActive = false;
        isInCooldown = true;
        SetHighlight(Color.yellow);
        yield return new WaitForSeconds(cooldownTime);
        isInCooldown = false;
        RemoveHighlight();
    }
}
