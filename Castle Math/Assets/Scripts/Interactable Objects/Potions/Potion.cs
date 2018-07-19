using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Potion : BaseInteractableObject {

    enum PotionState { shop, menu, inventory }
    PotionState currentState;
    public Text toolTip;
    public float cost;
    public float duration;
    
    public GameObject purchaseConfirmationMenu;
    public GameObject tossConfirmationMenu;
    
    [HideInInspector]
    public int inventoryPosition;

    // stop tooltips/menus when another potion in the shop is selected
    [HideInInspector]
    public bool UIEnabled = true;

	// Use this for initialization

    protected override void Init()
    {
        currentState = PotionState.shop;
        
        base.Init();
    }
	
	// Update is called once per frame
	void Update () {
		if(currentState == PotionState.menu && GameStateManager.instance.currentState == EnumManager.GameState.Wave)
        {
            CloseTossMenu();
        }
	}

    /// <summary>
	/// Main function of the potion
	/// </summary>
    public virtual void DoEffect()
    {

        //Debug.Log("Doing effect!");
        DestroyPotionFromInventory();
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {
        
    }

    public override void OnInteract()
    {
        if (!GameStateManager.instance.levelManager.isGamePaused)
        {
            if (currentState == PotionState.shop && UIEnabled)
            {
                if (cost <= GameStateManager.instance.playerController.gemsOwned
                && !GameStateManager.instance.inventory.IsInventoryFull()
                )
                {
                    currentState = PotionState.menu;
                    purchaseConfirmationMenu.SetActive(true);
                    GameStateManager.instance.potionShop.DisablePotionUI(this);
                    /* Set menu text to show confirm? + relevant info
                     * Maybe have error messages for not enough money or 
                     * Inventory full
                     * Maybe raise potion up a bit?
                     */
                }
                else
                {
                    // error tooltip?
                }

            }
            else if (currentState == PotionState.inventory)
            {
                if (!GameStateManager.instance.player.IsUnderTheInfluence() && GameStateManager.instance.currentState == EnumManager.GameState.Wave)
                    DoEffect();
                else if (GameStateManager.instance.currentState != EnumManager.GameState.Wave && UIEnabled)
                {
                    currentState = PotionState.menu;
                    tossConfirmationMenu.SetActive(true);
                    GameStateManager.instance.inventory.DisablePotionUI(this);
                }
            }
            base.OnInteract();
        }
    }

    public void Purchase()
    {
        if (currentState == PotionState.menu)
        {

            currentState = PotionState.inventory;

            GameStateManager.instance.playerController.BuyItem((int)cost);

            GameStateManager.instance.inventory.AddPotion(gameObject);

            GameStateManager.instance.potionShop.RemoveFromShop(this);

            GameStateManager.instance.potionShop.EnablePotionUI();

            purchaseConfirmationMenu.SetActive(false);
            OnEndPassOver();
            
        }
        
    }

    public void ClosePurchaseMenu()
    {
        if (currentState == PotionState.menu)
        {

            currentState = PotionState.shop;

            GameStateManager.instance.potionShop.EnablePotionUI();

            purchaseConfirmationMenu.SetActive(false);
            OnEndPassOver();

        }
    }

    public void Toss()
    {
        GameStateManager.instance.potionShop.EnablePotionUI();

        DestroyPotionFromInventory();
               
    }

    public void CloseTossMenu()
    {
        if(currentState  == PotionState.menu)
        {
            currentState = PotionState.inventory;

            GameStateManager.instance.potionShop.EnablePotionUI();

            tossConfirmationMenu.SetActive(false);
            OnEndPassOver();
            
        }
    }

    public void DestroyPotionFromInventory()
    {
        GameStateManager.instance.inventory.RemoveUsedPotion(this);
        GameStateManager.instance.player.SetLookingAtInterface(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Situational based on state of potion
    /// </summary>
    public override void OnPassOver()
    {
        
        if (!isHighlighted && UIEnabled && !GameStateManager.instance.levelManager.isGamePaused) {
            //c consider colorblindness for the future
            Color c = Color.green;
            if (currentState == PotionState.shop)
            {
                c = (GameStateManager.instance.playerController.gemsOwned >= cost ) ? Color.green : Color.red;
                if (GameStateManager.instance.inventory.IsInventoryFull()) c = Color.yellow;
            }else if(currentState == PotionState.inventory)
            {
                if(GameStateManager.instance.currentState != EnumManager.GameState.Wave) c = Color.yellow;
                else c = (GameStateManager.instance.player.IsUnderTheInfluence()) ? Color.red : Color.green;
            }

            SetHighlight(c);

            toolTip.gameObject.SetActive(true);
            
            
        }
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        
        if (isHighlighted && currentState != PotionState.menu) {
            RemoveHighlight();
            toolTip.gameObject.SetActive(false);
            
        }
        base.OnEndPassOver();
    }
}
