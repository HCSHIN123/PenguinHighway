using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NetworkConnect : SingleTon<NetworkConnect>
{
    // 콜백 메서드 정의 (파라미터가 없는 경우와 bool 값을 받는 경우)
    public delegate void CallbackMethod();
    public delegate void CallbackMethodwithBool(bool _value);

    // 플레이어 생성 관련 콜백 메서드
    private CallbackMethodwithBool callbackMethodSpawnplayer = null;

    // 플레이어 입장/퇴장 콜백 메서드
    private CallbackMethod playerEnterCallback = null;
    private CallbackMethod playerLeftCallback = null;

    // 플레이어 입장 시 콜백 설정
    public CallbackMethod SetPlayerEnterCallback
    {
        set { playerEnterCallback = value; }
    }

    // 플레이어 퇴장 시 콜백 설정
    public CallbackMethod SetPlayerLeftCallback
    {
        set { playerLeftCallback = value; }
    }

    // 플레이어 생성 시 콜백 설정
    public CallbackMethodwithBool SetSpawnPlayerCallback
    {
        set { callbackMethodSpawnplayer = value; }
    }

    // 필드 아이템 매니저와 방 옵션
    public FieldItemManager fieldItemManager;
    private RoomOptions roomOptions = null;

    // 이전 씬과 트리 매니저
    [SerializeField] private string prevScene = string.Empty;
    [SerializeField] private Tree_Manager treeManager = null;

    // 방에 입장하는 메서드
    public void EnteringRoom()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
        else
        {
            // Photon 네트워크 초기 설정
            PhotonNetwork.GameVersion = "0.0.5";
            PhotonNetwork.NickName = "TestUser";
            PhotonNetwork.ConnectUsingSettings();
        }

        // 방 옵션 설정
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.CleanupCacheOnLeave = true;
    }

    #region  테스트용 포톤 연결

    // 마스터 서버에 연결되었을 때 호출
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    // 로비에 입장했을 때 호출
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region PhotonNetworkCtrlSync

    // 방 생성 완료 시 호출
    public override void OnCreatedRoom()
    {
        Debug.LogError($"Create New Room...");
        base.OnCreatedRoom();
    }

    // 방에 입장했을 때 호출
    public override void OnJoinedRoom()
    {
        Debug.LogError("Hello World...");
        base.OnJoinedRoom();

        // 마스터 클라이언트 여부를 콜백 메서드로 전달
        callbackMethodSpawnplayer?.Invoke(PhotonNetwork.IsMasterClient);
    }

    // 다른 플레이어가 방에 입장했을 때 호출
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"{newPlayer} has entered room...");
        playerEnterCallback?.Invoke();

        base.OnPlayerEnteredRoom(newPlayer);

        // 필드 아이템 매니저 초기화 RPC 호출
        fieldItemManager.pv.RPC("InitFieldItemManager", RpcTarget.All);

        // 트리 위치 랜덤화
        treeManager.SetTreeRandom();
    }

    // 랜덤 방 입장 실패 시 호출
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room has Failed.. {returnCode} : {message}");
        // 방 입장 실패 시 새로운 방 생성
        PhotonNetwork.CreateRoom("testRoom333", roomOptions);
    }

    // 방 생성 실패 시 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"Create Room has Failed....{returnCode} : {message}");

        // 실패 시 기존 방 또는 새로운 방 생성 시도
        PhotonNetwork.JoinOrCreateRoom("OtherTestRoom", roomOptions, TypedLobby.Default);
    }

    // 다른 플레이어가 방을 떠났을 때 호출
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogError($"{otherPlayer.NickName} has left Room...");
        playerLeftCallback?.Invoke();
    }

    // 방을 떠났을 때 호출
    public override void OnLeftRoom()
    {
        Debug.LogError($"Left Room...");
    }

    // 마스터 서버와 연결이 끊겼을 때 호출
    public override void OnDisconnected(DisconnectCause _cause)
    {
        Debug.LogError($"Disconnected from Master Server...: {_cause}");
    }

    #endregion
}
