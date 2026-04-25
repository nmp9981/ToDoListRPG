using System.Numerics;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UIManager : MonoBehaviour
{
    static UIManager _uiInstance;
    public static UIManager UIInstance { get { Init();  return _uiInstance; } }

    [Header("캐릭터 정보")]
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerLvText;
    [SerializeField] private TextMeshProUGUI _playerSymbolText;
    [SerializeField] private TextMeshProUGUI _playerHPText;
    [SerializeField] private TextMeshProUGUI _playerEXPText;

    [Header("캐릭터 이미지")]
    [SerializeField] private Image _titleImage;//칭호

    [Header("HP/MP 바")]
    [SerializeField] private Image _hpBarImage;
    [SerializeField] private Image _expBarImage;

    [Header("UI오브젝트")]
    [SerializeField] private MissionCompleteUI _missionCompleteUI;
    [SerializeField] private MissionDetailUI _missionDetailUI;
    [SerializeField] private MissionDeleteUI _missionDeleteUI;
    [SerializeField] private SettingUI _settingUI;
    [SerializeField] private StatusUI _statusUI;
 
    [Header("메세지")]
    [SerializeField] private TextMeshProUGUI _messageText;

    [Header("선택한 미션")]
    private Mission _selectMisson;
    public MissionInfo _deleteMissionSoon;

    [Header("집중 지속 시간")]
    [SerializeField] private TextMeshProUGUI _concentrateRestTimeText;//남은 집중 시간

    [Header("오늘의 집중 통계")]
    [SerializeField] private TextMeshProUGUI _todayConcentrationTimeText;
    [SerializeField] private TextMeshProUGUI _countOtherActionText;
    [SerializeField] private TextMeshProUGUI _todayCompleteTODOText;
    [SerializeField] private TextMeshProUGUI _todayLossHPAmountText;

    static void Init()
    {
        if (_uiInstance == null)
        {
            GameObject gm = GameObject.Find("Canvas");
            if (gm == null)
            {
                gm = new GameObject { name = "Canvas" };

                gm.AddComponent<UIManager>();
            }
            _uiInstance = gm.GetComponent<UIManager>();
        }
    }

    private void Awake()
    {
        TextBinding();
        _settingUI.Change_GeneralMode();
    }

    private void Update()
    {
        DecreaseConcentrateRestTime();
        UpdateConcentrateText();
    }

    void TextBinding()
    {
        foreach (TextMeshProUGUI txt in this.gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
            string objname = txt.gameObject.name;

            switch (objname)
            {
                case "NameText":
                    _playerNameText= txt;
                    break;
                case "LvText":
                    _playerLvText = txt;
                    break;
                case "SymbolText":
                    _playerSymbolText = txt;
                    break;
                case "HPText":
                    _playerHPText = txt;
                    break;
                case "ExpText":
                    _playerEXPText = txt;
                    break;
                default:
                    break;
            }
        }
        _messageText.text = string.Empty;
    }

    /// <summary>
    /// 남은 집중 시간 감소
    /// </summary>
    public void DecreaseConcentrateRestTime()
    {
        if (GameManager.Instance.PlayMode == PlayMode.General)
        {
            _concentrateRestTimeText.text = string.Empty;
            return;
        }
        if (GameManager.Instance.PlayMode == PlayMode.Concentration)
        {
            var player = GameManager.Instance._player;
            GameManager.Instance.ConcentrateContinueTime -= Time.deltaTime;
            player.ConsumeConcentrateTime += Time.deltaTime;
            player.TotalConcentrateTime += Time.deltaTime;
            ShowRestTimeUI();
            EndConcentrateMode();
        }
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    public void UpdateUI()
    {
        var player = GameManager.Instance._player;
        _playerNameText.text = $"이름 : {player._playerName}";
        _playerLvText.text = $"Lv. {player.PlayerLV}";
        _playerSymbolText.text = $"{GameManager.Instance._playerLvSymbolImage[player.PlayerSymbolIndex]._symbolName}";
        _titleImage.sprite = GameManager.Instance._playerLvSymbolImage[player.PlayerSymbolIndex]._spriteImage;

        var rate = Cal_HP_EXPRate();
        _playerHPText.text = $"HP : {rate.hpRate*100:F1}%";
        _playerEXPText.text = $"EXP : {player.PlayerCurrentExp} [{rate.expRate100:F2}%]";

        _hpBarImage.fillAmount = rate.hpRate;
        _expBarImage.fillAmount = rate.expRate;
    }
    /// <summary>
    /// 집중 관련 UI 초기화
    /// </summary>
    public void InitConcentrateText()
    {
        _todayConcentrationTimeText.text = $"오늘의 집중 시간 : 0시간 0분";
        _countOtherActionText.text = $"딴짓 적발 횟수 : 0회";
        _todayCompleteTODOText.text = $"오늘 완료한 미션 횟수 : 0";
        _todayLossHPAmountText.text = $"오늘 잃은 HP : 0.0%";
    }
    /// <summary>
    /// 집중 관련 UI 업데이트
    /// </summary>
    public void UpdateConcentrateText()
    {
        var player = GameManager.Instance._player;
        int hour = (int)player.ConsumeConcentrateTime / 3600;
        int hourRest = (int)player.ConsumeConcentrateTime % 3600;
        int minute = (int)hourRest / 60;
        _todayConcentrationTimeText.text = $"오늘의 집중 시간 : {hour}시간 {minute}분";
        _countOtherActionText.text = $"딴짓 적발 횟수 : {player.CountOtherAction}회";
        _todayCompleteTODOText.text = $"오늘 완료한 미션 횟수 : {player.CountCompleteTODO}";
        int Prime = (int)player.TodayLossHP % 10;
        int intN = (int)player.TodayLossHP / 10;
        _todayLossHPAmountText.text = $"오늘 잃은 HP : {intN}.{Prime}%";
    }
    /// <summary>
    /// 남은 집중 지속 시간 UI로 표시
    /// </summary>
    public void ShowRestTimeUI()
    {
        int restInitTime = (int)GameManager.Instance.ConcentrateContinueTime;
        if (restInitTime<=-50)
        {
            _concentrateRestTimeText.text = string.Empty;
        }
        else _concentrateRestTimeText.text = (restInitTime >= 60) ? (restInitTime/60).ToString() : restInitTime.ToString();
    }
    /// <summary>
    /// 집중 모드 종료
    /// </summary>
    public void EndConcentrateMode()
    {
        if(GameManager.Instance.ConcentrateContinueTime<=0 && GameManager.Instance.ConcentrateContinueTime > -10)
        {
            _concentrateRestTimeText.text = string.Empty;
            _settingUI.Change_GeneralMode();
        }
    }
    /// <summary>
    /// HP, EXP 비율 계산
    /// </summary>
    /// <returns></returns>
    private (float hpRate, float expRate, float expRate100) Cal_HP_EXPRate()
    {
        float rateHP = (float)GameManager.Instance._player.PlayerCurrentHP / (float)GameManager.Instance._player.PlayerFullHP;
        float rateExp = (float)GameManager.Instance._player.PlayerCurrentExp / (float)GameManager.Instance._player.PlayerFullExp;
        float rate100Exp = rateExp * 100;

        return (rateHP, rateExp, rate100Exp);
    }

    /// <summary>
    /// 미션 완료 UI 열기
    /// </summary>
    public void OpenCompleteUI(Mission mission, MissionInfo missionInfo)
    {
        _selectMisson = mission;
        _deleteMissionSoon = missionInfo;
        _missionCompleteUI.gameObject.SetActive(true);
    }
    /// <summary>
    /// 미션 완료 UI 닫기
    /// </summary>
    public void CloseCompleteUI()
    {
        _missionCompleteUI.gameObject.SetActive(false);
    }
    /// <summary>
    /// 미션 완료 확정
    /// </summary>
    public void ClickMissonCompleteButton()
    {
        var player = GameManager.Instance._player;
        player.GetReward(_selectMisson.getExp);
        //반복 여부에 따라 삭제할지 결정
        if (!_selectMisson.isRepeat)//반복 아니면 삭제
        {
            Destroy(_deleteMissionSoon.gameObject);
        }
        else
        {
            //남은 시간 재설정
            _deleteMissionSoon.SetRepeatTime();
        }
        _deleteMissionSoon.isComplete = false;
        GameManager.Instance._player.CountCompleteTODO += 1;
        GameManager.Instance._player.TotalCompleteMission += 1;
        CloseCompleteUI();
    }
    #region 미션 세부 UI
    public void OpenDetailUI(MissionInfo mission)
    {
        _missionDetailUI.gameObject.SetActive(true);
        _missionDetailUI.ShowDetail(mission);
    }
    public void CloseDetailUI()
    {
        _missionDetailUI.gameObject.SetActive(false);
    }
    #endregion

    #region 삭제 UI
    public void OpenDeletelUI(MissionInfo mission)
    {
        _deleteMissionSoon = mission;
        _missionDeleteUI.gameObject.SetActive(true);
    }
    public void CloseDeleteUI()
    {
        _deleteMissionSoon = null;
        _missionDeleteUI.gameObject.SetActive(false);
    }
    #endregion

    #region 메세지
    public void ShowMessage(string msg, Color color)
    {
        _messageText.text = msg;
        _messageText.color = color;
        Invoke("DeleteMessage",1f);
    }
    void DeleteMessage()
    {
        _messageText.text = string.Empty;
    }
    #endregion

    #region 세팅 창
    public void OpenSettingUI()
    {
        _settingUI.gameObject.SetActive(true);
    }
    public void CloseSettingUI()
    {
        _settingUI.gameObject.SetActive(false);
    }
    #endregion

    #region 통계창
    public void OpenStatusUI()
    {
        _statusUI.gameObject.SetActive(true);
        _statusUI.OpenSettingStatusUI();
    }
    public void CloseStatusUI()
    {
        _statusUI.gameObject.SetActive(false);
    }
    #endregion
}
