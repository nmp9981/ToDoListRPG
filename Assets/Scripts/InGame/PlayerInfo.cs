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


    private int _playerLv;
    private string _playerSymbol;
    private int _playerHasMoney;
    private int _playerCurrentHP;
    private int _playerCurrentEXP;
    private int _playerFullHP = 1000;
    private int _playerFullExp;

    public int PlayerLV { get { return _playerLv; } set { _playerLv = value; } }
    public string PlayerSymbol { get { return _playerSymbol; } set { _playerSymbol = value; } }
    public int PlayerHasMoney { get { return _playerHasMoney; } set { _playerHasMoney = value; } }
    public int PlayerCurrentHP { get { return _playerCurrentHP; } set { _playerCurrentHP = value; } }
    public int PlayerCurrentExp { get { return _playerCurrentEXP; } set { _playerCurrentEXP = value; } }
    public int PlayerFullHP { get { return _playerFullHP; } set { _playerFullHP = value; } }
    public int PlayerFullExp { get { return _playerFullExp; } set { _playerFullExp = value; } }

}
