using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class Cannon : MonoBehaviourPunCallbacks
{
    public GameObject[] bullets;


    // 손재현) 콜백 델리게이트 정의, Gift 총알 발사 상태를 확인하기 위해 사용
    public delegate void CallbackMethod(bool _value);
    private CallbackMethod indicateGiftCallback = null;
    public CallbackMethod SetcallbackIsGift
    {
        set { indicateGiftCallback = value; }
    }

    // 총알을 생성하는 BulletFactory 참조
    public FieldItemManager bulletFactory;
    [SerializeField]
    private AudioSource audioSource;

    public UnityEngine.UI.Slider slider = null; // 발사 게이지용 슬라이더 UI
    public float bulletVelocity = 120f;         // 발사 속도
    public float maxRange = 250f;               // 최대 사거리

    private float launchAngle = 45f;            // 발사 각도 (y축 기준)
    private float initialVelocity = 10f;        // 초기 속도
    private float directionAngle = 0f;          // 발사 방향 (y축 평면에서의 각도)
    private Bullet bullet = null;               // 현재 장착된 총알
    private FirePort firePort;                  // 총알이 발사되는 포트
    private LineRenderer lr;                    // 총알 궤적을 그리는 라인 렌더러
    private List<Vector3> bulletPathList = new List<Vector3>(); // 총알의 이동 경로를 저장하는 리스트
    private float gauge = 0f;                   // 발사 게이지
    private const float gravity = 9.81f;       // 중력 가속도 상수
    private float curRange = 0.2f;              // 현재 범위 (게이지에 따라 달라짐)
    [SerializeField]
    private int curveLevel = 2;                 // 포물선의 크기를 정의하는 레벨
    [SerializeField]
    private float power = 2.0f;
    [SerializeField]
    private Transform shootPort;                // 발사시작위치
    private ItemChargeUI itemChargeUI = null;   // 슈팅게이지를 보여주는 UI
    private float groundHeight = 25f;           // 지면 높이
    private PhotonView pv;                      // Photon뷰
    /// <summary>
    /// 테스트용변수들
    /// </summary>
    public Bullet testBullet;                   
    [SerializeField]
    private int testIdx = 0;                    

    private void Start()
    {
        itemChargeUI = GetComponentInChildren<ItemChargeUI>();
        firePort = GetComponentInChildren<FirePort>();
        lr = GetComponent<LineRenderer>();
        pv = GetComponentInParent<PhotonView>();
        bulletFactory = GameManager.Instance.fieldItemManager;

        bullets = Resources.LoadAll<GameObject>("Prefab/Bullet_Prefab/");
    }

    [PunRPC]
    private void TEST_BulletApply() // 테스트코드
    {
        if (bullet != null)
        { 
            bullet.gameObject.SetActive(false);
            bullet = null;
        }
        
        bullet = GameManager.Instance.fieldItemManager.TEST_GetBullet();
        if (bullet is Bullet_Gift)
            indicateGiftCallback?.Invoke(true);
        else
            indicateGiftCallback?.Invoke(false);

        bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
        bullet.ReadyToShoot();

    }
    private void Update()
    {
        if (pv.IsMine == false)
            return;

        // 테스트코드
        //if (Input.GetKeyUp(KeyCode.Space))
        //    AttachBulletRandom();
        //if (Input.GetKeyUp(KeyCode.Alpha1))
        //{
        //    bullet = testBullet;
        //    bullet.ReadyToShoot();
        //    bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
        //    bullet.gameObject.transform.localPosition = Vector3.zero;
        //}
        if (Input.GetKeyUp(KeyCode.Q))
            pv.RPC("TEST_BulletApply", RpcTarget.All);
        launchAngle = firePort.transform.eulerAngles.x;
        // 360도 이상 값이 나오지 않도록 제한
        if (launchAngle >= 360f)
            launchAngle %= 360f;
        // 180도 미만 값으로 보정
        if (launchAngle > 180f)
            launchAngle = 360f - launchAngle;
        directionAngle = transform.eulerAngles.y;

        if (Input.GetMouseButton(0))
            TriggerOn();
        else if (Input.GetMouseButtonUp(0))
            TriggerOut();
    }

    public void TriggerOn() // 게이지채우는 처리
    {
        if (bullet == null)
            return;
        gauge += Time.deltaTime;
        if (gauge > 1.0f)
            gauge = 1.0f;
        slider.value = gauge;
        curRange = maxRange * slider.value;
        UpdateBulletPath();
    }

    public void TriggerOut() // 게이지에 따른 발사처리
    {
        if (bullet == null)
            return;
        gauge = 0f;
        slider.value = gauge;
        
        pv.RPC("ShootingNetwork", RpcTarget.All, bulletPathList.ToArray());
        indicateGiftCallback?.Invoke(false);
        audioSource.GetComponent<AudioSource>().Play();
         if(shootPort.childCount > 0)
        {
            Debug.Log("DET : " + shootPort.childCount);
            shootPort.DetachChildren();
        }
        bullet?.gameObject.transform.SetParent(firePort.bulletPos, false);
        bullet = null;
    }

    public void StealBullet()
    {
        if (bullet is Bullet_Gift)
        {
            bullet = null;
            gauge = 0f;
            slider.value = gauge;
            lr.positionCount = 0;
        }
    }

    public bool RequestBullet(int _idx) // 총알요청RPC호출함수
    {
        if (bullet != null)
            return false;
        pv.RPC("RequestBulletNetwork", RpcTarget.All, _idx);
        return true;
    }
    
    public void SetBullet(Bullet _bullet) // 총알장착RPC호출함수
    {
        if(_bullet == null) 
            return;
        
        pv.RPC("SetBulletNetwork", RpcTarget.AllBuffered, _bullet);
    }

    public void AttachBullet(Bullet _go)    // 총알 부착하는 메소드
    {
        if (bullet != null)
            return;
        bullet = _go;   // 총알 장착
        bullet.ReadyToShoot();  // 총알 초기화(준비)
        bullet.gameObject.transform.SetParent(firePort.bulletPos, false); // 총알 자식으로 붙이기
        bullet.gameObject.transform.localPosition = Vector3.zero;   // 로컬포지션 초기화
    }


    private void UpdateBulletPath()
    {
        bulletPathList.Clear();                                 // 경로리스트 초기화
        SetInitialVelocity();                                   // 현재 설정된 사거리와 각도를 기반으로 초기 속도 조정
        float angleUpDown = launchAngle * Mathf.Deg2Rad;        // 상하 각도를 라디안 값으로 변환 
        float angleLeftRight = directionAngle * Mathf.Deg2Rad;  // 좌우 각도를 라디안 값으로 변환
        Vector3 initialPosition = firePort.transform.position;  // 발사위치 초기화

        // 발사 방향 벡터 계산
        Vector3 initialVelocityVector = new Vector3(Mathf.Sin(angleLeftRight), Mathf.Sin(angleUpDown) * curveLevel, Mathf.Cos(angleLeftRight));

        int i = 0;
        Vector3 position;
        while (true)
        {
            float progressRatio = i / maxRange; // 시간비율(진행도)
            float time = bulletVelocity * progressRatio * (2 * Mathf.Sin(angleUpDown) / gravity); // 비행시간계산 총알속도 * 진행비율 * 시간공식

            // 해당 시간에 대한 위치계산
            position = CalculatePositionAtTime(initialPosition, initialVelocityVector * initialVelocity, time);

            bulletPathList.Add(position); // 경로 목록에 추가
            if (progressRatio >= 0.5f && position.y <= groundHeight) // 절반이상이 지났을때, 지면높이만큼 y값이 낮아지면 종료
                break;
            ++i;
        }

        // LineRenderer에 경로를 설정하여 시각적으로 표시
        lr.positionCount = bulletPathList.Count / 3;    // 난이도 조정을 위해 경로의 3분의1만 보이게 설정
        lr.SetPositions(bulletPathList.ToArray());
    }


    private void SetInitialVelocity() // 사거리에 맞게 초기 속도를 조정하는 함수
    {
        float radianLaunchAngle = launchAngle * Mathf.Deg2Rad;              // 발사각도를 라디안으로 변환
        float horizontalDistance = curRange * Mathf.Cos(radianLaunchAngle); // 수평거리
        float verticalDistance = curRange * Mathf.Sin(radianLaunchAngle);   // 수직거리

        // 최고점 도달시간
        float timeToReachTop = Mathf.Sqrt(2 * verticalDistance / gravity);
        // 총 비행시간
        float totalFlightTime = (2 * timeToReachTop);
        // 초기속도계산 (수평 거리 / 전체 비행 시간) * 파워
        initialVelocity = horizontalDistance / totalFlightTime * power;
    }


    private Vector3 CalculatePositionAtTime(Vector3 initialPosition, Vector3 initialVelocity, float time) // 각 프레임별 총알의 위치를 계산하는 함수
    {
        // 포물선 운동 공식
        // x(t) = x0 + v0x * t
        float x = initialPosition.x + initialVelocity.x * time;
        // y(t) = y0 + v0y * t + -0.5 * g * t^2
        float y = initialPosition.y + (initialVelocity.y * time) - (0.5f * gravity * time * time);
        // z(t) = z0 + v0z * t
        float z = initialPosition.z + initialVelocity.z * time;

        return new Vector3(x, y, z); // 계산된 위치 반환
    }
    #region PunRPCFuncs
    [PunRPC]
    public void RequestBulletNetwork(int _idx)
    {
        bullet = bulletFactory.GetBullet(_idx);
        if (bullet is Bullet_Gift)
            indicateGiftCallback?.Invoke(true);
        else
            indicateGiftCallback?.Invoke(false);

        bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
        bullet.ReadyToShoot();
    }

    [PunRPC]
    public void ShootingNetwork(Vector3[] _arr)
    {
        bullet.transform.SetParent(null, false);
        bullet.Shooting_Physical(_arr);
        bullet = null;
        lr.positionCount = 0;
    }

    [PunRPC]
    public void SetBulletNetwork(Bullet _bullet)
    {
         bullet = _bullet;
        _bullet.ReadyToShoot();
        _bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
    }
    #endregion

    /*
    // 베지어곡선 방식
    private void UpdateBulletPath()
    {
        peak.position = start.position + firePort.Dir * bullet.Range;
        float dis = Vector3.Distance(new Vector3(peak.position.x, 0f, peak.position.z), new Vector3(start.position.x, 0f, start.position.z));
        Vector3 dir = new Vector3(peak.position.x, 0f, peak.position.z) - new Vector3(start.position.x, 0f, start.position.z);
        dir.Normalize();
        end.position = start.position + dir * dis * 2f;


        bulletPathList.Clear();

        if (start != null && peak != null && end != null)
        {
            for (int i = 0; i < lineDetail; i++)
            {
                float t = (i / lineDetail);
                Vector3 p4 = Vector3.Lerp(start.position, peak.position, t);
                Vector3 p5 = Vector3.Lerp(peak.position, end.position, t);
                bulletPathList.Add(Vector3.Lerp(p4, p5, t));
            }
        }
        lr.SetPositions(bulletPathList.ToArray());
    }*/
}
