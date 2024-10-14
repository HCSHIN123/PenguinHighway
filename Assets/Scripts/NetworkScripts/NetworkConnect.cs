using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NetworkConnect : SingleTon<NetworkConnect>
{
    public delegate void CallbackMethod();
    public delegate void CallbackMethodwithBool(bool _value);
    private CallbackMethodwithBool callbackMethodSpawnplayer = null;

    private CallbackMethod playerEnterCallback = null;
    private CallbackMethod playerLeftCallback = null;
    public CallbackMethod SetPlayerEnterCallback 
    {
        set { playerEnterCallback = value; }
    }
    public CallbackMethod SetPlayerLeftCallback
    {
        set { playerLeftCallback = value; }
    }
    public CallbackMethodwithBool SetSpawnPlayerCallback 
    {
        set { callbackMethodSpawnplayer = value;}
    }
    public FieldItemManager fieldItemManager;
    private RoomOptions roomOptions = null;
    [SerializeField] private string prevScene = string.Empty;
    [SerializeField] private Tree_Manager treeManager = null;
    public void EnteringRoom()
    {
       if(PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
       else
       {
            PhotonNetwork.GameVersion = "0.0.5";
            PhotonNetwork.NickName = "TestUser";
            PhotonNetwork.ConnectUsingSettings();
       } 

        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.CleanupCacheOnLeave = true;
  
    }
#region  테스트용 포톤 연결

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion



 #region  PhotonNetworkCtrlSync
    public override void OnCreatedRoom()
    {
        Debug.LogError($"Create New Room...");
        base.OnCreatedRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.LogError("Hello World...");
        base.OnJoinedRoom();

        callbackMethodSpawnplayer?.Invoke(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"{newPlayer} has entered room...");
        playerEnterCallback?.Invoke();
      
        base.OnPlayerEnteredRoom(newPlayer);
        fieldItemManager.pv.RPC("InitFieldItemManager", RpcTarget.All);
        treeManager.SetTreeRandom();

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room has Failed.. {returnCode} : {message}");
         //If joinRoom() has Failed, Create new Room
        PhotonNetwork.CreateRoom("testRoom333", roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"Create Room has Failed....{returnCode} : {message}");
        // SceneManager.LoadScene(prevScene);
        PhotonNetwork.JoinOrCreateRoom("OtherTestRoom",roomOptions,TypedLobby.Default);

    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

        Debug.LogError($"{otherPlayer.NickName} has left Room...");
        playerLeftCallback?.Invoke();

    }

    public override void OnLeftRoom()
    {
        Debug.LogError($"Left Room...");
    }
  
    public override void OnDisconnected(DisconnectCause _cause)
    {
        Debug.LogError($"Disconnected from Master Server...: {_cause}");
    }

#endregion
}