using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Wall : Bullet
{
    [SerializeField]
    protected GameObject createObject;

    override protected void Start()
    {
        base.Start();
    }

    override public void Shooting_Physical(Vector3[] _path)
    {
        StartCoroutine(COR_ShootingProcess(_path));
        rb.useGravity = true;
    }
    override public IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        foreach (Vector3 p in _path)
        {
            if (isHited)
                break;

            if (p == _path[_path.Length/2])
                Create(p);
            transform.LookAt(p);
            transform.position = p;
            yield return waitForFixedUpdate;
        }
    }

    virtual public void Create(Vector3 _pos)
    {
        Instantiate(createObject, _pos, createObject.transform.rotation);
    }
}
