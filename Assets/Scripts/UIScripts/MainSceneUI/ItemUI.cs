using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private Image itemImage;
    public Sprite[] item;
    public Cannon canon;
    private int n;

    private void Start()
    {
        itemImage = GetComponentsInChildren<Image>()[1];
        ShowItem();
    }

    public void SelectItem()
    {
        if (n == -1)
        {
            return;
        }

        //canon.SetBullet((Bullet.bulletType)n);

        n = -1;
    }

    public void ShowItem()
    {
        StartCoroutine("RandomItemCoroutine");
    }

    private IEnumerator RandomItemCoroutine()
    {

        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < item.Length; ++j)
            {
                itemImage.sprite = item[j];
                yield return new WaitForSeconds(0.1f);
            }
        }

        n = Random.Range(0, item.Length);
        itemImage.sprite = item[n];

        
        yield break;
    }
}
