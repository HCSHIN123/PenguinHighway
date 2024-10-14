using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingleTon<StageManager>
{
    // ���� ���۽� �ʵ忡 ��ġ�� �Ѿ� ������Ʈ�� ���� ȹ��
    // ���� ����� �Ѿ˵��� ������ ���� ���� ������ GameManager���� ��ȯ
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
