using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCtrl : MonoBehaviour
{
    [SerializeField] private GameObject indicator = null;

    private void Awake()
    {
        indicator.SetActive(false);
    }

    private void Update()
    {
        float curRot = Vector3.Dot(transform.right, Vector3.up);
        // Debug.Log(curRot);
        if(curRot >= 0.95f) indicator.SetActive(true);
        else indicator.SetActive(false);
    }
}
