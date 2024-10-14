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
        
        // �Է��� ����ϴ� ��ũ��Ʈ���� �Է� ������Ű��(�ڷ�ƾ���� Rigidbody Velosity zero�ɶ����� �˹��Ȳ���� ���� or
        // �ð� ���س��� �ش�ð� ������ ������ velosity zero�� ���߰� �˹��Ȳ ����)
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
        while (time <= _time) //������� ȿ��
        {
            time += Time.deltaTime;
            ink.color = Color.Lerp(baseColor, targetColor, time / _time);
            yield return null;
        }
        ink.gameObject.SetActive(false);
        ink = null;
    }

   

}
