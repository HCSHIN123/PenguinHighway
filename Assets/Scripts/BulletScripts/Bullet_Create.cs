using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Create : Bullet
{
    [SerializeField]
    protected GameObject createObject;  // ������ų ������Ʈ
    [SerializeField]
    protected float growingSpeed;   // �����ӵ�
    [SerializeField]
    protected bool growingMode = false; // On/Off ���� �׽�Ʈ��
   
    override protected void Start()
    {
        base.Start();
    }

    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if(collision.collider.CompareTag(targetTag))    // ���س��� ������Ʈ�� �浹�� ����
        {
            Create(this.gameObject.transform.position);
        }
    }

    virtual public void Create(Vector3 _pos)
    {
        if(growingMode)
            StartCoroutine(COR_Grow());
        else
            Instantiate(createObject, _pos, createObject.transform.rotation);
    }

    virtual protected IEnumerator COR_Grow()    // �ڽ�Ŭ������ Ư���� �°� �������̵�
    {


        yield return null;

    }
    
}
