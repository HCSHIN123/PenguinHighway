using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ink : Bullet_RangeAttack
{
    [SerializeField]
    private UIManager.eEffectType type;


    protected override void Bomb()
    {
        Collider[] target = Physics.OverlapSphere(transform.position, bombRange, targetMask);   // Sphere범위의 있는 콜라이더 탐지
        if (target.Length >= 1) // 존재하면 해당 대상에 효과 적용
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
