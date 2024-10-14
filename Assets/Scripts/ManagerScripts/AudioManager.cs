using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

      
        AudioClip bgmClip = Resources.Load<AudioClip>("DowntownBGM");
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
        }
    }

    private void Start()
    {
       
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
