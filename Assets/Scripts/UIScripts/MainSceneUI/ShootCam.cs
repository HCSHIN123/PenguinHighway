using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootCam : MonoBehaviour
{
    private Vector3 TargetPos;
    [SerializeField] GameObject test;
    [SerializeField] GameObject test2;

    public void ShootOn(Vector3 targetPos, Vector3 heightPos)
    {
        TargetPos = test.transform.position;
        TargetPos.z = TargetPos.z - 10f;
        TargetPos.y = TargetPos.y + 2f;
        this.transform.position = TargetPos;
        this.transform.LookAt(test2.transform.position);
    }
}
