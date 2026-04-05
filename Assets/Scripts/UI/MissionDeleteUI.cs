using UnityEngine;

public class MissionDeleteUI : MonoBehaviour
{
    /// <summary>
    /// 미션 진짜로 삭제
    /// </summary>
    /// <param name="mission"></param>
    public void RealDeleteMission()
    {
        UIManager.UIInstance.CloseDeleteUI();
        Destroy(UIManager.UIInstance._deleteMissionSoon.gameObject);
    }
}
