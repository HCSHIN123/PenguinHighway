using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private bool myTurn = false;
   

    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MyTurnEnd()
    {
        myTurn = false;
        
    }
    public void MyTurnStart()
    {
        myTurn = true;
    }
}
