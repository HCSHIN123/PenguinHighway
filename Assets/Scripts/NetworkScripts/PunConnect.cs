using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PunConnect : SingleTon<PunConnect>
{
    // 게임 버전 정보
    [SerializeField] private string gameVersion = string.Empty;

    // 접속자수
    public int connectingCount = 0;

    // 서버에 연결 시작
    public void ConnectServer()
    {
        StartCoroutine("ConnectToServerRoutine");
    }

   
    public void ConnectToRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 서버 연결을 시도하는 루틴
    private IEnumerator ConnectToServerRoutine()
    {
        // 인원수 초과 시 리턴
        if (connectingCount >= 3) yield break;

        Debug.LogError("Connect to Master Server...");
        ConnecttoMasterServer();
        yield return new WaitForEndOfFrame();

        // 연결 실패 시 재시도
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Connection failed... Retrying to connect to the server...");
            StartCoroutine("ConnectToServerRoutine");
        }

        yield break;
    }

    // 닉네임 설정
    public void SetNickName(string _id)
    {
        PhotonNetwork.LocalPlayer.NickName = _id;
    }

    // 마스터 서버에 연결
    public void ConnecttoMasterServer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        connectingCount++;
    }

    // 마스터 서버 연결 성공 시 호출
    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Hello....Player {PhotonNetwork.NickName}");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장 성공 시 호출
    public override void OnJoinedLobby()
    {
        Debug.LogError("Joined Lobby...");
        base.OnJoinedLobby();
    }

    // 방 입장 성공 시 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room...");
        base.OnJoinedRoom();
    }

    // 방 입장 실패 시 호출
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining Room has Failed.... {returnCode} : {message}");
        Debug.LogError("Please try to Connect Master Server...");
        base.OnJoinRoomFailed(returnCode, message);

        // 새로운 방 생성 시도
        PhotonNetwork.CreateRoom($"{PhotonNetwork.NickName}'s Room", new RoomOptions
        {
            MaxPlayers = 2,
            BroadcastPropsChangeToAll = true,
            IsOpen = true
        });
    }

    // 서버 연결 해제 시 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Server .... {cause}");
        base.OnDisconnected(cause);
    }
}
