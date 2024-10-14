using UnityEngine;

[CreateAssetMenu]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public GameObject bulletPrefab;
    public float damage;
    public float speed;
    public LayerMask targetMask;  // �浹�� Ÿ�� ���̾�
}
