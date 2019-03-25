using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn_Out : MonoBehaviour
{
    public Image fadeScreen;
    public Image fadeImage;
    public Image fadeBackground;
    public float fadeRate = 0.05f;
    private float alpha;
    private Color tempColorScreen;
    private Color tempColorImage;

    // Start is called before the first frame update
    void Start()
    {
        tempColorScreen = fadeScreen.color;
        tempColorImage = fadeImage.color;
        StartCoroutine(SplashScreen());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SplashScreen()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeOut());

        fadeImage.transform.gameObject.SetActive(false);
        fadeBackground.transform.gameObject.SetActive(false);

        yield return StartCoroutine(FadeIn());

        fadeScreen.transform.gameObject.SetActive(false);

        yield return null;
    }

    public IEnumerator FadeIn()
    {
        alpha = 1;
        while(fadeScreen.color.a >= 0)
        {
            alpha -= 0.01f;
            tempColorScreen.a = alpha;
            fadeScreen.color = tempColorScreen;
            
            yield return new WaitForSeconds(fadeRate);
        }

        yield return null;
    }

    public IEnumerator FadeOut()
    {
        alpha = 0;
        while (fadeScreen.color.a <= 1)
        {
            alpha += 0.01f;
            tempColorScreen.a = alpha;
            fadeScreen.color = tempColorScreen;

            yield return new WaitForSeconds(fadeRate);
        }

        yield return null;
    }
}
