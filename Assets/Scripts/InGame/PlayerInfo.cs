using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TaskUnit
{
    Day,
    Week,
    Month,
    Personal,
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

[System.Serializable]
public struct SymbolInfo
{
    public int _nextSymbolLv;
    public string _symbolName;
    public Sprite _spriteImage;
}

public class PlayerInfo : MonoBehaviour
{
    public List<TaskInfo> _taskInfoList;//과제 리스트
    public string _playerName = "나이트로드";//캐릭터 명
    public Image _spriteImage;//캐릭터 아이콘


    private int _playerLv;
    private int _playerSymbolIndex;
    private int _playerHasMoney;
    private int _playerCurrentHP;
    private int _playerCurrentEXP;
    private int _playerFullHP = 1000;
    private int _playerFullExp;
    private int _playerIncreaseHP = 500;

    public int PlayerLV { get { return _playerLv; } set { _playerLv = value; } }
    public int PlayerSymbolIndex { get { return _playerSymbolIndex; } set { _playerSymbolIndex = value; } }
    public int PlayerHasMoney { get { return _playerHasMoney; } set { _playerHasMoney = value; } }
    public int PlayerCurrentHP { get { return _playerCurrentHP; } set { _playerCurrentHP = value; } }
    public int PlayerCurrentExp { get { return _playerCurrentEXP; } set { _playerCurrentEXP = value; } }
    public int PlayerFullHP { get { return _playerFullHP; } set { _playerFullHP = value; } }
    public int PlayerFullExp { get { return _playerFullExp; } set { _playerFullExp = value; } }

    private void Awake()
    {
        InitPlayerInfo();
    }

    /// <summary>
    /// 캐릭터 정보 초기화
    /// </summary>
    public void InitPlayerInfo()
    {
        PlayerLV = 1;
        PlayerHasMoney = 1000;
        PlayerCurrentHP = PlayerFullHP;
        PlayerCurrentExp = 20;
        PlayerFullExp = 800;
        PlayerSymbolIndex = 0;

        UIManager.UIInstance.UpdateUI();
    }

    public void GetReward(int xp, int money)
    {
        PlayerHasMoney += money;
        PlayerCurrentExp += xp;
        PlayerLevelUP();
    }

    /// <summary>
    /// 레벨 업
    /// </summary>
    public void PlayerLevelUP()
    {
        //레벨업
        if (PlayerCurrentExp >= PlayerFullExp)
        {
            var gm = GameManager.Instance;
            PlayerCurrentExp -= PlayerFullExp;
            PlayerFullExp = (PlayerFullExp * gm.RequireExpRate) / 100;
            PlayerCurrentHP = Mathf.Min(PlayerFullHP, PlayerCurrentHP+_playerIncreaseHP);
            PlayerLV += 1;

            //칭호 변경
            if (PlayerLV >= gm._playerLvSymbolImage[PlayerSymbolIndex]._nextSymbolLv
                && PlayerSymbolIndex < gm.SymbolMaxCount-1) PlayerSymbolIndex += 1;
        }
        //UI 반영
        UIManager.UIInstance.UpdateUI();
    }
}
