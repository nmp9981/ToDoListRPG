using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UIManager : MonoBehaviour
{
    static UIManager _uiInstance;
    public static UIManager UIInstance { get { Init();  return _uiInstance; } }

    [Header("캐릭터 정보")]
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerLvText;
    [SerializeField] private TextMeshProUGUI _playerSymbolText;
    [SerializeField] private TextMeshProUGUI _playerMoneyText;
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

    private Mission _selectMisson;
    public MissionInfo _deleteMissionSoon;

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
                case "MoneyText":
                    _playerMoneyText = txt;
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
        _playerMoneyText.text = $"{player.PlayerHasMoney}";
        _titleImage.sprite = GameManager.Instance._playerLvSymbolImage[player.PlayerSymbolIndex]._spriteImage;

        var rate = Cal_HP_EXPRate();
        _playerHPText.text = $"HP : {rate.hpRate*100:F1}%";
        _playerEXPText.text = $"EXP : {player.PlayerCurrentExp} [{rate.expRate100:F2}%]";

        _hpBarImage.fillAmount = rate.hpRate;
        _expBarImage.fillAmount = rate.expRate;
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
        player.GetReward(_selectMisson.getExp, _selectMisson.getMoney);
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
        CloseCompleteUI();
    }
    public void OpenDetailUI(MissionInfo mission)
    {
        _missionDetailUI.gameObject.SetActive(true);
        _missionDetailUI.ShowDetail(mission);
    }
    public void CloseDetailUI()
    {
        _missionDetailUI.gameObject.SetActive(false);
    }
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
}
