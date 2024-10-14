using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Bullet_RangeAttack : Bullet
{
    [SerializeField]
    protected float bombTime = 0.15f;
    [SerializeField]
    protected float delayTime = 1.5f;
    [SerializeField]
    protected float bombRange = 30.0f;
    [SerializeField]
    protected ParticleSystem particle;
    [SerializeField]
    protected VisualEffect visualEffect; // 추가된 VFX 필드
    protected bool isUsed = false;

    abstract protected void Bomb();

    override protected void Start()
    {
        base.Start();
    }

    virtual protected IEnumerator COR_Bomb()
    {
        yield return new WaitForSecondsRealtime(delayTime);

        BombEffect();

        yield return new WaitForSecondsRealtime(bombTime);

        Bomb();
    }

    virtual protected void BombEffect()
    {
        // ParticleSystem 재생
        if (particle != null)
        {
            particle.Play();
        }

        // VisualEffect 재생
        if (visualEffect != null)
        {
            visualEffect.enabled = true;
            visualEffect.Play();
        }
    }

    public override void ReadyToShoot()
    {
        base.ReadyToShoot();
        isUsed = false;
    }
}
