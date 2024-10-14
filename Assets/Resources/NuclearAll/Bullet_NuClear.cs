using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_NuClear : Bullet
{
    [SerializeField]
    protected ParticleSystem Nuclear = null;


    override protected void Start()
    {
        base.Start();
        if (Nuclear != null)
        {
            Nuclear.Stop();

        }
    }

    protected override void OnCollisionBulletEvent(Collision collision)
    {
        base.OnCollisionBulletEvent(collision);
        if(collision.collider.CompareTag(targetTag))
        {
            Nuclear.Play();
        }
    }

}
