using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : SingleTon<UIManager>
{
    public enum eEffectType { Octopus = 0, Frozen, end,}
    private Canvas canvasForCamera;
    private Image[] screenEffects = new Image[(int)eEffectType.end];
    private Dictionary<eEffectType, string> effectKeyName = new Dictionary<eEffectType, string>()
    {
        {eEffectType.Octopus,"UI_OctopusInc" },
        {eEffectType.Frozen,"UI_FrozenScreen" },
    };

    public delegate void CallBackMethod();
    private CallBackMethod callbackTimeEnd = null;
   
    [SerializeField] private TextMeshProUGUI[] scores = null;
    [SerializeField] private PlayerUI playerUI = null;
    [SerializeField] private TextMeshProUGUI timeUI = null;

    
    public CallBackMethod SetcallbackWithTimeEnd
    {
        set { callbackTimeEnd = value; }
    }

   
    public Image GetScreenEffect(eEffectType _type)
    {
        return screenEffects[(int)_type];
    }

    public void PlayerUIEvent(eEffectType _type)
    {
        int typeValue = (int)_type;
        switch (typeValue)
        {
            case 0:
                playerUI.CallbackUIEvent_InkEvent();
                break;

            case 1:
                playerUI.CallbackUIEvent_ColdEvent();
                break;
        
            default:
            break;
        }
    }
    public void InitPlayerUI()
    {
        SetStageScore(new int[2]{0,0}); 
        scores[0].gameObject.SetActive(false);
        scores[1].gameObject.SetActive(false);
    }

    public void SetTeamScores(int[] _scores)
    {
        StartCoroutine("SetScoresWithFrame", _scores);
    }
    public void GameSetResult(int _value)
    {
        playerUI.GamesetResultEvent(_value);
    }
    
    public void SetTime(int _time)
    {       
        int min = _time / 60;
        int sec = _time % 60;

        timeUI.text = min.ToString("D2") +" : "+ sec.ToString("D2");
    
    }   

    private void SetStageScore(int[] _teamScore)
    {
        for(int i =0; i < _teamScore.GetLength(0); ++i)
        {
            scores[i].text = _teamScore[i].ToString();
        }
    }

    private IEnumerator SetScoresWithFrame(int[] _scores)
    {

        scores[0].gameObject.SetActive(true);
        scores[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        SetStageScore(_scores);

        yield return new WaitForSeconds(1.0f);

        scores[0].gameObject.SetActive(false);
        scores[1].gameObject.SetActive(false);
        

        yield break;
    }
 
    
    

}
    
