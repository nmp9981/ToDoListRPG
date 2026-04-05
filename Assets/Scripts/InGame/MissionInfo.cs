using System;
using TMPro;
using UnityEngine;

public struct Mission
{
    public string Title;
    public int getExp;
    public int getMoney;
    public bool isRepeat;
}

public class MissionInfo : MonoBehaviour
{
    public Mission mission;
    public string missionDetail;
    public TaskUnit missionUnit;
    public float dueTime;//마감 기한(실시간)
    public bool isComplete = false;//미션 완료 여부
    public int decreaseHP;//감소 HP
    public string deadlineSecond;//마감 기한(초)

    [Header("UI")]
    [SerializeField] private GameObject repeatTextObj;
    [SerializeField] private TextMeshProUGUI titleTextUI;
    [SerializeField] private TextMeshProUGUI expTextUI;
    [SerializeField] private TextMeshProUGUI moneyTextUI;
    [SerializeField] private TextMeshProUGUI dueTextUI;

    private void Awake()
    {
        SetDecreaseHP();
    }

    private void Update()
    {
        ShowDeadline();
        FailMissonCheck();
    }

    /// <summary>
    /// HP감소량 설정
    /// </summary>
    void SetDecreaseHP()
    {
        switch (missionUnit)
        {
            case TaskUnit.Day:
                decreaseHP = 90;
                break;
            case TaskUnit.Week:
                decreaseHP = 300;
                break;
            case TaskUnit.Month:
                decreaseHP = 700;
                break;
            case TaskUnit.Personal:
                decreaseHP = 110;
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
        titleTextUI.text = mission.Title;
        expTextUI.text = mission.getExp.ToString();
        moneyTextUI.text = mission.getMoney.ToString();
        repeatTextObj.SetActive(mission.isRepeat);
    }

    public void SetDeadline()
    {
        deadlineSecond = DateTime.Now.AddSeconds(dueTime).ToString();
    }
    public void StartTimer()
    {
        InvokeRepeating("FlowTime", 1f,1f);
    }
    void FlowTime()
    {
        dueTime -= 1;
    }

    /// <summary>
    /// 반복 시간 재설정
    /// </summary>
    public void SetRepeatTime()
    {
        switch (missionUnit)
        {
            case TaskUnit.Day:
                dueTime += CalTimeUtility.dayUnit;
                break;
            case TaskUnit.Week:
                dueTime += (CalTimeUtility.dayUnit*7);
                break;
            case TaskUnit.Month:
                DateTime now  = DateTime.Now;
                dueTime += (CalTimeUtility.dayUnit*CalTimeUtility.AddMonthDay(now.Month, now.Year));
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
        int dueDay = (int)dueTime / 86400;
        int restHour = (int)dueTime % 86400;
        int dueHour = (int)restHour / 3600;
        int restMinute = (int)dueTime % 3600;
        int dueMinute = (int)restMinute / 60;
        int dueSecond = (int)dueTime % 60;

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
        if (dueTime > 0) return;

        GameManager.Instance._player.DecreaseHP(decreaseHP);
        GameManager.Instance._failMissionList.Add(this);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// 미션 완료
    /// </summary>
    public void MissonComplete()
    {
        isComplete = true;
        UIManager.UIInstance.OpenCompleteUI(mission,this);
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
        UIManager.UIInstance.OpenDeletelUI(this);
    }
}
