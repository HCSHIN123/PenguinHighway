using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image popUpImg_Ink = null;
    [SerializeField] private Image popUpImg_Cold = null;
    [SerializeField] private Image uWinImg = null;
    [SerializeField] private Image uLoseImg = null;
    [SerializeField] private Image drawGame = null;
    [SerializeField] private Button quitButton = null;
    [SerializeField] private TextMeshProUGUI timeSet = null;
    [SerializeField] private float debuffTime = 2.0f;


    private void Awake()
    {
        popUpImg_Ink.gameObject.SetActive(false);
        popUpImg_Cold.gameObject.SetActive(false);
        uWinImg.gameObject.SetActive(false);
        uLoseImg.gameObject.SetActive(false);
        drawGame.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

    }

    public void CallbackUIEvent_InkEvent()
    {
        StartCoroutine("Event_Ink");
    }

    private IEnumerator Event_Ink()
    {
        popUpImg_Ink.gameObject.SetActive(true);

        Vector4 targetColor = new Vector4(popUpImg_Ink.color.r, popUpImg_Ink.color.g, popUpImg_Ink.color.b, 0.0f);
        Color baseColor = popUpImg_Ink.color;
        yield return new WaitForSeconds(debuffTime);
        float time = 0f;
        while (time <= debuffTime) //사라지는 효과
        {
            time += Time.deltaTime;
            popUpImg_Ink.color = Color.Lerp(baseColor, targetColor, time / debuffTime);
            yield return new WaitForFixedUpdate();
        }
        popUpImg_Ink.gameObject.SetActive(false);
        popUpImg_Ink.color = baseColor;
    }

    public void CallbackUIEvent_ColdEvent()
    {
       StartCoroutine("Event_Cold");
    }

    private IEnumerator Event_Cold()
    {
        popUpImg_Cold.gameObject.SetActive(true);

        Vector4 targetColor = new Vector4(popUpImg_Ink.color.r, popUpImg_Cold.color.g, popUpImg_Cold.color.b, 0.0f);
        Color baseColor = popUpImg_Cold.color;
        yield return new WaitForSeconds(debuffTime);
        float time = 0f;
        while (time <= debuffTime) //사라지는 효과
        {
            time += Time.deltaTime;
            popUpImg_Cold.color = Color.Lerp(baseColor, targetColor, time / debuffTime);
            yield return new WaitForFixedUpdate();
        }
        popUpImg_Cold.gameObject.SetActive(false);
        popUpImg_Cold.color = baseColor;
    }

    public void GamesetResultEvent(int _value)
    {
        switch(_value)
        {
            case 0:
                uWinImg.gameObject.SetActive(true);
                break;
            case 1:
                uLoseImg.gameObject.SetActive(true);
                break;
            case 2:
                drawGame.gameObject.SetActive(true);
                break;

            default:
                break;
        }

        quitButton.gameObject.SetActive(true);
    }

   
 

}
