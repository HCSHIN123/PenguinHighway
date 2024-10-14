using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetScore : MonoBehaviour
{

    [SerializeField] ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gift"))
        {
            Destroy(other.gameObject);
            particle.Play();
        }
    }
}
