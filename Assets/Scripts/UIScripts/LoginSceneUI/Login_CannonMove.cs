using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login_CannonMove : MonoBehaviour
{
    [SerializeField] Transform col;
    [SerializeField] Transform port;
    [SerializeField] float rotSpeed = 10f;

    private void Start()
    {
        port.localRotation = Quaternion.AngleAxis(-45f, Vector3.right);
    }

    void Update()
    {
        //Move();
        // RotateCannon();
    }


    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
            transform.localPosition += Vector3.forward * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.S))
            transform.localPosition += Vector3.back * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.D))
            transform.localPosition += Vector3.right * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.A))
            transform.localPosition += Vector3.left * Time.deltaTime * 10f;

        if (Input.GetKey(KeyCode.UpArrow)) RotateCannon(1);
        else if (Input.GetKey(KeyCode.DownArrow)) RotateCannon(-1);
    }
    public void RotateCannon(float _value)
    {

        if (port.rotation.x > -0.45f && port.rotation.x < -0.01f)
            port.Rotate(new Vector3(_value * rotSpeed * Time.deltaTime, 0f, 0f));
        // port.Rotate(new Vector3(curValue,0f,0f));
        else
            port.localRotation = Quaternion.AngleAxis(-45f, Vector3.right);

    }
}
