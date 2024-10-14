using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 테스트용 코드 PC버전에서 이동코드
/// </summary>
public class CannonMove : MonoBehaviour
{
    [SerializeField]
    Transform col;
    [SerializeField]
    Transform port;
    [SerializeField]
    float rotSpeed = 10f;
   
   

    
   
    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.back * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * Time.deltaTime * 10f;
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * Time.deltaTime * 10f;
    }
    public void RotateCannon()
    {
        if (Input.GetKey(KeyCode.UpArrow) && port.rotation.x > -0.65f)
        {
            port.Rotate(new Vector3(-rotSpeed * Time.deltaTime, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.DownArrow) && port.rotation.x < -0.2f)
        {
            port.Rotate(new Vector3(rotSpeed * Time.deltaTime, 0f, 0f));
        }
       
    }

    public void RotateCannon(float _y)
    {
        if (_y > 0f && port.rotation.x > -0.4f)
            port.Rotate(new Vector3(-rotSpeed * Time.deltaTime, 0f, 0f));
        if (_y < 0f && port.rotation.x < -0.2f)
            port.Rotate(new Vector3(rotSpeed * Time.deltaTime, 0f, 0f));
        
    }
}
