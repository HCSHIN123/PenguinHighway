using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    // 팀 점수 배열
    public int[] scores = null;

    // 총 플레이어 수
    public int playerCount = 0;

    // FieldItemManager 참조
    public FieldItemManager fieldItemManager;

    // 로컬 플레이어 참조
    private PlayerVR localplayer = null;

    // 필드 아이템 프리팹과 스폰 포인트 참조
    [SerializeField] private GameObject fielditem = null;
    [SerializeField] private Transform SpawnPointA = null;
    [SerializeField] private Transform SpawnPointB = null;

    // 플레이어 목록
    [SerializeField] private List<PlayerVR> players = new List<PlayerVR>();

    // 게임 시간
    [SerializeField] private int gameTime = 0;

    // UIManager와 TreeTwingkle 참조
    [SerializeField] private UIManager uiManager = null;
    [SerializeField] private TreeTwingkle treeTwingkle = null;

    private void Awake()
    {
        // 각 팀 점수를 초기화
        scores = new int[2];

        // 네트워크 연결 시 콜백 메서드 설정
        NetworkConnect.Instance.SetSpawnPlayerCallback = SpawnPlayerWithClient;
        NetworkConnect.Instance.SetPlayerEnterCallback = GameStart;
        NetworkConnect.Instance.SetPlayerLeftCallback = PlayerLeftRoom;

        // 방에 입장 처리
        NetworkConnect.Instance.EnteringRoom();

        // StageManager 콜백 메서드 설정
        StageManager.Instance.SetCallbackMethod_Add = RemoteAddScore;

        // 필드 아이템의 위치와 리스폰 설정
        fielditem.GetComponentInChildren<FieldItem>().SetSpawnPos(StageManager.Instance.GetCenterPosInstage());
        fielditem.GetComponentInChildren<FieldItem>().Respawn();
    }

    // 플레이어를 클라이언트와 함께 스폰
    private void SpawnPlayerWithClient(bool _isMasterClient)
    {
        if (_isMasterClient)
        {
            // 마스터 클라이언트는 A 스폰 포인트에서 플레이어를 스폰
            SpawnPlayer(SpawnPointA);

            foreach (PlayerVR playerVR in players)
            {
                if (playerVR.GetComponent<PhotonView>().IsMine)
                {
                    // 자신이 마스터일 경우, 목표 설정
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
            // 다른 클라이언트는 B 스폰 포인트에서 플레이어를 스폰
            SpawnPlayer(SpawnPointB);

            foreach (PlayerVR playerVR in players)
            {
                if (playerVR.GetComponent<PhotonView>().IsMine)
                {
                    // 자신의 목표 설정
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

    // 원격으로 점수 추가
    private void RemoteAddScore(int _value)
    {
        AddTeamScore(_value);
    }

    [PunRPC]
    private void AddTeamScore(int _teamNum)
    {
        // 특정 팀이 아닌 다른 팀의 점수 증가
        for (int i = 0; i < scores.Length; ++i)
        {
            if (i != _teamNum) ++scores[i];
        }

        // UI 점수 갱신
        uiManager.SetTeamScores(scores);

        // 나무 트윙클 효과 실행
        treeTwingkle.IsTwinkle();
    }

    // 게임 종료를 원격으로 알림
    private void RemoteCallbackGameEnd()
    {
        photonView.RPC("GameEnd", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void GameEnd()
    {
        // 게임 종료 처리
        Time.timeScale = 0f;

        // 승리 팀 판단 및 결과 출력
        if (scores[0] > scores[1])
        {
            Debug.Log("승리한 팀은 Team A입니다!");

            if (localplayer.myTeam == PlayerVR.PlayerTeam.TeamA)
                uiManager.GameSetResult(0); // 승리
            else
                uiManager.GameSetResult(1); // 패배
        }
        else if (scores[0] < scores[1])
        {
            Debug.Log("승리한 팀은 Team B입니다!");

            if (localplayer.myTeam == PlayerVR.PlayerTeam.TeamB)
                uiManager.GameSetResult(0); // 승리
            else
                uiManager.GameSetResult(1); // 패배
        }
        else
        {
            Debug.Log("경기가 무승부로 끝났습니다!");
            uiManager.GameSetResult(2); // 무승부
        }

        Debug.Log("게임 종료!");
        localplayer.SetXButtonGameEnd = PlayerQuitGame;
    }

    // 게임 시작 알림
    private void GameStart()
    {
        photonView.RPC("StartGame", RpcTarget.All);
    }

    public void PlayerQuitGame()
    {
        // 게임 종료 처리
        Debug.Log("게임 종료");
        Application.Quit();
    }

    [PunRPC]
    private void StartGame()
    {
        // 게임 시작 처리
        Debug.LogError("게임 시작");
        StartCoroutine("PlayTimer");
    }

    private void PlayerLeftRoom()
    {
        // 플레이어가 방을 떠나면 게임 종료 처리
        RemoteCallbackGameEnd();
    }

    // 플레이어를 특정 스폰 위치에서 스폰
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

    // 게임 타이머 코루틴
    private IEnumerator PlayTimer()
    {
        while (gameTime > 0)
        {
            --gameTime;
            uiManager.SetTime(gameTime);

            yield return new WaitForSecondsRealtime(1.0f);
        }

        // 타이머 종료 시 게임 종료
        yield break;
    }
}
