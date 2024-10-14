using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Create : Bullet
{
    [SerializeField]
    protected GameObject createObject;
    [SerializeField]
    protected float growingSpeed;
    [SerializeField]
    protected bool growingMode = false;
   
    override protected void Start()
    {
        base.Start();
    }

    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if(collision.collider.CompareTag(targetTag) )
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

    virtual protected IEnumerator COR_Grow()
    {


        yield return null;

    }
    
}
