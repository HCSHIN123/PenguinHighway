using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_OptionalMove : Bullet
{
    [SerializeField]
    private bool onOptionalMove = false;
    bool endSpecialPath = false;
    [SerializeField, Range(0.0f, 1.0f)]
    private float moveChangeTiming = 0.55f;

    protected List<Vector3> specialPath = new List<Vector3>();
    public override IEnumerator COR_ShootingProcess(Vector3[] _path)
    {
        for (int i = 0; i < _path.Length; i++)
        {
            if (isHited)
                break;

            if(onOptionalMove && !endSpecialPath)
            {
                specialPath.Clear();
                AddVerticalCircularMotion(_path[i]);
                Vector3[] path = specialPath.ToArray();
                int idx = 0;
                while(path.Length > idx )
                {
                    if (isHited)
                        break;
                    transform.LookAt(path[idx]);
                    transform.position = path[idx];
                    idx++;
                    yield return waitForFixedUpdate;
                }
                endSpecialPath = true;
            }

            if (i >= _path.Length * moveChangeTiming)
                onOptionalMove = true;
            transform.LookAt(_path[i]);
            transform.position = _path[i];
            yield return waitForFixedUpdate;
        }
    }

    virtual protected void AddVerticalCircularMotion(Vector3 centerPosition)
    {
         float radius = 10f; // 원의 반지름
         float angularSpeed = 2 * Mathf.PI; // 각속도 (1초에 한 바퀴 도는 속도)
         int points = 100; // 원 운동을 위한 포인트 개수

         for (int i = 0; i < points; i++)
         {
            float angle = angularSpeed * i / points;
            float x = centerPosition.x;
            float y = centerPosition.y + radius * -Mathf.Sin(angle);
            float z = centerPosition.z + radius * Mathf.Cos(angle);
            specialPath.Add(new Vector3(x, y, z));
         }
    }
    
}
