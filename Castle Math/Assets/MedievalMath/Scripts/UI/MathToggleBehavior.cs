using UnityEngine;
using UnityEngine.UI;

public class MathToggleBehavior : MonoBehaviour
{
    Toggle toggle;
    public EnumManager.QuestionCategories questionCategory;
    public GameObject mathLocked;

    void Start()
    {
        toggle = GetComponent<UnityEngine.UI.Toggle>();
        //check initially if math is enabled
        UpdateToggle(toggle.isOn);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }


    //updates button and if math is enabled
    private void OnToggleValueChanged(bool isOn)
    {
        UpdateToggle(isOn);
    }

    //Changes toggle color and changes in mathController
    void UpdateToggle(bool isOn)
    {
        Color offColor = new Color(1, 0.7216981f, 0.7216981f);
        ColorBlock cb = toggle.colors;

        //update if this type of math is enabled 
        MathController.instance.selectMathType(questionCategory).isEnabled = toggle.isOn;

        if (isOn)
        {
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
        }
        else
        {
            cb.normalColor = offColor;
            cb.highlightedColor = offColor;
        }
        toggle.colors = cb;
    }
}
