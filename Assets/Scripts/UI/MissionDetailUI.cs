using TMPro;
using UnityEngine;

public class MissionDetailUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dueDateText;

    /// <summary>
    /// 세부 사할 보이기
    /// </summary>
    /// <param name="mission"></param>
    public void ShowDetail(MissionInfo mission)
    {
        dueDateText.text = string.Empty;

        dueDateText.text = mission.missionData.missionDetail + "\n\n"+mission.missionData.deadlineSecond;
    }
}
