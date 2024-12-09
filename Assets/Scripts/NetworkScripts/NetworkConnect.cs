using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NetworkConnect : SingleTon<NetworkConnect>
{
    // �ݹ� �޼��� ���� (�Ķ���Ͱ� ���� ���� bool ���� �޴� ���)
    public delegate void CallbackMethod();
    public delegate void CallbackMethodwithBool(bool _value);

    // �÷��̾� ���� ���� �ݹ� �޼���
    private CallbackMethodwithBool callbackMethodSpawnplayer = null;

    // �÷��̾� ����/���� �ݹ� �޼���
    private CallbackMethod playerEnterCallback = null;
    private CallbackMethod playerLeftCallback = null;

    // �÷��̾� ���� �� �ݹ� ����
    public CallbackMethod SetPlayerEnterCallback
    {
        set { playerEnterCallback = value; }
    }

    // �÷��̾� ���� �� �ݹ� ����
    public CallbackMethod SetPlayerLeftCallback
    {
        set { playerLeftCallback = value; }
    }

    // �÷��̾� ���� �� �ݹ� ����
    public CallbackMethodwithBool SetSpawnPlayerCallback
    {
        set { callbackMethodSpawnplayer = value; }
    }

    // �ʵ� ������ �Ŵ����� �� �ɼ�
    public FieldItemManager fieldItemManager;
    private RoomOptions roomOptions = null;

    // ���� ���� Ʈ�� �Ŵ���
    [SerializeField] private string prevScene = string.Empty;
    [SerializeField] private Tree_Manager treeManager = null;

    // �濡 �����ϴ� �޼���
    public void EnteringRoom()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinRandomRoom();
        else
        {
            // Photon ��Ʈ��ũ �ʱ� ����
            PhotonNetwork.GameVersion = "0.0.5";
            PhotonNetwork.NickName = "TestUser";
            PhotonNetwork.ConnectUsingSettings();
        }

        // �� �ɼ� ����
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.CleanupCacheOnLeave = true;
    }

    #region  �׽�Ʈ�� ���� ����

    // ������ ������ ����Ǿ��� �� ȣ��
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    // �κ� �������� �� ȣ��
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region PhotonNetworkCtrlSync

    // �� ���� �Ϸ� �� ȣ��
    public override void OnCreatedRoom()
    {
        Debug.LogError($"Create New Room...");
        base.OnCreatedRoom();
    }

    // �濡 �������� �� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.LogError("Hello World...");
        base.OnJoinedRoom();

        // ������ Ŭ���̾�Ʈ ���θ� �ݹ� �޼���� ����
        callbackMethodSpawnplayer?.Invoke(PhotonNetwork.IsMasterClient);
    }

    // �ٸ� �÷��̾ �濡 �������� �� ȣ��
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"{newPlayer} has entered room...");
        playerEnterCallback?.Invoke();

        base.OnPlayerEnteredRoom(newPlayer);

        // �ʵ� ������ �Ŵ��� �ʱ�ȭ RPC ȣ��
        fieldItemManager.pv.RPC("InitFieldItemManager", RpcTarget.All);

        // Ʈ�� ��ġ ����ȭ
        treeManager.SetTreeRandom();
    }

    // ���� �� ���� ���� �� ȣ��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room has Failed.. {returnCode} : {message}");
        // �� ���� ���� �� ���ο� �� ����
        PhotonNetwork.CreateRoom("testRoom333", roomOptions);
    }

    // �� ���� ���� �� ȣ��
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"Create Room has Failed....{returnCode} : {message}");

        // ���� �� ���� �� �Ǵ� ���ο� �� ���� �õ�
        PhotonNetwork.JoinOrCreateRoom("OtherTestRoom", roomOptions, TypedLobby.Default);
    }

    // �ٸ� �÷��̾ ���� ������ �� ȣ��
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogError($"{otherPlayer.NickName} has left Room...");
        playerLeftCallback?.Invoke();
    }

    // ���� ������ �� ȣ��
    public override void OnLeftRoom()
    {
        Debug.LogError($"Left Room...");
    }

    // ������ ������ ������ ������ �� ȣ��
    public override void OnDisconnected(DisconnectCause _cause)
    {
        Debug.LogError($"Disconnected from Master Server...: {_cause}");
    }

    #endregion
}
