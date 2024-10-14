using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
        public enum Stagesite
    {    Asite, Bsite   }
    public delegate void CallBackScore(int _teamNum);
    private CallBackScore callBackAddScore = null;

    public CallBackScore SetCallBackAddScore
    {
        set { callBackAddScore = value;}
    }
    
    [SerializeField] ParticleSystem particle = null;
    [SerializeField] private AudioSource audioSource = null;
    public Transform GoalPos = null;
    public Stagesite myStage = Stagesite.Asite;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
  
    private void OnTriggerEnter(Collider _collider)
    {
        if(_collider.gameObject.CompareTag("Gift"))
        {
            _collider.GetComponent<Bullet>()?.HitBullet();
            _collider.GetComponent<FieldItem>().Respawn();
            callBackAddScore?.Invoke((int)myStage);
            particle.Play();
            audioSource.Play();
        }
              
    }

}


