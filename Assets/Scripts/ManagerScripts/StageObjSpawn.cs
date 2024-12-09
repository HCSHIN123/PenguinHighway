using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjSpawn : MonoBehaviour
{
    // 스폰 주기 (초 단위)
    [SerializeField]
    private float spawnTime = 7f;

    // X축과 Z축 오프셋
    [SerializeField]
    private float offsetX = 100f;
    [SerializeField]
    private float offsetZ = 80f;

    // 스폰 개수 설정 (2~30 범위)
    [SerializeField, Range(2f, 30f)]
    private int spawnCount = 15;

    // 스폰에 사용할 박스 콜라이더 및 경계 정보
    private BoxCollider colliderBox = null;
    private Bounds bound;

    // 현재 시간과 필드 아이템 관리자 참조
    private float currentTime = 0f;
    private FieldItemManager bulletFactory;

    private void Awake()
    {
        // 박스 콜라이더 컴포넌트 가져오기
        colliderBox = GetComponent<BoxCollider>();
        // 박스 콜라이더의 경계 설정
        bound = colliderBox.bounds;
    }

    private void Start()
    {
        // GameManager를 통해 FieldItemManager 가져오기
        bulletFactory = GameManager.Instance.fieldItemManager;
    }

    private void Update()
    {
        // 현재는 아무 동작도 하지 않음 (코드 주석 처리됨)
        return;

        // 시간 업데이트
        currentTime += Time.deltaTime;

        // 스폰 주기 도달 시 스폰 실행
        if (currentTime >= spawnTime)
        {
            currentTime = 0f;
            // 무작위 위치에 필드 아이템 생성
            bulletFactory.CreateFieldItem(GetRandomPosition());
        }
    }

    // 박스 콜라이더 내부의 무작위 위치 반환
    private Vector3 GetRandomPosition()
    {
        // 박스 콜라이더의 X축과 Z축 범위 내에서 랜덤 값 생성
        float randX = Random.Range(bound.min.x, bound.max.x);
        float randZ = Random.Range(bound.min.z, bound.max.z);

        // 반환 값의 Y축은 고정값으로 설정
        return new Vector3(randX, 24.5f, randZ);
    }

    // 스테이지의 중심 위치 반환
    public Vector3 GetCenterPosInStage()
    {
        return transform.position;
    }
}
