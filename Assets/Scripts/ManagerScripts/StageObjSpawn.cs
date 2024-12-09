using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjSpawn : MonoBehaviour
{
    // ���� �ֱ� (�� ����)
    [SerializeField]
    private float spawnTime = 7f;

    // X��� Z�� ������
    [SerializeField]
    private float offsetX = 100f;
    [SerializeField]
    private float offsetZ = 80f;

    // ���� ���� ���� (2~30 ����)
    [SerializeField, Range(2f, 30f)]
    private int spawnCount = 15;

    // ������ ����� �ڽ� �ݶ��̴� �� ��� ����
    private BoxCollider colliderBox = null;
    private Bounds bound;

    // ���� �ð��� �ʵ� ������ ������ ����
    private float currentTime = 0f;
    private FieldItemManager bulletFactory;

    private void Awake()
    {
        // �ڽ� �ݶ��̴� ������Ʈ ��������
        colliderBox = GetComponent<BoxCollider>();
        // �ڽ� �ݶ��̴��� ��� ����
        bound = colliderBox.bounds;
    }

    private void Start()
    {
        // GameManager�� ���� FieldItemManager ��������
        bulletFactory = GameManager.Instance.fieldItemManager;
    }

    private void Update()
    {
        // ����� �ƹ� ���۵� ���� ���� (�ڵ� �ּ� ó����)
        return;

        // �ð� ������Ʈ
        currentTime += Time.deltaTime;

        // ���� �ֱ� ���� �� ���� ����
        if (currentTime >= spawnTime)
        {
            currentTime = 0f;
            // ������ ��ġ�� �ʵ� ������ ����
            bulletFactory.CreateFieldItem(GetRandomPosition());
        }
    }

    // �ڽ� �ݶ��̴� ������ ������ ��ġ ��ȯ
    private Vector3 GetRandomPosition()
    {
        // �ڽ� �ݶ��̴��� X��� Z�� ���� ������ ���� �� ����
        float randX = Random.Range(bound.min.x, bound.max.x);
        float randZ = Random.Range(bound.min.z, bound.max.z);

        // ��ȯ ���� Y���� ���������� ����
        return new Vector3(randX, 24.5f, randZ);
    }

    // ���������� �߽� ��ġ ��ȯ
    public Vector3 GetCenterPosInStage()
    {
        return transform.position;
    }
}
