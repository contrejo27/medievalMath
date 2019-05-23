using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Potion : BaseInteractableObject {

    enum PotionState { shop, potionMenu, selected, checkout, inventory }
    PotionState currentState;
    public Text toolTip;
    public Text toolTipShadow;
    public GameObject toolTipBG;
    public float cost;
    public float duration;
    float raiseAmt = .2f;
    IEnumerator coroutine;

	public Sprite PotionEffect;
    
    public GameObject purchaseConfirmationMenu;
    public GameObject tossConfirmationMenu;
    public Transform potionMesh;
    Vector3 initPoitionPos;
    
    [HideInInspector]
    public int inventoryPosition;

    // stop tooltips/menus when another potion in the shop is selected
    [HideInInspector]
    public bool UIEnabled = true;

    bool isSelected;

	private GameObject Effect;

	// Use this for initialization

    protected override void Init()
    {
        currentState = PotionState.shop;
        //priceText.text = cost.ToString("0.##");
        initPoitionPos = potionMesh.localPosition;
        
        base.Init();
    }
	
	// Update is called once per frame
	void Update () {
		if(currentState == PotionState.potionMenu && GameStateManager.instance.currentState == EnumManager.GameState.Wave)
        {
            CloseTossMenu();
        }

        if (isHighlighted)
        {
            potionMesh.Rotate(new Vector3(0, 1, 0));
        }

	}

    /// <summary>
	/// Main function of the potion
	/// </summary>
    public virtual void DoEffect()
    {
		Effect = GameObject.Find ("EffectImage");

		print ("Effect: " + Effect);

		Effect.GetComponent<SpriteRenderer> ().sprite = PotionEffect;

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
        toolTipShadow.gameObject.SetActive(true);
        toolTipBG.SetActive(true);
    }

    void DisableTooltip()
    {
        toolTip.gameObject.SetActive(false);
        toolTipShadow.gameObject.SetActive(false);
        toolTipBG.SetActive(false);
    }

    bool CheckPurchaseViability()
    {
        return true;/*
        return cost + GameStateManager.instance.
        .GetSelectedCost()
                    <= GameStateManager.instance.levelManager.GetTotalMoney()
                    && GameStateManager.instance.inventory.numPotions
                    + GameStateManager.instance.potionShop.selectedPotions.Count
                    < GameStateManager.instance.inventory.inventorySize;
    */}

    public override void OnInteract()
    {
        if (!GameStateManager.instance.levelManager.isGamePaused)
        {
            if (currentState == PotionState.shop && UIEnabled)
            {
                if (CheckPurchaseViability())
                {
                    Debug.Log("Purchase Check passed");
                    currentState = PotionState.selected;
                    GameStateManager.instance.potionShop.AddSelectedPotion(this);

                }
                else
                {
                    // error tooltip?
                }

            }
            else if (currentState == PotionState.selected)
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
            Purchase();
            //base.OnInteract();
        }
    }

    public void Purchase()
    {
      //  if (currentState == PotionState.checkout)
       // {

            currentState = PotionState.inventory;

            //GameStateManager.instance.playerController.BuyItem((int)cost);

            GameStateManager.instance.inventory.AddPotion(gameObject);

            GameStateManager.instance.potionShop.RemoveFromShop(this);

            GameStateManager.instance.potionShop.EnablePotionUI();

            //purchaseConfirmationMenu.SetActive(false);
            OnEndPassOver();
            
      //  }
        
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

            //SetHighlight(c);
            StartSelection();

            EnableTooltip();
            
            
        }
        base.OnPassOver();
    }

    public override void OnEndPassOver()
    {
        
        if (isHighlighted && currentState != PotionState.selected) {
            //RemoveHighlight();
            StopSelection();
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

    void StartSelection()
    {
        isHighlighted = true;
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = RaisePotion();
        //potionMesh.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        StartCoroutine(coroutine);
    }

    void StopSelection()
    {
        isHighlighted = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        //potionMesh.localScale = new Vector3(1.8f, 1.8f, 1.8f);

        coroutine = LowerPotion();
        StartCoroutine(coroutine);
    }

    IEnumerator RaisePotion()
    {
       // Debug.Log("INSIDE the coroutine");
        float timer = 0;
        float initHeight = potionMesh.localPosition.y;
        while (timer < .2f)
        {
            timer += Time.deltaTime;
            potionMesh.localPosition = new Vector3(potionMesh.localPosition.x, Mathf.SmoothStep(initHeight, initPoitionPos.y+raiseAmt, timer / .2f), potionMesh.localPosition.z);
            yield return null;
        }
    }
    
    IEnumerator LowerPotion()
    {
        float timer = 0;
        float initHeight = potionMesh.localPosition.y;
        Quaternion initialRot = potionMesh.rotation;
        Quaternion targetRotation = Quaternion.identity;
        while (timer < .2f)
        {
            timer += Time.deltaTime;
            potionMesh.localPosition = new Vector3(potionMesh.localPosition.x, Mathf.SmoothStep(initHeight, initPoitionPos.y, timer / .2f), potionMesh.localPosition.z);
            Quaternion.Slerp(initialRot, targetRotation, timer / .2f);
            yield return null;
        }
        potionMesh.localPosition = new Vector3(potionMesh.localPosition.x, initPoitionPos.y, potionMesh.localPosition.z);
        potionMesh.rotation = targetRotation;
    }
}
