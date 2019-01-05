using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGem : BaseInteractableObject {

    public bool doesAdd;
    public EnumManager.GemType gemType;
    public ShopMenu shopMenu;

	// Use this for initialization
	void Start () {
        
    }

    public override void OnPassOver()
    {
        if (doesAdd)
        {
            if (shopMenu.dGemsAvailable[gemType] > 0) SetHighlight(Color.green);
            else SetHighlight(Color.red);
        }
        else
        {
            if (shopMenu.dGemsInShop[gemType] > 0) SetHighlight(Color.green);
            else SetHighlight(Color.red);
        }
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        RemoveHighlight();
        base.OnEndPassOver();
    }

    public override void OnInteract()
    {
        if (doesAdd && shopMenu.dGemsAvailable[gemType] > 0)
            shopMenu.AddGemToShop(gemType);
        else if (!doesAdd && shopMenu.dGemsInShop[gemType] > 0)
            shopMenu.RemoveGemFromShop(gemType);
        base.OnInteract();
    }

}
