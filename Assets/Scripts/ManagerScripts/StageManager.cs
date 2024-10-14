using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingleTon<StageManager>
{
    // 게임 시작시 필드에 배치된 총알 오브젝트의 갯수 획득
    // 게임 진행시 총알들이 나가고 들어감에 따른 점수를 GameManager에게 반환
    public delegate void CallbackScoreWithTeam(int _teamNum);
    private CallbackScoreWithTeam callbackAddTeamScore = null;

    [SerializeField] private Stage[] stages = null;
    [SerializeField] private StageObjSpawn stageObjSpawn = null;


    public CallbackScoreWithTeam SetCallbackMethod_Add
    {
        set { callbackAddTeamScore = value; }
    }

    public Transform GetStageGoal(Stage.Stagesite _value)
    {
        foreach(Stage stage in stages)
        {
            if(stage.myStage == _value) return stage.GoalPos;
        }
        return null;
    }

    private void Awake()
    {
        stages = GetComponentsInChildren<Stage>();

        foreach(Stage stage in stages)
            stage.SetCallBackAddScore = AddScore;
        
    }

    private void AddScore(int _teamNum)
    {
        Debug.Log($"{stages[_teamNum].name} Get Score");
        callbackAddTeamScore?.Invoke(_teamNum);
    }

    public Vector3 GetCenterPosInstage()
    {
        return stageObjSpawn.GetCenterPosInStage();
    }

    
}
