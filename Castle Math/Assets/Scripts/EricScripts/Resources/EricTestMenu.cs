using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EricTestMenu : MonoBehaviour 
{
    public GameObject m_loginPopupObject;

    public void LoginButton()
    {
        Debug.Log("Login Button pressed");
        GameObject loginPopup = Instantiate(m_loginPopupObject);
        loginPopup.transform.SetParent(this.transform, false);
    }
	
}
