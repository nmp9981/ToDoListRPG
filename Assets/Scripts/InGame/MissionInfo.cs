using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct Mission
{
    public string Title;
    public int getExp;
    public bool isRepeat;
}

[Serializable]
public class MissionData
{
    public Mission mission;              
    public string missionDetail;
    public TaskUnit missionUnit;
    public float dueTime;
    public bool isComplete = false;
    public int decreaseHP;
    public string deadlineSecond;
}

public class MissionInfo : MonoBehaviour
{
    public MissionData missionData;

    public Mission mission => missionData.mission;
    public TaskUnit missionUnit => missionData.missionUnit;
    public float dueTime
    {
        get => missionData.dueTime;
        set => missionData.dueTime = value;
    }
    public bool isComplete
    {
        get => missionData.isComplete;
        set => missionData.isComplete = value;
    }
    public int decreaseHP => missionData.decreaseHP;

    [Header("UI")]
    [SerializeField] private GameObject repeatTextObj;
    [SerializeField] private TextMeshProUGUI titleTextUI;
    [SerializeField] private TextMeshProUGUI expTextUI;
    [SerializeField] private TextMeshProUGUI dueTextUI;

    private void Awake()
    {
        if (missionData == null)
            missionData = new MissionData();
    }

    private void Update()
    {
        if (missionData == null) return;

        ShowDeadline();
        FailMissonCheck();
    }

    /// <summary>
    /// HP감소량 설정
    /// </summary>
    public void SetDecreaseHP()
    {
        switch (missionUnit)
        {
            case TaskUnit.Day:
                missionData.decreaseHP = 90;
                break;
            case TaskUnit.Week:
                missionData.decreaseHP = 300;
                break;
            case TaskUnit.Month:
                missionData.decreaseHP = 700;
                break;
            case TaskUnit.Personal:
                missionData.decreaseHP = 110;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// UI 보이기
    /// </summary>
    public void ShowMissionUI()
    {
        titleTextUI.text = missionData.mission.Title;
        expTextUI.text = missionData.mission.getExp.ToString();
        repeatTextObj.SetActive(missionData.mission.isRepeat);
    }

    public void SetDeadline()
    {
        missionData.deadlineSecond = DateTime.Now.AddSeconds(missionData.dueTime).ToString();
    }
    public void StartTimer()
    {
        InvokeRepeating("FlowTime", 1f,1f);
    }
    void FlowTime()
    {
        missionData.dueTime -= 1;
    }

    /// <summary>
    /// 반복 시간 재설정
    /// </summary>
    public void SetRepeatTime()
    {
        switch (missionData.missionUnit)
        {
            case TaskUnit.Day:
                missionData.dueTime += CalTimeUtility.dayUnit;
                break;
            case TaskUnit.Week:
                missionData.dueTime += (CalTimeUtility.dayUnit*7);
                break;
            case TaskUnit.Month:
                DateTime now  = DateTime.Now;
                missionData.dueTime += (CalTimeUtility.dayUnit*CalTimeUtility.AddMonthDay(now.Month, now.Year));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 남은 시간 보이기
    /// </summary>
    void ShowDeadline()
    {
        int dueDay = (int)missionData.dueTime / 86400;
        int restHour = (int)missionData.dueTime % 86400;
        int dueHour = (int)restHour / 3600;
        int restMinute = (int)missionData.dueTime % 3600;
        int dueMinute = (int)restMinute / 60;
        int dueSecond = (int)missionData.dueTime % 60;

        if (dueDay >= 1)
        {
            dueTextUI.text = $" {dueDay}일 {dueHour} : {dueMinute}";
        }
        else dueTextUI.text = $" {dueHour} : {dueMinute} : {dueSecond}";
    }
    /// <summary>
    /// 미션 실패
    /// </summary>
    public void FailMissonCheck()
    {
        if (missionData.dueTime > 0) return;

        GameManager.Instance._player.DecreaseHP(missionData.decreaseHP);
        GameManager.Instance._failMissionList.Add(this);
        SaveManager.Instance.Data.activeMissions.Remove(missionData);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// 미션 완료
    /// </summary>
    public void MissonComplete()
    {
        missionData.isComplete = true;
        UIManager.UIInstance.OpenCompleteUI(missionData.mission, this);
    }
    
    /// <summary>
    /// 자세히 보기
    /// </summary>
    public void ShowMissonDetail()
    {
        UIManager.UIInstance.OpenDetailUI(this);
    }
    /// <summary>
    /// 미션 삭제
    /// </summary>
    public void DeleteMission()
    {
        SaveManager.Instance.Data.activeMissions.Remove(this.missionData);
        UIManager.UIInstance.OpenDeletelUI(this);
    }
}
