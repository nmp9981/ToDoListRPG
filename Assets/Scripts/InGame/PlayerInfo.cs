using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TaskUnit
{
    Day,
    Week,
    Month,
    Year,
    Count
}

/// <summary>
/// 과제 정보
/// </summary>
public struct TaskInfo
{
    public string _taskName;//과제 명
    public TaskUnit _taskUnit;//과제 단위
    public bool _isFinish;//완료 여부
    public float _durationTime;//지속시간
    public int _rewardExp;//보상 경험치
}

public class PlayerInfo : MonoBehaviour
{
    public List<TaskInfo> _taskInfoList;//과제 리스트
    public string _playerName;//캐릭터 명
    public Image _spriteImage;//캐릭터 아이콘



}
