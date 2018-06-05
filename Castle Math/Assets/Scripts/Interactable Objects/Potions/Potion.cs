using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Potion : BaseInteractableObject {

    enum PotionState { shop, menu, inventory }
    PotionState currentState;
    public Text toolTip;
    public int cost;
    public float duration;
    public Material outlineMaterial;

    public GameObject confirmationMenu;
    

    [HideInInspector]
    public bool isHighlighted = false;

    [HideInInspector]
    public int inventoryPosition;

    // stop tooltips/menus when another potion in the shop is selected
    [HideInInspector]
    public bool UIEnabled = true;

    private MaterialPropertyBlock mpb;

    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    List<Material[]> materials = new List<Material[]>();

	// Use this for initialization

    protected override void Init()
    {
        currentState = PotionState.shop;
        mpb = new MaterialPropertyBlock();
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderers.Add(mr);
            materials.Add(mr.materials);

        }
        base.Init();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
	/// Main function of the potion
	/// </summary>
    public virtual void DoEffect()
    {

        Debug.Log("Doing effect!");
        GameStateManager.instance.inventory.RemoveUsedPotion(inventoryPosition);
        GameStateManager.instance.player.SetLookingAtInterface(false);
        Destroy(gameObject);
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {
        
    }

    public override void OnInteract()
    {
        Debug.Log("Current state: " + currentState);
        if(currentState == PotionState.shop && UIEnabled)
        {
            if (cost <= GameStateManager.instance.playerController.gemsOwned
            && !GameStateManager.instance.inventory.IsInventoryFull()
            )
            {
                currentState = PotionState.menu;
                confirmationMenu.SetActive(true);
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
            
        } else if(currentState == PotionState.inventory)
        {
            if(!GameStateManager.instance.player.IsUnderTheInfluence())
                DoEffect();
            /* Destroy object
             * Update inventory with relevant info
             * (This si in the doeffect function now)
             */ 
        }
        base.OnInteract();
    }

    public void Purchase()
    {
        if (currentState == PotionState.menu)
        {

            currentState = PotionState.inventory;

            GameStateManager.instance.playerController.BuyItem(cost);

            GameStateManager.instance.inventory.AddPotion(gameObject);

            GameStateManager.instance.potionShop.RemoveFromShop(this);

            GameStateManager.instance.potionShop.EnablePotionUI();

            confirmationMenu.SetActive(false);
            OnEndPassOver();
            
        }
        
    }

    public void CloseMenu()
    {
        if (currentState == PotionState.menu)
        {

            currentState = PotionState.shop;

            GameStateManager.instance.potionShop.EnablePotionUI();

            confirmationMenu.SetActive(false);
            OnEndPassOver();

        }
    }

    /// <summary>
    /// Situational based on state of potion
    /// </summary>
    public override void OnPassOver()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
        if (!isHighlighted && UIEnabled) {
            //c consider colorblindness for the future
            Color c = Color.green;
            if (currentState == PotionState.shop)
            {
                c = (GameStateManager.instance.playerController.gemsOwned >= cost ) ? Color.green : Color.red;
                if (GameStateManager.instance.inventory.IsInventoryFull()) c = Color.yellow;
            }else if(currentState == PotionState.inventory)
            {
                c = (GameStateManager.instance.player.IsUnderTheInfluence()) ? Color.yellow : Color.green;
            }
            foreach (MeshRenderer mr in meshRenderers)
            {
                Material[] mBackup = new Material[mr.materials.Length];
                Material[] ms = new Material[mr.materials.Length];
                for(int i = 0; i<mr.materials.Length; i++)
                {
                    mBackup[i] = mr.materials[i];
                    ms[i] = outlineMaterial;
                }
                mr.materials = ms;
                for(int i = 0; i<mr.materials.Length; i++)
                {
                    //mr.GetPropertyBlock(mpb);
                    mr.materials[i].SetColor("_Color", mBackup[i].color);
                    mr.materials[i].SetColor("_OutlineColor", c);
                    //mpb.SetColor("_Color", mBackup[i].color);
                    //mpb.SetColor("_OutlineColor", c);
                    if (mBackup[i].mainTexture != null)
                    {
                        //mpb.SetTexture("_MainTex", mBackup[i].mainTexture);
                        mr.materials[i].SetTexture("_MainTex", mBackup[i].mainTexture);
                    }
                    //mr.SetPropertyBlock(mpb);
                    /*
                    Debug.Log("Set material to outline: " + mr.materials[i].name +", from: " + mBackup[i].name);
                    Debug.Log("Set Color " + outlineMaterial.color.ToString() + " to " + mBackup[i].color.ToString() + "(" + mr.materials[i].color + ")");
                    Debug.Log("Boop");
                    */
                }
            }
            toolTip.gameObject.SetActive(true);
            isHighlighted = true;
            
        }
    }

    public override void OnEndPassOver()
    {
        GameStateManager.instance.player.SetLookingAtInterface(false);
        if (isHighlighted && currentState != PotionState.menu) {
            int i = 0;
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.materials = materials[i];
                i++;
                Debug.Log("Removed outline");
            }
            toolTip.gameObject.SetActive(false);
            isHighlighted = false;
        }
    }
}
