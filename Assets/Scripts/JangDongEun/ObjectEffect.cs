using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffect : MonoBehaviour
{
    public GameObject followObj;
    private Vector3 prevPos;
    

    public bool use = true;
    

    private void Update()
    {
        if (!use)
            return;
       
       if(followObj)transform.position = followObj.transform.position;
       else Destroy(this.gameObject);
        
        
       
        prevPos = followObj.transform.position;
    }
    public void Attach(GameObject _obj)
    {
        transform.SetParent(followObj.transform, true);
        transform.localPosition = Vector3.zero;
    }

}
