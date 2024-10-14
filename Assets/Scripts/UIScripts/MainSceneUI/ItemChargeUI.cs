using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemChargeUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> itemIcons = null;
    [SerializeField] private Image targetImage = null; 


    private void Awake()
    {
        targetImage.sprite = null;
        targetImage.color = new Color(255f,255f,255f, 0f);
    }

   

}
 