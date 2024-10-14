using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class GameManager : SingleTon<GameManager>
{       
    public int[] scores = null;
    public int playerCount = 0;
    public FieldItemManager fieldItemManager;
    private PlayerVR localplayer = null;

    [SerializeField] private GameObject fielditem = null;
    [SerializeField] private Transform SpawnPointA = null;
    [SerializeField] private Transform SpawnPointB = null;
    [SerializeField] private List<PlayerVR> players = new List<PlayerVR>(); 
    [SerializeField] private int gameTime = 0;
    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private TreeTwingkle treeTwingkle = null;

    private void Awake()
    {
        scores = new int[2];

        NetworkConnect.Instance.SetSpawnPlayerCallback = SpawnPlayerWithClient;
        NetworkConnect.Instance.SetPlayerEnterCallback = GameStart;
        NetworkConnect.Instance.SetPlayerLeftCallback = PlayerLeftRoom;

        NetworkConnect.Instance.EnteringRoom();

        StageManager.Instance.SetCallbackMethod_Add = RemoteAddScore;
        fielditem.GetComponentInChildren<FieldItem>().SetSpawnPos(StageManager.Instance.GetCenterPosInstage());
        fielditem.GetComponentInChildren<FieldItem>().Respawn();
        
    }


    private void SpawnPlayerWithClient(bool _isMasterClient)
    {   
        if(_isMasterClient) 
        {
            SpawnPlayer(SpawnPointA);

            foreach(PlayerVR playerVR in players)
            {
               if(playerVR.GetComponent<PhotonView>().IsMine) 
               {
                    playerVR.SetIndicatorTargets(new Transform[2]{fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Bsite)});
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamA;
               }else
               {
                    playerVR.SetIndicatorTargets(new Transform[2]{fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Asite)});
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamB;
               }
               
            }
        }      
        else 
        {
            SpawnPlayer(SpawnPointB);
            foreach(PlayerVR playerVR in players)
            {
               if(playerVR.GetComponent<PhotonView>().IsMine) 
               {
                    playerVR.SetIndicatorTargets(new Transform[2]{fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Asite)});
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamB;
               }else
               {
                    playerVR.SetIndicatorTargets(new Transform[2]{fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Bsite)});
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamA;
               }
               
            }
        }
    }


    private void RemoteAddScore(int _value)
    {
        AddTeamScore(_value);
    }

    [PunRPC]
    private void AddTeamScore(int _teamNum)
    {
        for(int i= 0; i < scores.Length; ++i)
        {
            if(i != _teamNum) ++scores[i];
        }
        uiManager.SetTeamScores(scores);
        treeTwingkle.IsTwinkle();
    }
    private void RemoteCallbackGameEnd()  
    {
        photonView.RPC("GameEnd",RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void GameEnd()
    {

        Time.timeScale = 0f;
        if(scores[0] > scores[1])
        {
            Debug.Log("The winner is Team A!!");

            if(localplayer.myTeam == PlayerVR.PlayerTeam.TeamA)
               uiManager.GameSetResult(0);
            else
                uiManager.GameSetResult(1);

        }else if(scores[0] < scores[1])
        {
            Debug.Log("The winner is Team B!!");

            if(localplayer.myTeam == PlayerVR.PlayerTeam.TeamB)
                uiManager.GameSetResult(0);
            else
                uiManager.GameSetResult(1);

        }else
        {
            Debug.Log("This match is Draw!!");
            uiManager.GameSetResult(2);
        }
        Debug.Log("Game End!!!");
        localplayer.SetXButtonGameEnd = PlayerQuitGame;
    }
    
    
    private void GameStart()
    {
       
        photonView.RPC("StartGame",RpcTarget.All);
        
    }

    public void PlayerQuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    [PunRPC]
    private void StartGame()
    {
        Debug.LogError("Start Game");
        StartCoroutine("PlayTimer");
       
    }

    private void PlayerLeftRoom()
    {
        RemoteCallbackGameEnd();
    }

    private void SpawnPlayer(Transform _spawnPoint)
    {      
        GameObject playerObj = PhotonNetwork.Instantiate("Prefab/Main_Prefab/PenguinPlayer", _spawnPoint.position, _spawnPoint.localRotation);
        PlayerVR player = playerObj.GetComponentInChildren<PlayerVR>();
        if(playerObj.GetPhotonView().IsMine) localplayer = player;
    
        if(!uiManager) uiManager = playerObj.GetComponentInChildren<UIManager>();
        uiManager.InitPlayerUI();

        players.Add(player);
        player.SetInitTransform(_spawnPoint);
       
    }

    private IEnumerator PlayTimer()
    {

        while(gameTime > 0)
        {
            --gameTime;
            uiManager.SetTime(gameTime);

            yield return new WaitForSecondsRealtime(1.0f);

        }

        //RemoteCallbackGameEnd();
        yield break;
        
    }

   
    
}
