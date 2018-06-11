using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEntry : MonoBehaviour 
{
    public InputField InputField { get { return inputField; } }

    private InputField inputField;

	private void Awake()
	{
        inputField = transform.GetChild(0).GetComponent<InputField>();
	}

	private void Start()
	{
		if(inputField)
            inputField.text = "";
        
	}
}
