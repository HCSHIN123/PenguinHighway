using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login_Cannon : MonoBehaviour
{
    [SerializeField] private FieldItemManager bulletFactory;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem particleSystem = null;
    public UnityEngine.UI.Slider slider = null;
    public float bulletVelocity = 120f; //발사 속도
    public float maxRange = 250f; // 최대 발사범위

    private float launchAngle = 45f; // 발사 각도 (y축 기준)
    private float initialVelocity = 10f; // 초기 속도
    private float directionAngle = 0f; // 발사 방향 (y축 평면에서의 각도)
    private Bullet bullet = null;
    private FirePort firePort;
    private LineRenderer lr;
    private List<Vector3> bulletPathList = new List<Vector3>();
    private float gauge = 0f;
    private const float gravity = -9.81f;
    private float curRange = 0.2f;
    [SerializeField] private int testIdx = 0;
    private void Start()
    {
        firePort = GetComponentInChildren<FirePort>();
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            AttachBulletRandom();
        /*if (Input.GetKeyUp(KeyCode.Alpha1))
            SetBulletNetwork(testIdx);*/

        if (Input.GetMouseButton(0))
            TriggerOn();
        else if (Input.GetMouseButtonUp(0))
            TriggerOut();

        launchAngle = firePort.transform.eulerAngles.x;
        // 360도 이상 값이 나오지 않도록 제한
        if (launchAngle >= 360f)
            launchAngle %= 360f;
        // 180도 미만 값으로 변환
        if (launchAngle > 180f)
            launchAngle = 360f - launchAngle;
        directionAngle = transform.eulerAngles.y;
    }

    // public void SetBullet(Bullet.bulletType _type)
    // {
    //     AttachBullet((int)_type);
    // }

    public void TriggerOn()
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

    public void TriggerOut()
    {
        if (bullet == null) return;

        UpdateBulletPath();
        gauge = 0f;
        slider.value = gauge;
        if (bullet?.gameObject.activeSelf == true)
        {
           ShootingNetwork(bulletPathList.ToArray());
            audioSource.GetComponent<AudioSource>().Play();
            //particleSystem.Play(true);
        }
    }

    public void AttachBulletRandom()
    {
        if (bullet != null)
            return;
        //int randint = Random.Range(0, bulletFactory.bulletPrefabs.Length);

        //SetBulletNetwork(randint);
    }

    public void ShootingNetwork(Vector3[] _arr)
    {
        bullet.Shooting_Physical(_arr);
        bullet.gameObject.transform.SetParent(null, false);
        bullet = null;
        lr.positionCount = 0;
    }

    public void SetBulletNetwork(int _idx)
    {
        // Debug.Log("SETBULLET");
       // bullet = bulletFactory.CreateBulletPrefabs(_idx, firePort.bulletPos.position);
        bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
        bullet.ReadyToShoot();
    }

    private void AttachBullet(int _idx)
    {
        bullet?.gameObject.SetActive(false);
        bullet.gameObject.transform.SetParent(firePort.bulletPos, false);
        bullet.ReadyToShoot();
    }

    private void UpdateBulletPath()
    {
        bulletPathList.Clear();
        AdjustInitialVelocityForDistance();
        float angleUpDown = launchAngle * Mathf.Deg2Rad; // 상하각도
        float angleLeftRight = directionAngle * Mathf.Deg2Rad; // 좌우각도

        Vector3 initialPosition = firePort.transform.position;

        // 좌우방향 벡터 계산 (y축 중심) x = sin(theta), z = cos(theta)
        Vector3 direction = new Vector3(Mathf.Sin(angleLeftRight), 0, Mathf.Cos(angleLeftRight));

        // 초기 속도 벡터 계산
        Vector3 initialVelocityVector = new Vector3(
            direction.x * initialVelocity * Mathf.Cos(angleUpDown),
            initialVelocity * Mathf.Sin(angleUpDown),
            direction.z * initialVelocity * Mathf.Cos(angleUpDown)
        );

        // 고도에 따른 초기 속도 벡터 계산
        initialVelocityVector += Vector3.up * initialVelocity * Mathf.Sin(angleUpDown) * Mathf.Sin(angleUpDown) * Mathf.Sin(angleUpDown);

        for (int i = 0; ; i++)
        {
            float t = i / maxRange;
            float time = bulletVelocity * t * 2 * Mathf.Sin(angleUpDown) / -gravity;
            Vector3 position = CalculatePositionAtTime(initialPosition, initialVelocityVector, time);
            bulletPathList.Add(position);
            if (i >= 20 && position.y <= -0.3f)
                break;
        }

        lr.positionCount = bulletPathList.Count / 3;
        lr.SetPositions(bulletPathList.ToArray());
    }
    private void AdjustInitialVelocityForDistance()
    {
        float radianLaunchAngle = launchAngle * Mathf.Deg2Rad;
        float horizontalDistance = curRange * Mathf.Cos(radianLaunchAngle);
        float verticalDistance = curRange * Mathf.Sin(radianLaunchAngle);
        float timeToReachDistance = Mathf.Sqrt(2 * verticalDistance / -gravity);
        initialVelocity = horizontalDistance / timeToReachDistance;
    }

    private Vector3 CalculatePositionAtTime(Vector3 initialPosition, Vector3 initialVelocity, float time)
    {
        float x = initialPosition.x + initialVelocity.x * time;
        float y = initialPosition.y + initialVelocity.y * time + 0.5f * gravity * time * time;
        float z = initialPosition.z + initialVelocity.z * time;
        return new Vector3(x, y, z);
    }
}
