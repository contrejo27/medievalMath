using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public void ReturnToMainMenu()
    {
        StartCoroutine(DeactivatorVR());
    }

    public void Resume()
    {
        GameStateManager.instance.levelManager.ResumeGame();
    }

    void OnEnable()
    {
        //GetComponent<UIEffects>().fadeIn(3f);
    }

    public void ReturnToWarRoom()
    {
        //get the proper scene and stuff
    }

    public IEnumerator DeactivatorVR()
    {
        SceneManager.LoadScene(0);
        yield return new WaitForSeconds(.5f);
        UnityEngine.XR.XRSettings.enabled = false;
    }
}
