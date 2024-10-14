using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ink : Bullet_RangeAttack
{
    [SerializeField]
    private UIManager.eEffectType type;


    protected override void Bomb()
    {
        Collider[] target = Physics.OverlapSphere(transform.position, bombRange, targetMask);
        if (target.Length >= 1)
            target[0].GetComponentInChildren<UIManager>()?.PlayerUIEvent(type);
            // UIManager.Instance.PlayerUIEvent(type);
            // target[0].GetComponent<PlayerDamaged>().InkToScreen(type); // RPC로 상대한테만
    }
    
    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if (collision.collider.CompareTag(targetTag) && !isUsed)
        {
            isHited = true;
            isUsed = true;
            StartCoroutine(COR_Bomb());
            // Bomb();
        }
    }

}
