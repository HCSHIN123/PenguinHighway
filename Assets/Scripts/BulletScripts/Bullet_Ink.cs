using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ink : Bullet_RangeAttack
{
    [SerializeField]
    private UIManager.eEffectType type;


    protected override void Bomb()
    {
        Collider[] target = Physics.OverlapSphere(transform.position, bombRange, targetMask);   // Sphere������ �ִ� �ݶ��̴� Ž��
        if (target.Length >= 1) // �����ϸ� �ش� ��� ȿ�� ����
            target[0].GetComponentInChildren<UIManager>()?.PlayerUIEvent(type);
            
    }
    
    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if (collision.collider.CompareTag(targetTag) && !isUsed)
        {
            isHited = true;
            isUsed = true;
            StartCoroutine(COR_Bomb());
            
        }
    }

}
