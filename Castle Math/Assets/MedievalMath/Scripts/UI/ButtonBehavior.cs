using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{

    public GameObject gameLockWarning;
    public void Retry()
    {
        GameStateManager.instance.Retry();
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().name == "kellsLevel")
        {
            if (UserManager.instance.currentActivation == EnumManager.ActivationType.Free)
            {
                gameLockWarning.SetActive(true);
            }
            else
            {
                GameStateManager.instance.LoadSceneByName("frostLevel");
            }

        }
        if (SceneManager.GetActiveScene().name == "frostLevel")
        {
            GameStateManager.instance.LoadSceneByName("bossLevel");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
