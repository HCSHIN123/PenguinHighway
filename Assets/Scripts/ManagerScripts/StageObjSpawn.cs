using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjSpawn : MonoBehaviour
{
    [SerializeField]
    private float spawnTime = 7f;
    [SerializeField]
    private float offsetX = 100f;
    [SerializeField]
    private float offsetZ = 80f;
    [SerializeField, Range(2f, 30f)]
    private int spawnCount = 15;
    private BoxCollider colliderBox = null;
    private Bounds bound;

    private float currentTime = 0f;
    private FieldItemManager bulletFactory;


    private void Awake()
    {
        colliderBox = GetComponent<BoxCollider>();
        bound = colliderBox.bounds;
    }
    private void Start()
    {
        bulletFactory = GameManager.Instance.fieldItemManager;
    }
    private void Update()
    {
        return;
        currentTime += Time.deltaTime;
        if( currentTime >= spawnTime )
        {
            currentTime = 0f;
            bulletFactory.CreateFieldItem(GetRandomPosition());
            
        }
    }

    private Vector3 GetRandomPosition()
    {
        // float randX = Random.Range(transform.position.x - offsetX, transform.position.x + offsetX);
        // float randZ = Random.Range(transform.position.z - offsetZ, transform.position.z + offsetZ);

        float randX = Random.Range(bound.min.x, bound.max.x);
        float randZ = Random.Range(bound.min.z, bound.max.z);

        return new Vector3(randX, 24.5f, randZ);
    }



    public Vector3 GetCenterPosInStage()
    {       
        return transform.position;
    }
    
}

