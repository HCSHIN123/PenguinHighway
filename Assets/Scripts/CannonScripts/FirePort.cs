using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대포의 끝에 위치하여 현재 대포의 방향(각도)를 제공해주는 코드
/// </summary>
public class FirePort : MonoBehaviour
{
    public Transform bulletPos;

    private Vector3 dir = Vector3.zero;
    public Vector3 Dir 
    {
        get 
        {
            dir = bulletPos.position - transform.position;
            dir.Normalize();
            return dir;
        }
    }
}
