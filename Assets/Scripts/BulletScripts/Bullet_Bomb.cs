using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bullet_Bomb : Bullet_RangeAttack
{
    [SerializeField]
    private float nuckBackForce = 10f;      // ³Ë¹éÈû

    private BulletFrame[] bulletFrame;      // BulletÀÇ ¿ÜÇü
    
    override protected void Start()
    {
        base.Start();
        bulletFrame = GetComponentsInChildren<BulletFrame>();
    }
    override public void ReadyToShoot()
    {
        base.ReadyToShoot();
        foreach (BulletFrame b in bulletFrame)
        {
            b.gameObject.SetActive(true);
        }
    }

    protected override void Bomb()
    {
        Collider[] target = Physics.OverlapSphere(transform.position, bombRange, targetMask);
        foreach(Collider pd  in target)
        {
            pd.GetComponent<PlayerDamaged>().NuckBack(nuckBackForce, transform.position);
        }
            
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

    protected override void BombEffect()
    {
        base.BombEffect();
        foreach (BulletFrame b in bulletFrame)
        {
            b.gameObject.SetActive(false);
        }
    }
}
