using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action ReturnToPoolCallback; //������� �Լ��� ��� ����
    public int keyId = -1;  // ���� Ű��

    // �ʱ�ȭ�� ������
    private Vector3 startPos = Vector3.zero;
    private Quaternion startRot = Quaternion.identity;

    [SerializeField]
    protected LayerMask targetMask = 3; // �Ѿ� Ÿ���� ���̾�
    [SerializeField]
    protected float endTime = 0.0f;     // ���� �Ѿ��� ���� ���Ƽ� ��Ȱ��ȭ�Ǵ� �ð�

    protected WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    protected bool isHited = false;              // �߻�����, ���̳� ��ǥ������ �浹����
    protected string targetTag = "Breakable";    // �Ѿ˰� �浹������  �±�
    protected string wallTag = "Wall";           // �� �浹ü �±�
    protected string playerTag = "Player";       // �÷��̾� �±�
    [SerializeField]
    private float normalNuckbackForce = 20f;
    [SerializeField]
    protected Rigidbody rb;

    protected float enablePhysicsTiming = 0.3f;

    protected float progressRate = 0.0f;      // ���൵
    protected AudioSource sound;              // ����
    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        sound = GetComponent<AudioSource>();
    }

    public void InitBullet(int _keyId, Action _returnToPool) // ������ ȣ��
    {
        keyId = _keyId; // �����Ǵ� ��ųʸ��� ���� Ű��
        ReturnToPoolCallback = _returnToPool; // ����� ������ Ǯ���Ǵ� ������ �ݹ��Լ�
    }

    virtual public void ReadyToShoot()  // �߻��غ�, ������ ȣ��Ǵ� �Ѿ��� ���¸� �ʱ�ȭ�ϴ� �Լ�
    {
        gameObject.SetActive(true);         // SetActive Ȱ��ȭ
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        isHited = false;                    // �浹���� false
        progressRate = 0.0f;                // ���൵ 0���� �ʱ�ȭ
        ReadyRB();                          // ������ٵ� �߻��غ�
        ReadyTransform();                   // Ʈ������ �غ�

    }
    virtual public void Shooting_Physical(Vector3[] _path)  // �����κ��� ���� ��������θ� ������� �߻������ϴ� �Լ�
    {
        StartCoroutine(COR_ShootingProcess(_path));
    }
   
    virtual public IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        bool readyCol = false;      // �浹 ���� Ȱ��ȭ ���� �÷���

        // ����� �� ����(p)���� �̵�
        for (int i = 0; i < _path.Length; i++)
        {
            // �浹�� �߻��ϸ� �߻� �ߴ�
            if (isHited)
                break;
            // ���� ��ġ���� ��ǥ �������� ȸ��
            transform.LookAt(_path[i]);
            // ��ǥ �������� �̵�
            transform.position = _path[i];
            // ���൵ ������Ʈ (���� ��� �ε����� ��ü ��� ���̷� ���� ��)
            progressRate = (float)i / (float)_path.Length;
            // Ư�� ���൵ �̻��� �� ���� ȿ�� Ȱ��ȭ(�������ݹ���)
            if (progressRate > enablePhysicsTiming && !readyCol)
            {
                EnablePhysics();   // ���� ȿ�� Ȱ��ȭ
                readyCol = true;   // �浹 ���� Ȱ��ȭ �÷��� ����
            }
            // ���� �����ӱ��� ���
            yield return waitForFixedUpdate;
        }
    }

    // ���� ȿ���� Ȱ��ȭ�ϴ� �Լ�
    public void EnablePhysics()
    {
        rb.useGravity = true;   // �߷� Ȱ��ȭ
        rb.isKinematic = false; // ���� ȿ�� Ȱ��ȭ (���� ���� ����)
    }

    
    protected void OnCollisionEnter(Collision collision)
    {
        OnCollisionBulletEvent(collision);
    }
    virtual protected void OnCollisionBulletEvent(Collision collision)
    {
        if (isHited)    // �̹� �浹�� �ߴٸ� Return
            return;

        if (collision.gameObject.CompareTag(playerTag))  // �÷��̾�� �浹
        {
            collision.gameObject.GetComponent<PlayerDamaged>().NuckBack(normalNuckbackForce, transform.position);   // �÷��̾� �˹�
            HitTargetEvent();   // �̺�Ʈó��(ȿ����, �浹�÷���, Ǯ��)
        }

        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag(wallTag)) // ��ǥ�� ���� �浹
        {
            HitTargetEvent();   // �̺�Ʈó��
        }
    }

    public void HitBullet()
    {
        isHited = true; // �浹�̺�Ʈ �ߺ�ó�� ��������
    }
    private void ReadyRB()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.useGravity = false;              // �߷� ��Ȱ��ȭ
        rb.isKinematic = true;              // ����ȿ�� ��Ȱ��ȭ
    }

    private void ReadyTransform()
    {
        transform.localPosition = startPos; // �߻���ġ �ʱ�ȭ(�����ÿ��� ������ �ڽ����� ���� ������ localPos����)
        transform.localRotation = startRot; // ȸ���� �ʱ�ȭ
    }

    private void HitTargetEvent()
    {
        isHited = true;   // ���� �浹 ó��
        sound?.Play();    // ȿ���� ���
        Invoke("Die", 10f); // 10�� �� ���ó��
    }

    private void Die()
    {
        this.gameObject.SetActive(false); // ������Ʈ ��Ȱ��ȭ
        ReturnToPoolCallback(); // Ǯ�� �ݹ��Լ� ����
    }

}

