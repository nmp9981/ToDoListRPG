using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Image _concentrationImage;//집중 이미지
    [SerializeField] private Sprite[] _concentrationImageSet = new Sprite[2];//집중 이미지

    /// <summary>
    /// 일반 모드로 변경
    /// </summary>
    public void Change_GeneralMode()
    {
        GameManager.Instance.PlayMode = PlayMode.General;
        _concentrationImage.sprite = _concentrationImageSet[0];
    }
    public void Change_ConcentrationMode()
    {
        GameManager.Instance.PlayMode = PlayMode.Concentration;
        _concentrationImage.sprite = _concentrationImageSet[1];
    }
}
