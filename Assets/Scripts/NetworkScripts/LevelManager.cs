using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingleTon<LevelManager>
{
    [SerializeField]
    private string nextScene = string.Empty;
    


    private void Awake()
    {
        PunConnect.Instance.ConnectServer();
    }    

    public void LoadNextScene()
    {
        if(nextScene != string.Empty) 
        {
            SceneManager.LoadScene(nextScene);
            PunConnect.Instance.ConnectToRandomRoom();
        }           
        else 
            Debug.LogError("NextLevel name has not entered... Please check Inspector ...");
    }



}
