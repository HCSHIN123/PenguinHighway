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

    // 3���� �и��Ǵ� �Ѿ� �߻� ������ ó���ϴ� �ڷ�ƾ
    override public IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        float splitTime = 0f;           // �и� �� ��� �ð�
        bool rbActivated = false;       // Rigidbody Ȱ��ȭ ���� �÷���

        // ��� �и��� ��ź�� �߷� Ȱ��ȭ
        foreach (Transform t in bulletTransforms)
        {
            t.GetComponent<Rigidbody>().useGravity = true;
        }

        // ���(_path) �迭�� ���� �̵�
        for (int i = 0; i < _path.Length; i++)
        {
            // �浹 �߻� �� ���� �ߴ�
            if (isHited)
                break;

            // ��ź �и� ����
            if (isSplit)
            {
                splitTime += Time.deltaTime;

                // ���� �̵� ������ ���ϰ� ����ȭ
                Vector3 moveDirection = (_path[i] - transform.position).normalized;

                // ���� �� ������ �������� ȸ�� (90��)
                Vector3 leftDir = Quaternion.Euler(0f, -90f, 0f) * moveDirection;
                Vector3 rightDir = Quaternion.Euler(0f, 90f, 0f) * moveDirection;

                // �и��� ��ź ��ġ ������Ʈ (���� ��ǥ ����)
                bulletTransforms[0].localPosition = splitTime * 0.01f * splitSpeed * Vector3.left;
                bulletTransforms[1].localPosition = splitTime * 0.01f * splitSpeed * Vector3.right;
            }

            // ������ �и� Ÿ�ֿ̹� ���� �� �и� ����
            if (i >= _path.Length * splitTiming)
                isSplit = true;

            // �Ѿ��� �̵��� ���� ��� �������� ȸ�� �� �̵�
            transform.LookAt(_path[i]);
            transform.position = _path[i];

            // ���൵ ������Ʈ
            progressRate = (float)i / (float)_path.Length;

            // Ư�� ���൵ �̻󿡼� ���� ȿ�� Ȱ��ȭ
            if (progressRate > enablePhysicsTiming && !rbActivated)
            {
                EnablePhysics();  // ���� ȿ�� Ȱ��ȭ
                rbActivated = true;
            }

            // ���� �����ӱ��� ������ ������Ʈ �ð� ���
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