using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private GameObject[] missionPages = new GameObject[4];
    [SerializeField] private Transform[] missionPivotPoint = new Transform[4];
    [SerializeField] private GameObject addMissionEnrollObj;

    private void Start()
    {
        RestoreMissions();
    }

    /// <summary>
    /// 저장된 미션을 적절한 페이지에 다시 그리기
    /// </summary>
    private void RestoreMissions()
    {
        var savedMissions = SaveManager.Instance.Data.activeMissions;
        Debug.Log($"[MissionUI] 미션 {savedMissions.Count}개 복원");

        //저장된 미션이 없음
        if (savedMissions.Count == 0) return;

        foreach (var data in savedMissions)
        {
            // missionUnit에 맞는 페이지를 부모로 사용
            int pageIndex = (int)data.missionUnit;

            // 안전 체크 (Count 같은 잘못된 값 방지)
            if (pageIndex < 0 || pageIndex >= missionPages.Length)
            {
                Debug.LogWarning($"[MissionUI] 잘못된 missionUnit: {data.missionUnit}");
                continue;
            }

            Transform parent = missionPivotPoint[pageIndex];

            GameObject obj = Instantiate(GameManager.Instance._missionPrefab, parent);
            MissionInfo info = obj.GetComponent<MissionInfo>();

            info.missionData = data;
            info.SetDecreaseHP();
            info.ShowMissionUI();
            info.StartTimer();
        }
    }


    /// <summary>
    /// 미션 추가
    /// </summary>
    public void AddMission()
    {
        addMissionEnrollObj.SetActive(true);
    }

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
