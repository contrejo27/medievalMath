using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToUpgradeScreen : MonoBehaviour
{
    public GameObject removeHeadsetImage;
    public GameObject subDescription;

    public void LoadSubscriptionPage()
    {
        removeHeadsetImage.SetActive(true);
        subDescription.SetActive(false);
        StartCoroutine("DelayedSendToMainMenu");
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator DelayedSendToMainMenu()
    {
        yield return new WaitForSeconds(3.5f);
        GameStateManager.instance.LoadSceneByName("LevelSelection");
        yield return null;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GameObject.Find("SubscribeMenu").GetComponent<AnimationHelper>().TriggerAnimation("slideIn");
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        Destroy(gameObject);
    }
}
