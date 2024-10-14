using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DB : MonoBehaviour
{
    string loginUri = "http://127.0.0.1/login.php";
    string SetDataUri = "http://127.0.0.1/SetData.php";
    string CheckDataUri = "http://127.0.0.1/CheckData.php";

    private void Awake()
    {

    }

    void Start()
    {

        /*loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoginCoroutine(id.text, pw.text));
        });


        accountButton.onClick.AddListener(() =>
        {
            StopCoroutine("LoginCoroutine");


            checkaccountButton.onClick.AddListener(() =>
            {
                StartCoroutine(CheckDataCoroutine(cId.text));
            });


            makeaccountButton.onClick.AddListener(() =>
            {
                if (checkPassword(cPw.text))
                {
                    StartCoroutine(SetDataCoroutine(cId.text, cPw.text));
                }
                if (!checkPassword(cPw.text))
                {
                    StartCoroutine(textCoroutine());
                }
            });
        });*/
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", username);
        form.AddField("pw", password);
        

        using (UnityWebRequest www = UnityWebRequest.Post(loginUri, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if ("Wrong password.." == www.downloadHandler.text)
                {

                }
                if ("ID not found.." == www.downloadHandler.text)
                {

                }
                if ("Login success!!" == www.downloadHandler.text)
                {

                }
            }
        }
    }

    private IEnumerator SetDataCoroutine(string _id, string _pass)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("pw", _pass);

        using (UnityWebRequest www = UnityWebRequest.Post(SetDataUri, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }

            else
            {
                if ("Success" == www.downloadHandler.text)
                {
                    
                }

                if ("already id exist" == www.downloadHandler.text)
                {
                    
                }
            }
        }
    }

    private IEnumerator CheckDataCoroutine(string _id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);

        using (UnityWebRequest www = UnityWebRequest.Post(CheckDataUri, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }

            else
            {
                if ("you can" == www.downloadHandler.text)
                {
                    
                }
                if ("already exist" == www.downloadHandler.text)
                {
                    
                }
            }
        }
    }

    private IEnumerator textCoroutine()
    {
        yield return new WaitForSeconds(4f);
    }

    private bool checkPassword(string _pass)
    {
        int cnt = 0;
        int cnt2 = 0;
        int cnt3 = 0;
        foreach (char a in _pass)
        {
            if ('0' <= a && a <= '9')
            {
                cnt += 1;
            }

            if ('A' <= a && a <= 'Z')
            {
                cnt2 += 1;
            }
            if ('a' <= a && a <= 'z')
            {
                cnt3 += 1;
            }
        }
        if (cnt + cnt2 + cnt3 >= 3 && cnt != 0 && cnt2 != 0 && cnt3 != 0)
        {
            return true;
        }
        else return false;
    }

}