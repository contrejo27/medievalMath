using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// TODO set up dictionaries for gems

public class LevelManager : MonoBehaviour {

    [Header("UI")]
    public UIEffects mainMenuEffects;
    public UIEffects notificationEffects;
    public GameObject LoseScreen;
    public GameObject MathScreen;
    public GameObject tutorialImage;
    public GameObject target;

    [Header("Enemy Behavior")]
    public GameObject InsidePoint;
    public GameObject StatScreen;
    public Light directionalLight;

    [Header("Environment")]
    public GameObject billboard;
    public DoorHealth fence1, fence2, fence3, fence4;

    [Header("Audio")]
    public AudioClip LostTheCastle;
    public AudioClip[] CastleScreams;
    public AudioSource music;
    public AudioClip gameplaySong;
    public PlayerMathStats playerMathStats;

    //[Header("Game")]
    [HideInInspector]
    public Dictionary<EnumManager.GemType, int> gemsOwned =
        new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Red, 0 },
            {EnumManager.GemType.Yellow, 0 },
            {EnumManager.GemType.Purple, 0 },
            {EnumManager.GemType.Cyan, 0 },
            {EnumManager.GemType.Green, 0 }
        };

    // Use this for initialization
    void Start () {

        GameStateManager.instance.levelManager = this;
	}

    public void StartGame()
    {
        GameStateManager.instance.currentState = EnumManager.GameState.Wave;
        billboard.GetComponent<Animator>().Play("hide");
        Debug.Log("Hiding billboard");
        tutorialImage.SetActive(false);
        target.SetActive(false);
        mainMenuEffects.fadeOut(1.5f);
        notificationEffects.fadeIn(1.5f);
        music.clip = gameplaySong;
        music.loop = true;
        music.Play();
    }

    public void DoLoseGameEffects()
    {
        music.Stop();
        music.clip = LostTheCastle;
        music.loop = false;
        music.Play();

        //set UI
        LoseScreen.SetActive(true);
        MathScreen.SetActive(false);
        StatScreen.SetActive(true);
        DoorHealth[] dh = GameObject.FindObjectsOfType<DoorHealth>();
        for (int i = 0; i < dh.Length; i++)
        {
            dh[i].loseFences();
        }

        //change enemy target so they start running
        EnemyBehavior[] Enemies = GameObject.FindObjectsOfType<EnemyBehavior>();
        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].UpdateTarget(InsidePoint.transform);
            //Enemies [i].Target = InsidePoint;
            //Enemies [i].gameObject.transform.parent = null;
        }
        FadeWorldOut();
    }

    void FadeWorldOut()
    {
        StartCoroutine(FadeSky(0.8f, 0.0f));
        StartCoroutine(FadeLight(false));
    }

    void FadeWorldIn()
    {
        StartCoroutine(FadeSky(0.0f, 0.8f));
        StartCoroutine(FadeLight(true));
    }

    void SetupDictionaries()
    {
        /*
        foreach (EnumManager.GemType upgrade in Enum.GetValues(typeof(EnumManager.Upgrades)))
        {
            // Debug.Log("Upgrade: " + upgrade.ToString() + " index " + Convert.ToInt32(upgrade));
            unlockedUpgrades.Add(upgrade, pd.unlockedAbilities[Convert.ToInt32(upgrade)]);
        }
        */
    }

    public void RecieveGems(int amount, EnumManager.GemType type)
    {
        gemsOwned[type] += amount;
    }

    IEnumerator FadeSky(float initialValue, float endValue)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
        {
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(initialValue, endValue, t));
            yield return null;
        }
    }

    //fades light when you lose
    IEnumerator FadeLight(bool fadeIn)
    {
        Color sunLightColor = directionalLight.color;
        Color ambientLightColor = RenderSettings.ambientSkyColor;
        Color currentColor = RenderSettings.fogColor;

        if (fadeIn)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
            {
                directionalLight.color = Color.Lerp(sunLightColor, new Color(.93f, .87f, .76f), t);
                RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor, new Color(.93f, .87f, .76f), t);
                RenderSettings.fogColor = Color.Lerp(currentColor, new Color(.98f, .918f, .7f), t);
                yield return null;
            }
        }
        else
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
            {
                RenderSettings.fogColor = Color.Lerp(currentColor, new Color(.01f, .02f, .03f), t);
                RenderSettings.ambientSkyColor = Color.Lerp(ambientLightColor, new Color(.05f, .06f, .08f), t);
                directionalLight.color = Color.Lerp(sunLightColor, new Color(.05f, .06f, .08f), t);
                yield return null;
            }
        }
    }

}
