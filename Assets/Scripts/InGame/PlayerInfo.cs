using System;
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
[Serializable]
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
    public string _playerName = string.Empty;//캐릭터 명
    public Image _spriteImage;//캐릭터 아이콘


    private int _playerLv;
    private int _playerSymbolIndex;
    private int _playerCurrentHP;
    private int _playerCurrentEXP;
    private int _playerFullHP = 1000;
    private int _playerFullExp;
    private int _playerIncreaseHP = 500;
    private int _playerDecreaseEXPPercent = 30;

    private float _consumeConcentrateTime = 0;
    private int _countOtherAction=0;
    private int _countCompleteTODO=0;
    private float _todayLossHP=0;

    private float _totalConcentrateTime = 0;
    private float _totalCompleteMission = 0;

    public Stack<float> _weekConcentrateTimeStack = new();//실제 통계반영
    public List<DailyFocusRecord> _dailyFocusRecordList = new();//데이터 저장용

    // ===== SaveData 가리키는 단축 프로퍼티 =====
    private SaveData D => SaveManager.Instance.Data;

    //===== 캐릭터 기본 정보 (SaveData 위임) =====
    public int PlayerLV { get { return D.level; } set { D.level = value; } }
    public int PlayerSymbolIndex { get { return D.titleIdx; } set { D.titleIdx = value; } }
    public int PlayerCurrentHP { get { return D.hp; } set { D.hp = value; } }
    public int PlayerCurrentExp { get { return D.exp; } set { D.exp = value; } }
    public int PlayerFullHP { get { return D.playerFullHp; } set { D.playerFullHp = value; } }
    public int PlayerFullExp { get { return D.playerFullExp; } set { D.playerFullExp = value; } }
    public int PlayerDecreaseExpPercent { get { return _playerDecreaseEXPPercent; } }

    public float ConsumeConcentrateTime { get { return D.todayConcentrateSeconds; } set { D.todayConcentrateSeconds = value; } }
    public int CountOtherAction { get { return D.countOtherAction; } set { D.countOtherAction = value; } }
    public int CountCompleteTODO { get { return D.todayMissionCompleted; } set { D.todayMissionCompleted = value; } }
    public float TodayLossHP { get { return D.todayLossHP; } set { D.todayLossHP = value; } }

    public float TotalConcentrateTime { get { return D.totalFocusSeconds; } set { D.totalFocusSeconds = value; } }
    public int TotalCompleteMission { get { return D.totalMissionsCompleted; } set { D.totalMissionsCompleted = value; } }

    private void Awake()
    {
        var data = SaveManager.Instance.Data;

        if (GameManager.Instance != null)
            GameManager.Instance._player = this;

        // 첫 실행인지 판단
        bool isFirstRun = string.IsNullOrEmpty(data.playerName) || data.level == 0
                  || data.totalFocusSeconds == 0;

        if (isFirstRun)
        {
            // 첫 실행 → 초기값 세팅
            InitPlayerInfo();
            UIManager.UIInstance.InitConcentrateText();
        }
        UIManager.UIInstance.UpdateUI();   // 로드된 값으로 UI 한번 갱신
    }

    /// <summary>
    /// 캐릭터 정보 초기화
    /// </summary>
    public void InitPlayerInfo()
    {
        PlayerLV = 1;
        PlayerCurrentHP = PlayerFullHP;
        PlayerCurrentExp = 0;
        PlayerFullExp = 300;
        PlayerSymbolIndex = 0;

        _weekConcentrateTimeStack.Clear();
        _dailyFocusRecordList.Clear();

        UIManager.UIInstance.UpdateUI();
    }
    /// <summary>
    /// 집중 정보 초기화
    /// </summary>
    public void InitConcentrateInfo()
    {
        ConsumeConcentrateTime = 0;
        CountOtherAction = 0;
        CountCompleteTODO = 0;
        TodayLossHP = 0;
    }

    public void GetReward(int xp)
    {
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
            PlayerLV += 1;
            PlayerFullExp = gm.CalRequireExp(PlayerLV);
            PlayerCurrentHP = Mathf.Min(PlayerFullHP, PlayerCurrentHP+_playerIncreaseHP);

            //칭호 변경
            if (PlayerLV >= gm._playerLvSymbolImage[PlayerSymbolIndex]._nextSymbolLv
                && PlayerSymbolIndex < gm.SymbolMaxCount-1) PlayerSymbolIndex += 1;
        }
        //UI 반영
        UIManager.UIInstance.UpdateUI();
        UIManager.UIInstance.ShowMessage("레벨 업!!", Color.white);
    }
    /// <summary>
    /// 레벨 다운
    /// </summary>
    public void PlayerLevelDown(int exp)
    {
        var gm = GameManager.Instance;
        PlayerCurrentHP = _playerIncreaseHP;
        //10미만에서는 감소X
        if (PlayerLV < 10)
        {
            UIManager.UIInstance.UpdateUI();
            return;
        }

        //레벨 다운
        PlayerLV -= 1;
        PlayerFullExp = gm.CalRequireExp(PlayerLV);
        PlayerCurrentExp = PlayerFullExp + exp;

        //칭호 변경
        if (PlayerLV < gm._playerLvSymbolImage[PlayerSymbolIndex]._nextSymbolLv
            && PlayerSymbolIndex > 0) PlayerSymbolIndex -= 1;
        //UI 반영
        UIManager.UIInstance.UpdateUI();
    }
    /// <summary>
    /// HP 감소
    /// </summary>
    /// <param name="amountHP"></param>
    public void DecreaseHP(int amountHP)
    {
        PlayerCurrentHP = Mathf.Max(0, PlayerCurrentHP - amountHP);

        if (PlayerCurrentHP <= 0)
        {
            DeadPlayer();
        }
        else
        {
            UIManager.UIInstance.UpdateUI();
            UIManager.UIInstance.ShowMessage("HP가 감소했습니다.", Color.red);
        }
    }
    /// <summary>
    /// 사망 처리
    /// </summary>
    public void DeadPlayer()
    {
        int amountDecrease = (PlayerFullExp * _playerDecreaseEXPPercent) / 100;//감소량
        PlayerCurrentExp -= amountDecrease;

        if (PlayerCurrentExp < 0)
        {
            PlayerLevelDown(PlayerCurrentExp);
        }
        else UIManager.UIInstance.UpdateUI();

        UIManager.UIInstance.ShowMessage("캐릭터가 사망했습니다.", Color.white);
    }
}
