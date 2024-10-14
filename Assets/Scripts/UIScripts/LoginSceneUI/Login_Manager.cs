using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login_Manager : MonoBehaviour
{
    Transform[] loginObjectlist;
    [SerializeField] GameObject xrOrigin;
    GameObject cannonObj;
    GameObject iceArrow;

    private void Start()
    {
        loginObjectlist = transform.GetComponentsInChildren<Transform>();
        cannonObj = xrOrigin.GetComponentInChildren<Login_Cannon>().transform.parent.gameObject;
        iceArrow = GetComponentInChildren<Login_IceArrow>().gameObject;
        for (int i = 1; i < loginObjectlist.Length; ++i)
        {
            loginObjectlist[i].gameObject.SetActive(false);
        }
        cannonObj.SetActive(false);
    }

    public void TutorialStart()
    {
        for (int i = 1; i < loginObjectlist.Length; ++i)
        {
            if (i < loginObjectlist.Length)
            {
                loginObjectlist[i].gameObject.SetActive(true);
            }
        }
        iceArrow.GetComponent<Login_IceArrow>().IsOn(true);
        cannonObj.SetActive(true);
    }

    public void TutorialClose()
    {
        iceArrow.GetComponent<Login_IceArrow>().IsOn(false);
        for (int i = 1; i < loginObjectlist.Length; ++i)
        {
            if (i < loginObjectlist.Length)
            {
                loginObjectlist[i].gameObject.SetActive(false);
            }
        }
        cannonObj.SetActive(false);
    }
}
