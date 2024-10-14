using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Login_Player : MonoBehaviour
{
    public InputActionProperty[] inputAction;

    [SerializeField] private GameObject cannon = null;
    private float curValue = 0f;
    private float rotSpeed = 50f;

    public Transform initTranform = null;

    public void SetInitTransform(Transform _value)
    {
        initTranform = _value;
    }

    private void Start()
    {
        inputAction[0].action.performed += XButtonActionCallback;
        inputAction[2].action.performed += YbuttonActionCallBack;
    }

    private void Update()
    {
        float inputTrigger = inputAction[1].action.ReadValue<float>();
        Vector2 cannonAngle = inputAction[3].action.ReadValue<Vector2>();
        if (!XRSettings.isDeviceActive) Move();
        if (inputTrigger > 0)
        {
            cannon.GetComponentInChildren<Login_Cannon>().TriggerOn();
            curValue = inputTrigger;
        }
        else if (curValue > inputTrigger)
        {
            cannon.GetComponentInChildren<Login_Cannon>().TriggerOut();
            curValue = 0f;
        }


        if (cannonAngle != Vector2.zero)
            cannon.GetComponentInChildren<Login_CannonMove>().RotateCannon(cannonAngle.y);
    }


    private void Move()
    {
        Vector3 dir = transform.forward;
        Vector3 side = transform.right;

        if (Input.GetKey(KeyCode.W))
            transform.localPosition += dir * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.S))
            transform.localPosition += -dir * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.D))
            transform.localPosition += side * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.A))
            transform.localPosition += -side * Time.deltaTime * 10f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0f, -rotSpeed * Time.deltaTime, 0f));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0f, rotSpeed * Time.deltaTime, 0f));
        }

        if (Input.GetKey(KeyCode.UpArrow))
            cannon.GetComponentInChildren<Login_CannonMove>().RotateCannon(1f);
        else if (Input.GetKey(KeyCode.DownArrow))
            cannon.GetComponentInChildren<Login_CannonMove>().RotateCannon(-1f);
    }

    private void RespawnPlayer()
    {
        this.transform.position = initTranform.position;
        this.transform.rotation = initTranform.localRotation;
    }

    private void XButtonActionCallback(InputAction.CallbackContext _context)
    {
        RespawnPlayer();
    }

    private void YbuttonActionCallBack(InputAction.CallbackContext _context)
    {
        cannon.GetComponentInChildren<Login_Cannon>().AttachBulletRandom();
    }
}
