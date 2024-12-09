using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Gift : Bullet
{
    private FieldItem myFieldItem;
    private void Awake()
    {
        myFieldItem = GetComponent<FieldItem>();
    }
    protected override void OnCollisionBulletEvent(Collision collision)
    {
        if (isHited)    // 이미 충돌을 했다면 Return
            return;
        if (progressRate <= 0.2f)   // 발사중 대포나 이동중인 본인과의 충돌을 방지하고자 진행도 20퍼 이하일때는 충돌이벤트X
            return;
        if (collision.gameObject.CompareTag("Player"))  // 플레이어와 충돌시 이벤트
        {
            myFieldItem.ItemCollisionPlayer(collision);
            if (progressRate <= 0.2f)
                return;
            if (isHited)
                return;
            isHited = true;
            rb.isKinematic = true;
            transform.localPosition = Vector3.zero;
            sound?.Play();
            return;
        }
        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag(wallTag)) //땅이나 벽에 박았을때
        {
            isHited = true;

            StopAllCoroutines();
            return;

        }
    }
}
