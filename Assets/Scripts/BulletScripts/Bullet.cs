using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action ReturnToPoolCallback;
    public int keyId = -1;

    // 초기화용 변수들
    private Vector3 startPos = Vector3.zero;
    private Quaternion startRot = Quaternion.identity;

    [SerializeField]
    protected LayerMask targetMask = 3; // 총알 타겟의 레이어
    [SerializeField]
    protected float endTime = 0.0f;     // 사용된 총알이 땅에 남아서 비활성화되는 시간

    protected WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    protected bool isHited = false;              // 발사이후, 땅이나 목표물과의 충돌여부
    protected string targetTag = "Breakable";    // 총알과 충돌가능한  태그
    protected string wallTag = "Wall";           // 벽 충돌체 태그
    protected string playerTag = "Player";       // 플레이어 태그
    [SerializeField]
    private float normalNuckbackForce = 20f;
    [SerializeField]
    protected Rigidbody rb;

    protected float enablePhysicsTiming = 0.3f;

    protected float progressRate = 0.0f;      // 진행도
    protected AudioSource sound;              // 사운드
    virtual protected void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();
        sound = GetComponent<AudioSource>();
    }

    public void InitBullet(int _keyId, Action _returnToPool)
    {
        keyId = _keyId;
        ReturnToPoolCallback = _returnToPool;
        if (ReturnToPoolCallback == null)
        {
            Debug.LogError("ReturnToPoolCallback is null");
        }
       
    }

    virtual public void ReadyToShoot()  // 발사준비, 장전시 호출되는 총알의 상태를 초기화하는 함수
    {
        gameObject.SetActive(true);         // SetActive 활성화
        isHited = false;                    // 충돌여부 false
        progressRate = 0.0f;                // 진행도 0으로 초기화
        ReadyRB();                          // 리지드바디 발사준비
        ReadyTransform();                   // 트랜스폼 준비

    }
    virtual public void Shooting_Physical(Vector3[] _path)  // 대포로부터 받은 포물선경로를 기반으로 발사진행하는 함수
    {
        StartCoroutine(COR_ShootingProcess(_path));
    }
   
    virtual public IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        bool readyCol = false;      // 충돌 감지 활성화 여부 플래그

        // 경로의 각 지점(p)으로 이동
        for (int i = 0; i < _path.Length; i++)
        {
            // 충돌이 발생하면 발사 중단
            if (isHited)
                break;
            // 현재 위치에서 목표 지점으로 회전
            transform.LookAt(_path[i]);
            // 목표 지점으로 이동
            transform.position = _path[i];
            // 진행도 업데이트 (현재 경로 인덱스를 전체 경로 길이로 나눈 값)
            progressRate = (float)i / (float)_path.Length;
            // 특정 진행도 이상일 때 물리 효과 활성화(근접공격방지)
            if (progressRate > enablePhysicsTiming && !readyCol)
            {
                EnablePhysics();   // 물리 효과 활성화
                readyCol = true;   // 충돌 감지 활성화 플래그 설정
            }
            // 다음 프레임까지 대기
            yield return waitForFixedUpdate;
        }
    }

    // 물리 효과를 활성화하는 함수
    public void EnablePhysics()
    {
        rb.useGravity = true;   // 중력 활성화
        rb.isKinematic = false; // 물리 효과 활성화 (비동적 상태 해제)
    }

    
    protected void OnCollisionEnter(Collision collision)
    {
        OnCollisionBulletEvent(collision);
    }
    virtual protected void OnCollisionBulletEvent(Collision collision)
    {
        if (isHited)    // 이미 충돌을 했다면 Return
            return;

        if (collision.gameObject.CompareTag(playerTag))  // 플레이어와 충돌
        {
            collision.gameObject.GetComponent<PlayerDamaged>().NuckBack(normalNuckbackForce, transform.position);   // 플레이어 넉백
            HitTargetEvent();   // 이벤트처리(효과음, 충돌플레그, 풀링)
        }

        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag(wallTag)) // 목표나 벽과 충돌
        {
            HitTargetEvent();   // 이벤트처리
        }
    }

    public void HitBullet()
    {
        isHited = true;
    }
    private void ReadyRB()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.useGravity = false;              // 중력 비활성화
        rb.isKinematic = true;              // 물리효과 비활성화
    }

    private void ReadyTransform()
    {
        transform.localPosition = startPos; // 발사위치 초기화(장전시에는 대포의 자식으로 들어가기 때문에 localPos세팅)
        transform.localRotation = startRot; // 회전값 초기화
    }

    private void HitTargetEvent()
    {
        isHited = true;   // 먼저 충돌 처리
        sound?.Play();    // 효과음 재생
        Invoke("Die", 10f);
    }

    private void Die()
    {
        Debug.Log("BULLET DIE" + ReturnToPoolCallback);
        
        this.gameObject.SetActive(false);
        ReturnToPoolCallback();
    }

}

