using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Image _concentrationImage;//집중 이미지
    [SerializeField] private Sprite[] _concentrationImageSet = new Sprite[2];//집중 이미지
    [SerializeField] GameObject _programSetObj;//프로그램 세팅 오브젝트
    [SerializeField] GameObject _continueTimeSetObj;//지속시간 설정 오브젝트

    private void Start()
    {
        _continueTimeSetObj.gameObject.SetActive(false);
    }

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

        UIManager.UIInstance.ShowRestTimeUI();
    }
    public void OpenProgramSetUI()
    {
        _programSetObj.SetActive(true);
    }
    public void CloseProgramSetUI()
    {
        _programSetObj.SetActive(false);
    }
    public void OpenContinueTimeSetUI()
    {
        _continueTimeSetObj.SetActive(true);
    }
    public void CloseContinueTimeSetUI()
    {
        _continueTimeSetObj.SetActive(false);
    }
    /// <summary>
    /// 집중 지속 시간 설정
    /// </summary>
    /// <param name="time"></param>
    public void SetConcentRateTime(int time)
    {
        GameManager.Instance.ConcentrateContinueTime = time;
    }
}
