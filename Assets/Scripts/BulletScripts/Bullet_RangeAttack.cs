using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Bullet_RangeAttack : Bullet
{
    [SerializeField]
    protected float bombTime = 0.15f;   // 폭발 후 효과 적용까지의 시간
    [SerializeField]
    protected float delayTime = 1.5f;   // 땅에 떨어진 후 딜레이시간(수류탄 느낌)
    [SerializeField]
    protected float bombRange = 30.0f;  // 피격볌위
    [SerializeField]
    protected ParticleSystem particle;  // 효과 파티클
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
