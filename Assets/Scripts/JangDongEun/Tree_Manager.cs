using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Manager : MonoBehaviour
{
    private PhotonView pv;
    private List<GameObject> treeList = new List<GameObject>();
    private Vector3[] treePos = null;

    private void Awake()
    {
        pv = transform.GetComponent<PhotonView>();
    }

    private void Start()
    {
        for (int i = 1; i < transform.childCount; ++i)
        {
            treeList.Add(transform.GetChild(i).gameObject);
        }

        treePos = new Vector3[transform.childCount];
    }

    public void SetTreeRandom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < treeList.Count; ++i)
            {
                Vector3 newPos = treeList[i].transform.localPosition;
                newPos.z = newPos.z + Random.Range(-10, 10);
                treePos[i] = newPos;
            }

            pv.RPC("SetPos", RpcTarget.All, treePos);
        }
    }

    [PunRPC]
    public void SetPos(Vector3[] newPos)
    {
        for (int i = 0; i < treeList.Count; ++i)
        {
            treeList[i].transform.localPosition = newPos[i];
        }
    }
}
