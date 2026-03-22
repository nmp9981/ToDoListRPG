using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private GameObject[] missionPages = new GameObject[4];

    /// <summary>
    /// 일일미션 보이기
    /// </summary>
    public void ShowDayMission()
    {
        for (int i = 0; i < 4; i++) missionPages[i].SetActive(false);
        missionPages[0].SetActive(true);
    }
    /// <summary>
    /// 주간미션 보이기
    /// </summary>
    public void ShowWeekMission()
    {
        for (int i = 0; i < 4; i++) missionPages[i].SetActive(false);
        missionPages[1].SetActive(true);
    }
    /// <summary>
    /// 월간미션 보이기
    /// </summary>
    public void ShowMonthMission()
    {
        for (int i = 0; i < 4; i++) missionPages[i].SetActive(false);
        missionPages[2].SetActive(true);
    }
    /// <summary>
    /// 개인미션 보이기
    /// </summary>
    public void ShowPersonalMission()
    {
        for (int i = 0; i < 4; i++) missionPages[i].SetActive(false);
        missionPages[3].SetActive(true);
    }
}
