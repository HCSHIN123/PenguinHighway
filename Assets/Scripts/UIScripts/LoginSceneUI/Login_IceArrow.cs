using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login_IceArrow : MonoBehaviour
{
    private Image arrowImg;
    private Coroutine co;
    private bool isOn = false;

    private void Start()
    {
        arrowImg = GetComponent<Image>();
    }

    private IEnumerator BlinkArrow()
    {
        while (isOn)
        {
            arrowImg.enabled = false;
            yield return new WaitForSeconds(0.5f);
            arrowImg.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    public void IsOn(bool value)
    {
        isOn = value;
        if (isOn)
        {
            if (co != null) StopAllCoroutines();
            co = StartCoroutine("BlinkArrow");
        }
        else StopAllCoroutines();
    }
}
