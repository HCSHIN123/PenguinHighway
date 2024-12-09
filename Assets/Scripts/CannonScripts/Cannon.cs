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


    // ������) �ݹ� ��������Ʈ ����, Gift �Ѿ� �߻� ���¸� Ȯ���ϱ� ���� ���
    public delegate void CallbackMethod(bool _value);
    private CallbackMethod indicateGiftCallback = null;
    public CallbackMethod SetcallbackIsGift
    {
        set { indicateGiftCallback = value; }
    }

    // �Ѿ��� �����ϴ� BulletFactory ����
    public FieldItemManager bulletFactory;
    [SerializeField]
    private AudioSource audioSource;

    public UnityEngine.UI.Slider slider = null; // �߻� �������� �����̴� UI
    public float bulletVelocity = 120f;         // �߻� �ӵ�
    public float maxRange = 250f;               // �ִ� ��Ÿ�

    private float launchAngle = 45f;            // �߻� ���� (y�� ����)
    private float initialVelocity = 10f;        // �ʱ� �ӵ�
    private float directionAngle = 0f;          // �߻� ���� (y�� ��鿡���� ����)
    private Bullet bullet = null;               // ���� ������ �Ѿ�
    private FirePort firePort;                  // �Ѿ��� �߻�Ǵ� ��Ʈ
    private LineRenderer lr;                    // �Ѿ� ������ �׸��� ���� ������
    private List<Vector3> bulletPathList = new List<Vector3>(); // �Ѿ��� �̵� ��θ� �����ϴ� ����Ʈ
    private float gauge = 0f;                   // �߻� ������
    private const float gravity = 9.81f;       // �߷� ���ӵ� ���
    private float curRange = 0.2f;              // ���� ���� (�������� ���� �޶���)
    [SerializeField]
    private int curveLevel = 2;                 // �������� ũ�⸦ �����ϴ� ����
    [SerializeField]
    private float power = 2.0f;
    [SerializeField]
    private Transform shootPort;                // �߻������ġ
    private ItemChargeUI itemChargeUI = null;   // ���ð������� �����ִ� UI
    private float groundHeight = 25f;           // ���� ����
    private PhotonView pv;                      // Photon��
    /// <summary>
    /// �׽�Ʈ�뺯����
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
    private void TEST_BulletApply() // �׽�Ʈ�ڵ�
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

        // �׽�Ʈ�ڵ�
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
        // 360�� �̻� ���� ������ �ʵ��� ����
        if (launchAngle >= 360f)
            launchAngle %= 360f;
        // 180�� �̸� ������ ����
        if (launchAngle > 180f)
            launchAngle = 360f - launchAngle;
        directionAngle = transform.eulerAngles.y;

        if (Input.GetMouseButton(0))
            TriggerOn();
        else if (Input.GetMouseButtonUp(0))
            TriggerOut();
    }

    public void TriggerOn() // ������ä��� ó��
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

    public void TriggerOut() // �������� ���� �߻�ó��
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

    public bool RequestBullet(int _idx) // �Ѿ˿�ûRPCȣ���Լ�
    {
        if (bullet != null)
            return false;
        pv.RPC("RequestBulletNetwork", RpcTarget.All, _idx);
        return true;
    }
    
    public void SetBullet(Bullet _bullet) // �Ѿ�����RPCȣ���Լ�
    {
        if(_bullet == null) 
            return;
        
        pv.RPC("SetBulletNetwork", RpcTarget.AllBuffered, _bullet);
    }

    public void AttachBullet(Bullet _go)    // �Ѿ� �����ϴ� �޼ҵ�
    {
        if (bullet != null)
            return;
        bullet = _go;   // �Ѿ� ����
        bullet.ReadyToShoot();  // �Ѿ� �ʱ�ȭ(�غ�)
        bullet.gameObject.transform.SetParent(firePort.bulletPos, false); // �Ѿ� �ڽ����� ���̱�
        bullet.gameObject.transform.localPosition = Vector3.zero;   // ���������� �ʱ�ȭ
    }


    private void UpdateBulletPath()
    {
        bulletPathList.Clear();                                 // ��θ���Ʈ �ʱ�ȭ
        SetInitialVelocity();                                   // ���� ������ ��Ÿ��� ������ ������� �ʱ� �ӵ� ����
        float angleUpDown = launchAngle * Mathf.Deg2Rad;        // ���� ������ ���� ������ ��ȯ 
        float angleLeftRight = directionAngle * Mathf.Deg2Rad;  // �¿� ������ ���� ������ ��ȯ
        Vector3 initialPosition = firePort.transform.position;  // �߻���ġ �ʱ�ȭ

        // �߻� ���� ���� ���
        Vector3 initialVelocityVector = new Vector3(Mathf.Sin(angleLeftRight), Mathf.Sin(angleUpDown) * curveLevel, Mathf.Cos(angleLeftRight));

        int i = 0;
        Vector3 position;
        while (true)
        {
            float progressRatio = i / maxRange; // �ð�����(���൵)
            float time = bulletVelocity * progressRatio * (2 * Mathf.Sin(angleUpDown) / gravity); // ����ð���� �Ѿ˼ӵ� * ������� * �ð�����

            // �ش� �ð��� ���� ��ġ���
            position = CalculatePositionAtTime(initialPosition, initialVelocityVector * initialVelocity, time);

            bulletPathList.Add(position); // ��� ��Ͽ� �߰�
            if (progressRatio >= 0.5f && position.y <= groundHeight) // �����̻��� ��������, ������̸�ŭ y���� �������� ����
                break;
            ++i;
        }

        // LineRenderer�� ��θ� �����Ͽ� �ð������� ǥ��
        lr.positionCount = bulletPathList.Count / 3;    // ���̵� ������ ���� ����� 3����1�� ���̰� ����
        lr.SetPositions(bulletPathList.ToArray());
    }


    private void SetInitialVelocity() // ��Ÿ��� �°� �ʱ� �ӵ��� �����ϴ� �Լ�
    {
        float radianLaunchAngle = launchAngle * Mathf.Deg2Rad;              // �߻簢���� �������� ��ȯ
        float horizontalDistance = curRange * Mathf.Cos(radianLaunchAngle); // ����Ÿ�
        float verticalDistance = curRange * Mathf.Sin(radianLaunchAngle);   // �����Ÿ�

        // �ְ��� ���޽ð�
        float timeToReachTop = Mathf.Sqrt(2 * verticalDistance / gravity);
        // �� ����ð�
        float totalFlightTime = (2 * timeToReachTop);
        // �ʱ�ӵ���� (���� �Ÿ� / ��ü ���� �ð�) * �Ŀ�
        initialVelocity = horizontalDistance / totalFlightTime * power;
    }


    private Vector3 CalculatePositionAtTime(Vector3 initialPosition, Vector3 initialVelocity, float time) // �� �����Ӻ� �Ѿ��� ��ġ�� ����ϴ� �Լ�
    {
        // ������ � ����
        // x(t) = x0 + v0x * t
        float x = initialPosition.x + initialVelocity.x * time;
        // y(t) = y0 + v0y * t + -0.5 * g * t^2
        float y = initialPosition.y + (initialVelocity.y * time) - (0.5f * gravity * time * time);
        // z(t) = z0 + v0z * t
        float z = initialPosition.z + initialVelocity.z * time;

        return new Vector3(x, y, z); // ���� ��ġ ��ȯ
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
    // ������ ���
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
