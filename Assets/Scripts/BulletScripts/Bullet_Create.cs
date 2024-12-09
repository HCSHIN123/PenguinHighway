using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Create : Bullet
{
    [SerializeField]
    protected GameObject createObject;  // 생성시킬 오브젝트
    [SerializeField]
    protected float growingSpeed;   // 생성속도
    [SerializeField]
    protected bool growingMode = false; // On/Off 변수 테스트용
   
    override protected void Start()
    {
        base.Start();
    }

    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if(collision.collider.CompareTag(targetTag))    // 정해놓은 오브젝트와 충돌시 생성
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

    virtual protected IEnumerator COR_Grow()    // 자식클래스의 특성에 맞게 오버라이딩
    {


        yield return null;

    }
    
}
