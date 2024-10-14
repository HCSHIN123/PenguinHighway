using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamaged : MonoBehaviour
{

    public delegate void CallbackEvent();
    private CallbackEvent callbackStunEvent = null;
    public CallbackEvent SetCallbackEvent
    {
        set { callbackStunEvent = value; } 
    }

    private Image ink = null;
    private Rigidbody rb;
 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void NuckBack(float _force, Vector3 _bombPos) //RPC
    {
        
        // 입력을 담당하는 스크립트에서 입력 방지시키기(코루틴으로 Rigidbody Velosity zero될때까지 넉백상황으로 간주 or
        // 시간 정해놓고 해당시간 끝나면 강제로 velosity zero로 멈추고 넉백상황 종료)
        Vector2 dir = new Vector2(transform.position.x, transform.position.z) - new Vector2(_bombPos.x, _bombPos.z);
        dir.Normalize();
        Vector3 nuckbackDir = new Vector3(dir.x, 0f, dir.y);
        rb.AddForce(nuckbackDir * _force, ForceMode.Impulse);
        callbackStunEvent?.Invoke();
    }

    public void InkToScreen(UIManager.eEffectType _type, float _time = 3f)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        ink = UIManager.Instance.GetScreenEffect(_type).GetComponent<Image>();
        ink.gameObject.SetActive(true);
        ink.color = Color.white;
        StopAllCoroutines();
        StartCoroutine(COR_BlindScreen(_time));
    }
    IEnumerator COR_BlindScreen(float _time = 3f)
    {
        ink.gameObject.SetActive(true);
        Vector4 targetColor = new Vector4(ink.color.r, ink.color.g, ink.color.b, 0.0f);
        Color baseColor = ink.color;
        yield return new WaitForSeconds(_time);
        float time = 0f;
        while (time <= _time) //사라지는 효과
        {
            time += Time.deltaTime;
            ink.color = Color.Lerp(baseColor, targetColor, time / _time);
            yield return null;
        }
        ink.gameObject.SetActive(false);
        ink = null;
    }

   

}
