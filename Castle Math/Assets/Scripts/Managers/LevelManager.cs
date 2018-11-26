using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


// Mostly contains functions formerly in the GameStateManager
// This is specifically for the levels rather than the full game

public class LevelManager : MonoBehaviour {

    [Header("UI")]
    public UIEffects mainMenuEffects;
    public UIEffects notificationEffects;
    public GameObject LoseScreen;
    public GameObject MathScreen;
    public GameObject tutorialImage;
    public GameObject target;
    public GameObject PauseMenu;
    public Button medium;
    public Button nextLevel;
    public GemUIEffect mediumGemEffect;
    public GemUIEffect nLevelGemEffect;

    [Header("Enemy Behavior")]
    public GameObject InsidePoint;
    public GameObject StatScreen;
    public Light directionalLight;
    public List<EnemyBehavior> activeEnemies = new List<EnemyBehavior>();
    public Transform scarecrowSpawnPoint;
    public GameObject dummyPrefab;
    public GameObject explosiveDummyPrefab;

    [Header("Environment")]
    public GameObject billboard;
    public DoorHealth fence1, fence2, fence3, fence4;
    [HideInInspector]
    public bool isGamePaused;

    [Header("Audio")]
    public AudioClip LostTheCastle;
    public AudioClip[] CastleScreams;
    public AudioSource music;
    public AudioClip gameplaySong;
    public PlayerMathStats playerMathStats;

    [HideInInspector]
    public WaveManager waveManager;
    public GameObject pauseButton;
    //[Header("Game")]
    [HideInInspector]
    public Dictionary<EnumManager.GemType, int> gemsOwned =
        new Dictionary<EnumManager.GemType, int>()
        {
            {EnumManager.GemType.Penny, 0 },
            {EnumManager.GemType.Nickel, 0 },
            {EnumManager.GemType.Dime, 0 },
            {EnumManager.GemType.Quarter, 0 },
            {EnumManager.GemType.Dollar, 0 }
        };

    // Use this for initialization
    void Start () {

        GameStateManager.instance.levelManager = this;
        RenderSettings.skybox.SetFloat("_Exposure", .8f);
    }

	private GameObject Effect;

    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                gemsOwned[EnumManager.GemType.Dollar]++;
                GameStateManager.instance.potionShop.UpdateTotalMoney();
            }
        }
    }

    public void StartGame()
    {
        if (!isGamePaused)
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
            pauseButton.SetActive(true);
			//FadeWorldIn ();
        }
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

    public void unlockNextGameMode()
    {
        if (GameStateManager.instance.currentDifficulty == EnumManager.GameplayMode.Easy)
        {
            medium.interactable = true;
            mediumGemEffect.unlockGem();
        }
        if (GameStateManager.instance.currentDifficulty == EnumManager.GameplayMode.Medium)
        {
            nextLevel.interactable = true;
            nLevelGemEffect.unlockGem();
        }
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

    public void SlowAllEnemeies(float slowScale, float duration)
    {
        foreach(EnemyBehavior eb in activeEnemies)
        {
            eb.SlowsEnemy(slowScale, duration);
        }
    }

    public void RecieveGems(int amount, EnumManager.GemType type)
    {
        gemsOwned[type] += amount;
    }

    public void RemoveGems(int amount, EnumManager.GemType type)
    {
        gemsOwned[type] -= amount;
    }

    public void SetDummyEnemies(float duration, bool doesExplode = false)
    {
        StartCoroutine(PullEnemies(scarecrowSpawnPoint, duration, doesExplode));
    }

    public void PauseGame()
    {
        //billboard.SetActive(true);
        //billboard.GetComponent<Animator>().Play("show");
        PauseMenu.SetActive(true);
        MathScreen.SetActive(false);
        Time.timeScale = 0;
        
        isGamePaused = true;
        //StartCoroutine(FadeSky(.8f, .4f));
        foreach (EnemyBehavior eb in activeEnemies)
            eb.PauseEnemy();
    }

    public void ResumeGame()
    {
        //billboard.GetComponent<Animator>().Play("hide");
        //PauseMenu.GetComponent<UIEffects>().fadeOut(3f);
        PauseMenu.SetActive(false);
        MathScreen.SetActive(true);

        isGamePaused = false;
        Time.timeScale = 1;
        //StartCoroutine(FadeSky(.4f, .8f));
        foreach (EnemyBehavior eb in activeEnemies)
            eb.ResumeEnemy();
    }

    public float GetTotalMoney()
    {
        return gemsOwned[EnumManager.GemType.Penny] * EnumManager.gemValues[EnumManager.GemType.Penny]
            + gemsOwned[EnumManager.GemType.Nickel] * EnumManager.gemValues[EnumManager.GemType.Nickel]
            + gemsOwned[EnumManager.GemType.Dime] * EnumManager.gemValues[EnumManager.GemType.Dime]
            + gemsOwned[EnumManager.GemType.Quarter] * EnumManager.gemValues[EnumManager.GemType.Quarter]
            + gemsOwned[EnumManager.GemType.Dollar] * EnumManager.gemValues[EnumManager.GemType.Dollar];
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

    IEnumerator PullEnemies(Transform target, float duration, bool doesExplode)
    {
        GameObject dummy =
            (doesExplode) ? Instantiate(explosiveDummyPrefab, target.position, Quaternion.identity, target) as GameObject
            : Instantiate(dummyPrefab, target.position, Quaternion.identity, target) as GameObject;

        foreach (EnemyBehavior eb in activeEnemies)
        {
            eb.UpdateTarget(target, true);
        }

        yield return new WaitForSeconds(duration);

        foreach(EnemyBehavior eb in activeEnemies)
        {
            eb.UpdateTarget(eb.fenceTarget);
        }

		Effect = GameObject.Find ("EffectImage");

		Effect.GetComponent<SpriteRenderer> ().sprite = null;

        // do extra stuff if it explodes
        dummy.GetComponent<DummyScript>().DoOnDestroy();
    }

}
