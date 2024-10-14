using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PunConnect : SingleTon<PunConnect>
{
    [SerializeField] private string gameVersion = string.Empty;

    public int connectingCount = 0;

    public void ConnectServer()
    {
        StartCoroutine("ConnectToServerRoutine");
    }

    public void ConnectToRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private IEnumerator ConnectToServerRoutine()
    {
        if(connectingCount >= 3) yield break;


        Debug.LogError("Connect to Master Server...");
        ConnecttoMasterServer();
        yield return new WaitForEndOfFrame();


        if(!PhotonNetwork.IsConnected)  
        {
            Debug.LogError("Connection failed... Retrying to connect to the server...");
            StartCoroutine("ConnectToServerRoutine");
        } 
       
        yield break;
        
    }

    public void SetNickName(string _id)
    {
        PhotonNetwork.LocalPlayer.NickName = _id;
    }

    public void ConnecttoMasterServer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        connectingCount++;
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Hello....Player {PhotonNetwork.NickName}");      
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.LogError("Joined Lobby...");

        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined Room....");
        base.OnJoinedRoom();
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joinning Room has Failed.... {returnCode} : {message} ");
        Debug.LogError("Please try to Connect Master Server..");
        base.OnJoinRoomFailed(returnCode, message);

        PhotonNetwork.CreateRoom($"{PhotonNetwork.NickName}'s Room", new RoomOptions{MaxPlayers = 2, BroadcastPropsChangeToAll = true, IsOpen = true});
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnect to Server .... {cause}");
        base.OnDisconnected(cause);
    }




}
