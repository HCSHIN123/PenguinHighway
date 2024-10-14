using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Bullet_ThreeWay : Bullet_DistributedAttack
{
    [SerializeField]
    private float splitSpeed = 5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float splitTiming = 0.65f;
    private void Awake()
    {
        bulletTransforms = new Transform[2];
        bulletTransforms[0] = transform.GetChild(0).transform;
        bulletTransforms[1] = transform.GetChild(1).transform;
    }
   

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space))
            isSplit = true;
    }

    public override void ReadyToShoot()
    {
        base.ReadyToShoot();
    }

    // 3개로 분리되는 총알 발사 과정을 처리하는 코루틴
    override public IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        float splitTime = 0f;           // 분리 후 경과 시간
        bool rbActivated = false;       // Rigidbody 활성화 여부 플래그

        // 모든 분리된 포탄에 중력 활성화
        foreach (Transform t in bulletTransforms)
        {
            t.GetComponent<Rigidbody>().useGravity = true;
        }

        // 경로(_path) 배열을 따라 이동
        for (int i = 0; i < _path.Length; i++)
        {
            // 충돌 발생 시 루프 중단
            if (isHited)
                break;

            // 포탄 분리 로직
            if (isSplit)
            {
                splitTime += Time.deltaTime;

                // 현재 이동 방향을 구하고 정규화
                Vector3 moveDirection = (_path[i] - transform.position).normalized;

                // 왼쪽 및 오른쪽 방향으로 회전 (90도)
                Vector3 leftDir = Quaternion.Euler(0f, -90f, 0f) * moveDirection;
                Vector3 rightDir = Quaternion.Euler(0f, 90f, 0f) * moveDirection;

                // 분리된 포탄 위치 업데이트 (월드 좌표 기준)
                bulletTransforms[0].localPosition = splitTime * 0.01f * splitSpeed * Vector3.left;
                bulletTransforms[1].localPosition = splitTime * 0.01f * splitSpeed * Vector3.right;
            }

            // 지정된 분리 타이밍에 도달 시 분리 시작
            if (i >= _path.Length * splitTiming)
                isSplit = true;

            // 총알이 이동할 다음 경로 지점으로 회전 및 이동
            transform.LookAt(_path[i]);
            transform.position = _path[i];

            // 진행도 업데이트
            progressRate = (float)i / (float)_path.Length;

            // 특정 진행도 이상에서 물리 효과 활성화
            if (progressRate > enablePhysicsTiming && !rbActivated)
            {
                EnablePhysics();  // 물리 효과 활성화
                rbActivated = true;
            }

            // 다음 프레임까지 고정된 업데이트 시간 대기
            yield return waitForFixedUpdate;
        }
    }


    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if (collision.collider.CompareTag(targetTag))
        {
            Debug.Log("THREECOL " + gameObject.transform.position);
            StopAllCoroutines();
        }
    }
}

    //override public IEnumerator COR_Shoot(float duration = 1.0f)
    //{
    //    float time = 0f;
    //    float splitTime = 0f;

    //    while (time <= 1f)
    //    {
    //        Vector3 p4 = Vector3.Lerp(start, peak, time);
    //        Vector3 p5 = Vector3.Lerp(peak, end, time);
    //        transform.position = Vector3.Lerp(p4, p5, time);
    //        if (isSplit)
    //        {
    //            splitTime += Time.deltaTime;
    //            //Vector3 leftpos = new Vector3(transform.position.x - (splitSpeed * splitTime), transform.position.y, transform.position.z);
    //            //Vector3 rightpos = new Vector3(transform.position.x + (splitSpeed * splitTime), transform.position.y, transform.position.z);
    //            //left.position = Vector3.Lerp(transform.position, leftpos, time);
    //            //right.position = Vector3.Lerp(transform.position, rightpos, time);                
    //            Vector3 leftpos =  new Vector3(transform.position.x - (splitSpeed * splitTime), transform.position.y, transform.position.z);
    //            Vector3 rightpos = new Vector3(transform.position.x + (splitSpeed * splitTime), transform.position.y, transform.position.z);
    //            left.position = Vector3.Lerp(transform.position, leftpos, time);
    //            right.position = Vector3.Lerp(transform.position, rightpos, time);
    //        }
    //        time += Time.deltaTime / (duration * range * 0.02f);
    //        yield return null;
    //    }

    //    isShooting = false;
    //}