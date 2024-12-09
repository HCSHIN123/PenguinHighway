using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    // �� ���� �迭
    public int[] scores = null;

    // �� �÷��̾� ��
    public int playerCount = 0;

    // FieldItemManager ����
    public FieldItemManager fieldItemManager;

    // ���� �÷��̾� ����
    private PlayerVR localplayer = null;

    // �ʵ� ������ �����հ� ���� ����Ʈ ����
    [SerializeField] private GameObject fielditem = null;
    [SerializeField] private Transform SpawnPointA = null;
    [SerializeField] private Transform SpawnPointB = null;

    // �÷��̾� ���
    [SerializeField] private List<PlayerVR> players = new List<PlayerVR>();

    // ���� �ð�
    [SerializeField] private int gameTime = 0;

    // UIManager�� TreeTwingkle ����
    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private TreeTwingkle treeTwingkle = null;

    private void Awake()
    {
        // �� �� ������ �ʱ�ȭ
        scores = new int[2];

        // ��Ʈ��ũ ���� �� �ݹ� �޼��� ����
        NetworkConnect.Instance.SetSpawnPlayerCallback = SpawnPlayerWithClient;
        NetworkConnect.Instance.SetPlayerEnterCallback = GameStart;
        NetworkConnect.Instance.SetPlayerLeftCallback = PlayerLeftRoom;

        // �濡 ���� ó��
        NetworkConnect.Instance.EnteringRoom();

        // StageManager �ݹ� �޼��� ����
        StageManager.Instance.SetCallbackMethod_Add = RemoteAddScore;

        // �ʵ� �������� ��ġ�� ������ ����
        fielditem.GetComponentInChildren<FieldItem>().SetSpawnPos(StageManager.Instance.GetCenterPosInstage());
        fielditem.GetComponentInChildren<FieldItem>().Respawn();
    }

    // �÷��̾ Ŭ���̾�Ʈ�� �Բ� ����
    private void SpawnPlayerWithClient(bool _isMasterClient)
    {
        if (_isMasterClient)
        {
            // ������ Ŭ���̾�Ʈ�� A ���� ����Ʈ���� �÷��̾ ����
            SpawnPlayer(SpawnPointA);

            foreach (PlayerVR playerVR in players)
            {
                if (playerVR.GetComponent<PhotonView>().IsMine)
                {
                    // �ڽ��� �������� ���, ��ǥ ����
                    playerVR.SetIndicatorTargets(new Transform[2] { fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Bsite) });
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamA;
                }
                else
                {
                    playerVR.SetIndicatorTargets(new Transform[2] { fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Asite) });
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamB;
                }
            }
        }
        else
        {
            // �ٸ� Ŭ���̾�Ʈ�� B ���� ����Ʈ���� �÷��̾ ����
            SpawnPlayer(SpawnPointB);

            foreach (PlayerVR playerVR in players)
            {
                if (playerVR.GetComponent<PhotonView>().IsMine)
                {
                    // �ڽ��� ��ǥ ����
                    playerVR.SetIndicatorTargets(new Transform[2] { fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Asite) });
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamB;
                }
                else
                {
                    playerVR.SetIndicatorTargets(new Transform[2] { fielditem.transform, StageManager.Instance.GetStageGoal(Stage.Stagesite.Bsite) });
                    playerVR.myTeam = PlayerVR.PlayerTeam.TeamA;
                }
            }
        }
    }

    // �������� ���� �߰�
    private void RemoteAddScore(int _value)
    {
        AddTeamScore(_value);
    }

    [PunRPC]
    private void AddTeamScore(int _teamNum)
    {
        // Ư�� ���� �ƴ� �ٸ� ���� ���� ����
        for (int i = 0; i < scores.Length; ++i)
        {
            if (i != _teamNum) ++scores[i];
        }

        // UI ���� ����
        uiManager.SetTeamScores(scores);

        // ���� Ʈ��Ŭ ȿ�� ����
        treeTwingkle.IsTwinkle();
    }

    // ���� ���Ḧ �������� �˸�
    private void RemoteCallbackGameEnd()
    {
        photonView.RPC("GameEnd", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void GameEnd()
    {
        // ���� ���� ó��
        Time.timeScale = 0f;

        // �¸� �� �Ǵ� �� ��� ���
        if (scores[0] > scores[1])
        {
            Debug.Log("�¸��� ���� Team A�Դϴ�!");

            if (localplayer.myTeam == PlayerVR.PlayerTeam.TeamA)
                uiManager.GameSetResult(0); // �¸�
            else
                uiManager.GameSetResult(1); // �й�
        }
        else if (scores[0] < scores[1])
        {
            Debug.Log("�¸��� ���� Team B�Դϴ�!");

            if (localplayer.myTeam == PlayerVR.PlayerTeam.TeamB)
                uiManager.GameSetResult(0); // �¸�
            else
                uiManager.GameSetResult(1); // �й�
        }
        else
        {
            Debug.Log("��Ⱑ ���ºη� �������ϴ�!");
            uiManager.GameSetResult(2); // ���º�
        }

        Debug.Log("���� ����!");
        localplayer.SetXButtonGameEnd = PlayerQuitGame;
    }

    // ���� ���� �˸�
    private void GameStart()
    {
        photonView.RPC("StartGame", RpcTarget.All);
    }

    public void PlayerQuitGame()
    {
        // ���� ���� ó��
        Debug.Log("���� ����");
        Application.Quit();
    }

    [PunRPC]
    private void StartGame()
    {
        // ���� ���� ó��
        Debug.LogError("���� ����");
        StartCoroutine("PlayTimer");
    }

    private void PlayerLeftRoom()
    {
        // �÷��̾ ���� ������ ���� ���� ó��
        RemoteCallbackGameEnd();
    }

    // �÷��̾ Ư�� ���� ��ġ���� ����
    private void SpawnPlayer(Transform _spawnPoint)
    {
        GameObject playerObj = PhotonNetwork.Instantiate("Prefab/Main_Prefab/PenguinPlayer", _spawnPoint.position, _spawnPoint.localRotation);
        PlayerVR player = playerObj.GetComponentInChildren<PlayerVR>();

        if (playerObj.GetPhotonView().IsMine) localplayer = player;

        if (!uiManager) uiManager = playerObj.GetComponentInChildren<UIManager>();
        uiManager.InitPlayerUI();

        players.Add(player);
        player.SetInitTransform(_spawnPoint);
    }

    // ���� Ÿ�̸� �ڷ�ƾ
    private IEnumerator PlayTimer()
    {
        while (gameTime > 0)
        {
            --gameTime;
            uiManager.SetTime(gameTime);

            yield return new WaitForSecondsRealtime(1.0f);
        }

        // Ÿ�̸� ���� �� ���� ����
        yield break;
    }
}
