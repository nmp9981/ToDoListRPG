using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
        _playerNameText.text = $"{GameManager.Instance._player._playerName}";
        _playerLvText.text = $"{GameManager.Instance._player.PlayerLV}";
        _playerSymbolText.text = $"{GameManager.Instance._player.PlayerSymbol}";

        var rate = Cal_HP_EXPRate();
        _playerHPText.text = $"{rate.hpRate:F1}%";
        _playerEXPText.text = $"{GameManager.Instance._player.PlayerCurrentExp} [{rate.expRate100:F2}%]";

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
}
