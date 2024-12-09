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
        if (isHited)    // �̹� �浹�� �ߴٸ� Return
            return;
        if (progressRate <= 0.2f)   // �߻��� ������ �̵����� ���ΰ��� �浹�� �����ϰ��� ���൵ 20�� �����϶��� �浹�̺�ƮX
            return;
        if (collision.gameObject.CompareTag("Player"))  // �÷��̾�� �浹�� �̺�Ʈ
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
        if (collision.gameObject.CompareTag(targetTag) || collision.gameObject.CompareTag(wallTag)) //���̳� ���� �ھ�����
        {
            isHited = true;

            StopAllCoroutines();
            return;

        }
    }
}
