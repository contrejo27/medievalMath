using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    //UI
    public UIEffects mainMenuEffects;
    public UIEffects notificationEffects;
    public GameObject LoseScreen;
    public GameObject MathScreen;
    public GameObject tutorialImage;
    public GameObject target;

    //enemy behavior
    public GameObject InsidePoint;

    public GameObject StatScreen;

    public Light directionalLight;

    //Environment
    public doorHealth fence1, fence2, fence3;
    public GameObject billboard;

    //audio
    public AudioClip LostTheCastle;
    public AudioClip[] CastleScreams;
    public AudioSource music;
    public AudioClip gameplaySong;
    public PlayerMathStats playerMathStats;

    // Use this for initialization
    void Start () {
        GameStateManager.instance.levelManager = this;
	}

    public void StartGame()
    {
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
        doorHealth[] dh = GameObject.FindObjectsOfType<doorHealth>();
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
        StartCoroutine(fadeSky(0.8f, 0.0f));
        StartCoroutine(fadeLight(false));
    }

    void FadeWorldIn()
    {
        StartCoroutine(fadeSky(0.0f, 0.8f));
        StartCoroutine(fadeLight(true));
    }

    IEnumerator fadeSky(float initialValue, float endValue)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 3f)
        {
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(initialValue, endValue, t));
            yield return null;
        }
    }

    //fades light when you lose
    IEnumerator fadeLight(bool fadeIn)
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
