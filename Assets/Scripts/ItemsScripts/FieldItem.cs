using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField]
    public Bullet bullet = null;
    public GameObject bulletObject = null;

    private Vector3 centerPos = Vector3.zero;
    public int id = -1;

    public int keyId = -1;

    private Cannon owner = null;
    private bool isTouchable = true;


    public void InitFieldItem(int _keyId, Action _callback)
    {
        keyId = _keyId;
        bulletObject = Instantiate(bullet.gameObject);
        Bullet b = bulletObject.GetComponent<Bullet>();
        b.InitBullet(keyId, _callback);
        b.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (transform.position.y < 15f)
        {
            Respawn();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 접촉 && 그 플레이어가 장착한 총알이 없다면 총알적용
        if(other.transform.CompareTag("Player") && other.GetComponentInChildren<Cannon>().RequestBullet(keyId))
        {
            GameManager.Instance.fieldItemManager.ItemDie(keyId);

        }
    }
    
    public bool IsReady()
    {
        //return 
        return false;
    }

    public void ItemCollisionPlayer(Collision collision)
    {
        if (!isTouchable)
            return;
        if (collision.transform.CompareTag("Player"))
        {
            if (owner != null)
            {
                owner.StealBullet();
            }
            Cannon newOwner = collision.gameObject.GetComponentInChildren<Cannon>();
            newOwner.AttachBullet(GetComponent<Bullet>());
            owner = newOwner;
            StartCoroutine(COR_Untouchable());
        }
    }

    IEnumerator COR_Untouchable()
    {
        isTouchable = false;
        yield return new WaitForSecondsRealtime(1.5f);
        isTouchable = true;
    }

    public void SetSpawnPos(Vector3 _centerPos)
    {
        centerPos = _centerPos;
    }

    public void Respawn()
    {
        transform.position = centerPos;
    }


}
