using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PunConnect : SingleTon<PunConnect>
{
    // ���� ���� ����
    [SerializeField] private string gameVersion = string.Empty;

    // �����ڼ�
    public int connectingCount = 0;

    // ������ ���� ����
    public void ConnectServer()
    {
        StartCoroutine("ConnectToServerRoutine");
    }

   
    public void ConnectToRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // ���� ������ �õ��ϴ� ��ƾ
    private IEnumerator ConnectToServerRoutine()
    {
        // �ο��� �ʰ� �� ����
        if (connectingCount >= 3) yield break;

        Debug.LogError("Connect to Master Server...");
        ConnecttoMasterServer();
        yield return new WaitForEndOfFrame();

        // ���� ���� �� ��õ�
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Connection failed... Retrying to connect to the server...");
            StartCoroutine("ConnectToServerRoutine");
        }

        yield break;
    }

    // �г��� ����
    public void SetNickName(string _id)
    {
        PhotonNetwork.LocalPlayer.NickName = _id;
    }

    // ������ ������ ����
    public void ConnecttoMasterServer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        connectingCount++;
    }

    // ������ ���� ���� ���� �� ȣ��
    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Hello....Player {PhotonNetwork.NickName}");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� ���� �� ȣ��
    public override void OnJoinedLobby()
    {
        Debug.LogError("Joined Lobby...");
        base.OnJoinedLobby();
    }

    // �� ���� ���� �� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room...");
        base.OnJoinedRoom();
    }

    // �� ���� ���� �� ȣ��
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining Room has Failed.... {returnCode} : {message}");
        Debug.LogError("Please try to Connect Master Server...");
        base.OnJoinRoomFailed(returnCode, message);

        // ���ο� �� ���� �õ�
        PhotonNetwork.CreateRoom($"{PhotonNetwork.NickName}'s Room", new RoomOptions
        {
            MaxPlayers = 2,
            BroadcastPropsChangeToAll = true,
            IsOpen = true
        });
    }

    // ���� ���� ���� �� ȣ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Server .... {cause}");
        base.OnDisconnected(cause);
    }
}
