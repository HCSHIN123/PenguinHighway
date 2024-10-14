using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private Transform[] targetGoal = null;

    private Transform curTarget = null;

    public void InitTargets(Transform[] _target)
    {
        targetGoal = _target;        
    }
    public void SetTarget(bool _isGiftInCannon)
    {
        curTarget = _isGiftInCannon? targetGoal[1] : targetGoal[0];
    }
    private void Update()
    {
        if(curTarget)transform.LookAt(curTarget.position);
    }

}
