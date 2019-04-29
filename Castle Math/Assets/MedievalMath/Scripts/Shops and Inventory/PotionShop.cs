using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionShop : MonoBehaviour {
    // To be enabled every 5 waves in waveManager; 
    // This mostly just manages stocking the shop; payment is handled through the
    // potion/playercontroller scripts
    
    public ShopMenu shopMenu;
    public Text totalMoney;
    public Text totalPrice;

    string totalMoneyString = "Total Money: ";
    string totalPriceString = "Total Price: ";
    public Animator potionRays_anim; 
    public Transform[] shopSlots;
    public GameObject scatterShotPotion;
    public GameObject burstFirePotion;
    public GameObject slowTimePotion;
    public GameObject doubleAgentPotion;
    public GameObject freezeTimePotion;
    public GameObject quickShotPotion;
    public GameObject rapidFirePotion;
    public GameObject scarecrowPotion;
    [HideInInspector]
    public List<Potion> potionsInShop = new List<Potion>();

    [HideInInspector]
    public List<Potion> selectedPotions = new List<Potion>();

    [HideInInspector]
    public int numSelectedPotions;



    UIEffects canvasFade;
    int isAwakeCounter = 0;

    void Awake()
    {
        // Moving to Start() for now 
        // GameStateManager.instance.potionShop = this;
        // fill availablePotions with unlocked potions. 
        // (for now we'll do this manually)
    }

    void Start()
    {
        GameStateManager.instance.potionShop = this;
        canvasFade = MathManager.instance.mathCanvas.GetComponent<UIEffects>();
    }

    // May casue issue in the future; see if this happens before Awake()/Start()
    void OnEnable()
    {
        LoadPotions();
        if (isAwakeCounter > 0)
        {
            canvasFade.fadeOut(1);
            //UpdateTotalMoney();
            //UpdateTotalPrice();
        } 
        isAwakeCounter++;
    }

    void LoadPotions()
    {
        // load potions into slots
        // random for now
        // (consider avoiding dupes in the future)
        foreach (Transform t in shopSlots)
        {
            GameObject g = Instantiate(GeneratePotion(), Vector3.zero, Quaternion.identity, t) as GameObject;
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            potionsInShop.Add(g.GetComponent<Potion>());


        }
    }

    GameObject GeneratePotion()
    {
        float temp = Random.Range(0f, 1f);
        if (temp < .15f)
            return slowTimePotion;
        else if (temp < .23f)
            return freezeTimePotion;
        else if (temp < .35f)
            return scarecrowPotion;
        else if (temp < .42f)
            return doubleAgentPotion;
        else if (temp < .63f)
            return burstFirePotion;
        else if (temp < .79f)
            return scatterShotPotion;
        else if (temp < .92f)
            return quickShotPotion;
        else
            return rapidFirePotion;

    }

    void OnDisable()
    {
        foreach(Potion p in potionsInShop)
        {
            Destroy(p.gameObject);
        }
        potionsInShop.Clear();
        selectedPotions.Clear();
        if (isAwakeCounter > 1)
        {
            canvasFade.fadeIn(1);
        }
        isAwakeCounter++;
    }

    public void RemoveFromShop(Potion p)
    {
        potionsInShop.Remove(p);
        if (selectedPotions.Contains(p))
            RemoveSelectedPotion(p);
    }
	
    public void DisablePotionUI(Potion potion)
    {
        foreach(Potion p in potionsInShop)
        {
            if (p != potion)
            {
                p.UIEnabled = false;
            }
        }
    }

    public void EnablePotionUI()
    {
        foreach(Potion p in potionsInShop)
        {
            p.UIEnabled = true;
        }
    }

    IEnumerator DeactivateShop()
    {
        if (!GameStateManager.instance.levelManager.isGamePaused)
        {
            GameStateManager.instance.player.SetLookingAtInterface(false);

            GameStateManager.instance.waveManager.NextWave();
            potionRays_anim.Play("potionRaysShrink");

            //get clip length so we can deactivate right after.
            yield return new WaitForEndOfFrame();
            float animationLength = potionRays_anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);

            gameObject.SetActive(false);
        }
    }

   
    public void AddSelectedPotion(Potion p)
    {
        selectedPotions.Add(p);
        numSelectedPotions++;
        StartCoroutine(DeactivateShop());
        //UpdateTotalPrice();
    }

    public void RemoveSelectedPotion(Potion p)
    {
        selectedPotions.Remove(p);
        numSelectedPotions--;
        //UpdateTotalPrice();
    }

    public void SendToShopMenu()
    {
        if (selectedPotions.Count > 0)
        {
            shopMenu.gameObject.SetActive(true);
            shopMenu.LoadPotions(selectedPotions);
            selectedPotions.Clear();
        }
    }

    public float GetSelectedCost()
    {
        float sum = 0;
        foreach (Potion p in selectedPotions)
            sum += p.cost;

        return sum;
    }

    public void OnPointerEnter()
    {
        GameStateManager.instance.player.SetLookingAtInterface(true);
    }

    public void OnPointerExit()
    {
        GameStateManager.instance.player.SetLookingAtInterface(false);
    }

    public void UpdateTotalMoney()
    {
        //totalMoney.text = totalMoneyString + GameStateManager.instance.levelManager.GetTotalMoney().ToString("0.##");
    }

    public void UpdateTotalPrice()
    {
        totalPrice.text = totalPriceString + GetSelectedCost().ToString("0.##");
    }
}
