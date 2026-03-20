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
    public int dueTime;//마감 기한

    public int decreaseHP;//감소 HP

    /// <summary>
    /// 미션 완료
    /// </summary>
    public void MissonComplete()
    {

    }
    /// <summary>
    /// 자세히 보기
    /// </summary>
    public void ShowMissonDetail()
    {

    }
}
