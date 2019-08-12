using System.Collections;
using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{
    public static UtilityFunctions instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UIPositionLerp(RectTransform rt, float time, Vector3 startPos, Vector3 targetPos, bool isLocal, bool setInactive = false)
    {
        StartCoroutine(UIPositionLerpCoroutine(rt, time, startPos, targetPos, setInactive));
    }

    IEnumerator UIPositionLerpCoroutine(RectTransform rt, float time, Vector3 initPos, Vector3 targetPos, bool setInactive)
    {
        float timer = 0;
        while (timer < time && rt.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            rt.anchoredPosition = Vector3.Lerp(initPos, targetPos, timer / time);
            yield return null;
        }
        if (setInactive)
        {
            rt.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// used to read documents from resource folder
    /// </summary>
    /// <param name="path">path to file in resource folder without extension. example: waves/kells_WaveEasy</param>
    /// <returns>Returns an array of strings that have text divided by newlines</returns>
    public string[] ReadDoc(string path)
    {
        TextAsset Dat = Resources.Load(path, typeof(TextAsset)) as TextAsset;

        //each row is split with a '\n' a.k.a an enter to a new row
        string[] data = Dat.text.Split(new char[] { '\n' });

        return data;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
