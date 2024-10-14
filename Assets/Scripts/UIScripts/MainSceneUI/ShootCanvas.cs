using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCanvas : MonoBehaviour
{
    Coroutine HitView;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        HitView = StartCoroutine("HitViewCo");
    }

    private IEnumerator HitViewCo()
    {
        yield return new WaitForSeconds(4f);
        this.gameObject.SetActive(false);
    }
}
