using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Potion : BaseInteractableObject {

    enum PotionState { shop, potionMenu, selected, checkout, inventory }
    PotionState currentState;
    public Text toolTip;
    public Text priceText;
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
        priceText.text = cost.ToString("0.##");
        
        base.Init();
    }
	
	// Update is called once per frame
	void Update () {
		if(currentState == PotionState.potionMenu && GameStateManager.instance.currentState == EnumManager.GameState.Wave)
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

    void EnableTooltip()
    {
        toolTip.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);
    }

    void DisableTooltip()
    {
        toolTip.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
    }

    bool CheckPurchaseViability()
    {
        return cost + GameStateManager.instance.potionShop.GetSelectedCost()
                    <= GameStateManager.instance.levelManager.GetTotalMoney()
                    && GameStateManager.instance.inventory.numPotions
                    + GameStateManager.instance.potionShop.selectedPotions.Count
                    < GameStateManager.instance.inventory.inventorySize;
    }

    public override void OnInteract()
    {
        if (!GameStateManager.instance.levelManager.isGamePaused)
        {
            if (currentState == PotionState.shop && UIEnabled)
            {
                if ( CheckPurchaseViability() )
                {
                    Debug.Log("Purchase Ceck passed");
                    currentState = PotionState.selected;
                    GameStateManager.instance.potionShop.AddSelectedPotion(this);
                    
                }
                else
                {
                    // error tooltip?
                }

            }
            else if(currentState == PotionState.selected)
            {
                currentState = PotionState.shop;
                GameStateManager.instance.potionShop.RemoveSelectedPotion(this);
                //purchaseConfirmationMenu.SetActive(false);
                
            }
            else if (currentState == PotionState.inventory)
            {
                if (!GameStateManager.instance.player.IsUnderTheInfluence() && GameStateManager.instance.currentState == EnumManager.GameState.Wave)
                    DoEffect();
                else if (GameStateManager.instance.currentState != EnumManager.GameState.Wave && UIEnabled)
                {
                    currentState = PotionState.potionMenu;
                    tossConfirmationMenu.SetActive(true);
                    GameStateManager.instance.inventory.DisablePotionUI(this);
                }
            }
            base.OnInteract();
        }
    }

    public void Purchase()
    {
        if (currentState == PotionState.checkout)
        {

            currentState = PotionState.inventory;

            //GameStateManager.instance.playerController.BuyItem((int)cost);

            GameStateManager.instance.inventory.AddPotion(gameObject);

            GameStateManager.instance.potionShop.RemoveFromShop(this);

            GameStateManager.instance.potionShop.EnablePotionUI();

            //purchaseConfirmationMenu.SetActive(false);
            OnEndPassOver();
            
        }
        
    }

    public void ClosePurchaseMenu()
    {
        if (currentState == PotionState.potionMenu)
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
        if(currentState  == PotionState.potionMenu)
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
        
        if (!isHighlighted && UIEnabled && currentState != PotionState.checkout && !GameStateManager.instance.levelManager.isGamePaused) {
            //c consider colorblindness for the future
            Color c = Color.green;
            if (currentState == PotionState.shop)
            {
                c = CheckPurchaseViability() ? Color.green : Color.red;
                if (GameStateManager.instance.inventory.IsInventoryFull()) c = Color.yellow;
            }else if(currentState == PotionState.inventory)
            {
                if(GameStateManager.instance.currentState != EnumManager.GameState.Wave) c = Color.yellow;
                else c = (GameStateManager.instance.player.IsUnderTheInfluence()) ? Color.red : Color.green;
            }

            SetHighlight(c);

            EnableTooltip();
            
            
        }
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        
        if (isHighlighted && currentState != PotionState.selected) {
            RemoveHighlight();
            if(currentState!=PotionState.checkout)
                DisableTooltip();
            
        }
        base.OnEndPassOver();
    }

    public void OnToShopMenu()
    {
        currentState = PotionState.checkout;
        RemoveHighlight();
    }
}
