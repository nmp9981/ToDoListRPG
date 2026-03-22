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
    public float dueTime;//마감 기한
    public bool isComplete = false;//미션 완료 여부
    public int decreaseHP;//감소 HP

    [Header("UI")]
    public GameObject repeatTextObj; 

    private void Awake()
    {
        SetDecreaseHP();
    }

    private void Update()
    {
        ShowDeadline();
    }

    /// <summary>
    /// HP감소량 설정
    /// </summary>
    void SetDecreaseHP()
    {
        switch (missionUnit)
        {
            case TaskUnit.Day:
                decreaseHP = 20;
                break;
            case TaskUnit.Week:
                decreaseHP = 80;
                break;
            case TaskUnit.Month:
                decreaseHP = 400;
                break;
            case TaskUnit.Personal:
                decreaseHP = 100;
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
        dueTime -= Time.deltaTime;
    }

    /// <summary>
    /// 미션 완료
    /// </summary>
    public void MissonComplete()
    {
        isComplete = true;
    }
    /// <summary>
    /// 자세히 보기
    /// </summary>
    public void ShowMissonDetail()
    {

    }
}
