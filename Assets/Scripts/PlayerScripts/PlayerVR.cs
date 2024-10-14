using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerVR : MonoBehaviourPunCallbacks
{ 
    public enum PlayerTeam
    {
        TeamA = 0 , TeamB = 1
    }

    public delegate void vrTriggerdelegate();
    private vrTriggerdelegate VrTriggerdelegate = null;
    public vrTriggerdelegate SetVrTriggerdelegate { set { VrTriggerdelegate = value; } }

    private vrTriggerdelegate VrTriggerOutdelegate = null;
    public vrTriggerdelegate SetVrTriggerOutdelegate { set { VrTriggerOutdelegate = value; } }

    public delegate void vrButtondelegate();
    private vrButtondelegate VrButtondelegate = null;
    public vrButtondelegate SetVrButtondelegate { set { VrButtondelegate = value; } }

    private vrButtondelegate xButtondelegate = null;
    public vrButtondelegate SetXButtonGameEnd
    {
        set{xButtondelegate = value;}      
    }
    public InputActionProperty[] inputAction;

    [SerializeField] private GameObject myPlayerModel = null;
    [SerializeField] private GameObject otherPlayerModel = null;
    [SerializeField] private GameObject cannon = null;
    [SerializeField] private ParticleSystem stunEffect = null;
    private float curValue = 0f;
    private float rotSpeed = 50f;
    public Transform initTranform = null;
    public PlayerTeam myTeam = PlayerTeam.TeamA;
    [SerializeField] private PlayerIndicator playerIndicator = null;
    private Cannon myCannon = null;
    private PlayerDamaged playerDamaged = null;

    private void Awake()
    {

        if(XRSettings.isDeviceActive)
        {
            if(photonView.IsMine)
            {
                myPlayerModel.SetActive(true);
                otherPlayerModel.SetActive(false);
                otherPlayerModel.GetComponentInChildren<Camera>().gameObject.SetActive(false);

            }else
            {
                myPlayerModel.SetActive(false);
                otherPlayerModel.SetActive(true);
                otherPlayerModel.GetComponentInChildren<Camera>().gameObject.SetActive(false);
   
            }
        }else
        {
            if(photonView.IsMine)
            {
                myPlayerModel.SetActive(false);
                otherPlayerModel.SetActive(true);
                otherPlayerModel.GetComponentInChildren<Camera>().gameObject.SetActive(true);

            }
            else
            {
                myPlayerModel.SetActive(false);
                otherPlayerModel.SetActive(true);
                otherPlayerModel.GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
        }

        // playerIndicator = GetComponentInChildren<PlayerIndicator>();    
        playerDamaged = GetComponentInChildren<PlayerDamaged>();


        playerDamaged.SetCallbackEvent = StartStunEffect;

        stunEffect.gameObject.SetActive(false);

        myCannon = GetComponentInChildren<Cannon>();    
        myCannon.SetcallbackIsGift = GetIndicatorTargetwithGift;
 
    }


    public void SetInitTransform(Transform _value)
    {
        initTranform = _value;
    }

    public void SetIndicatorTargets(Transform[] _target)
    {
  
        playerIndicator.InitTargets(_target);
    }

    private void GetIndicatorTargetwithGift(bool _isGift)
    {
        playerIndicator.SetTarget(_isGift);
    }


    private void Start()
    {
        inputAction[0].action.performed += XButtonActionQuitGame;
        inputAction[2].action.performed += YbuttonActionCallBack;
        GetIndicatorTargetwithGift(true);
    }



    
    private void Update()
    {
        if(!photonView.IsMine) return;

        if(transform.position.y < 19f) RespawnPlayer();
        
        float inputTrigger = inputAction[1].action.ReadValue<float>();
        Vector2 cannonAngle = inputAction[3].action.ReadValue<Vector2>();

        if(!XRSettings.isDeviceActive) MoveForPC();
        else
        {
            if(inputTrigger > 0) {

            cannon.GetComponentInChildren<Cannon>().TriggerOn();
            curValue = inputTrigger;
            }
            else if(curValue > inputTrigger)
            {
                cannon.GetComponentInChildren<Cannon>().TriggerOut();
                curValue = 0f;
            }

            cannon.GetComponentInChildren<CannonMove>().RotateCannon(cannonAngle.y);       

        }
        
        
    }

    private void MoveForPC()
    {
        Vector3 dir = transform.forward;
        Vector3 side = transform.right;
        
        if (Input.GetKey(KeyCode.W))
            transform.position +=  dir * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.S))
            transform.position += -dir * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.D))
            transform.position += side * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.A))
            transform.position += -side * Time.deltaTime * 30f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            cannon.GetComponentInChildren<CannonMove>().RotateCannon(1f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cannon.GetComponentInChildren<CannonMove>().RotateCannon(-1f);
        }

        // if (Input.GetKey(KeyCode.UpArrow))
        //     cannon.GetComponentInChildren<CannonMove>().RotateCannon(1f);
        // else if (Input.GetKey(KeyCode.DownArrow))
        //     cannon.GetComponentInChildren<CannonMove>().RotateCannon(-1f);
    }

    private void RespawnPlayer()
    {
        this.transform.position = initTranform.position;
        this.transform.rotation = initTranform.localRotation;
    }

    private void XButtonActionCallback(InputAction.CallbackContext _context) 
    { 
        
    }

    private void YbuttonActionCallBack(InputAction.CallbackContext _context)
    { 
        RespawnPlayer();
    }

    private void XButtonActionQuitGame(InputAction.CallbackContext _context) 
    {
        xButtondelegate?.Invoke();
    }

    private void StartStunEffect()
    {
        StartCoroutine("StunEvent");
    }

    private IEnumerator StunEvent()
    {
        stunEffect.gameObject.SetActive(true);
        stunEffect.Play();
        yield return new WaitForSecondsRealtime(1.5f);
        stunEffect.Stop();
        stunEffect.gameObject.SetActive(false);
        yield break;

    }
}
