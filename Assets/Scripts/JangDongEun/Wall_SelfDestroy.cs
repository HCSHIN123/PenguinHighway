using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall_SelfDestroy : MonoBehaviour
{
    private void Start()
    {
        Invoke("SelfDestroy", 3f);
    }

    private void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
}
