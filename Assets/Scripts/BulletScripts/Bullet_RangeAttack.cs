using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Bullet_RangeAttack : Bullet
{
    [SerializeField]
    protected float bombTime = 0.15f;   // ���� �� ȿ�� ��������� �ð�
    [SerializeField]
    protected float delayTime = 1.5f;   // ���� ������ �� �����̽ð�(����ź ����)
    [SerializeField]
    protected float bombRange = 30.0f;  // �ǰݓ���
    [SerializeField]
    protected ParticleSystem particle;  // ȿ�� ��ƼŬ
    [SerializeField]
    protected VisualEffect visualEffect; // �߰��� VFX �ʵ�
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
        // ParticleSystem ���
        if (particle != null)
        {
            particle.Play();
        }

        // VisualEffect ���
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
